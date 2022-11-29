﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Bot.Builds;
using Bot.ExtensionMethods;
using Bot.GameData;
using Bot.GameSense;
using Bot.GameSense.RegionTracking;
using Bot.Managers.WarManagement.ArmySupervision;
using Bot.MapKnowledge;
using Bot.Utils;
using SC2APIProtocol;

namespace Bot.Managers.WarManagement.States.MidGame;

public class MidGameBehaviour : IWarManagerBehaviour {
    private const float RequiredForceRatioBeforeAttacking = 1.5f;
    private const float MaxSupplyBeforeAttacking = 175;

    private static readonly HashSet<uint> ManageableUnitTypes = Units.ZergMilitary.Except(new HashSet<uint> { Units.Queen, Units.QueenBurrowed }).ToHashSet();

    private readonly BuildRequest _antiTerranBuildRequest = new TargetBuildRequest(BuildType.Train, Units.Corruptor, targetQuantity: 10, priority: BuildRequestPriority.VeryHigh, blockCondition: BuildBlockCondition.All);
    private BuildRequest _armyBuildRequest = new TargetBuildRequest(BuildType.Train, Units.Roach, targetQuantity: 100, priority: BuildRequestPriority.Low);

    private readonly MidGameBehaviourDebugger _debugger = new MidGameBehaviourDebugger();
    private readonly WarManager _warManager;

    public Stance Stance { get; private set; } = Stance.Defend;
    public readonly ArmySupervisor AttackSupervisor = new ArmySupervisor();
    public readonly ArmySupervisor DefenseSupervisor = new ArmySupervisor();
    public readonly ArmySupervisor TerranFinisherSupervisor = new ArmySupervisor();

    public IAssigner Assigner { get; }
    public IDispatcher Dispatcher { get; }
    public IReleaser Releaser { get; }

    public List<BuildRequest> BuildRequests { get; } = new List<BuildRequest>();

    public MidGameBehaviour(WarManager warManager) {
        _warManager = warManager;

        BuildRequests.Add(_armyBuildRequest);
        TerranFinisherSupervisor.AssignTarget(new Vector2(MapAnalyzer.MaxX / 2f, MapAnalyzer.MaxY / 2f), 999, canHuntTheEnemy: true);

        Assigner = new WarManagerAssigner<MidGameBehaviour>(this);
        Dispatcher = new MidGameDispatcher(this);
        Releaser = new WarManagerReleaser<MidGameBehaviour>(this);
    }

    public void RecruitmentPhase() {
        _warManager.Assign(Controller.GetUnits(UnitsTracker.NewOwnedUnits, ManageableUnitTypes));
    }

    public void DispatchPhase() {
        _warManager.Dispatch(_warManager.ManagedUnits.Where(soldier => soldier.Supervisor == null));
    }

    public void ManagementPhase() {
        if (TheGameIsOurs()) {
            FinishHim();
        }
        else {
            BigBrainPlay();
        }

        AdjustBuildRequests();

        if (ShouldFinishOffTerran()) {
            FinishOffTerran();
        }

        if (ShouldFreeSomeSupply()) {
            FreeSomeSupply();
        }
        else {
            AttackSupervisor.OnFrame();
            DefenseSupervisor.OnFrame();
        }

        TerranFinisherSupervisor.OnFrame();

        // TODO GD The war manager could do most of that
        _debugger.OwnForce = _warManager.ManagedUnits.GetForce();
        _debugger.EnemyForce = GetEnemyForce();
        _debugger.CurrentStance = Stance;
        _debugger.BuildPriority = BuildRequests.FirstOrDefault()?.Priority ?? BuildRequestPriority.Low;
        _debugger.BuildBlockCondition = BuildRequests.FirstOrDefault()?.BlockCondition ?? BuildBlockCondition.None;
        _debugger.Debug();
    }

    // TODO GD When we extract finisher from here
    public bool CleanUp() {
        return true;
    }

    /// <summary>
    /// Evaluates if we are overwhelming the opponent.
    /// </summary>
    /// <returns>True if we can stop being fancy and just finish the opponent</returns>
    private bool TheGameIsOurs() {
        if (!Stance.HasFlag(Stance.Finisher) && MapAnalyzer.VisibilityRatio < 0.85) {
            return false;
        }

        var ourForce = _warManager.ManagedUnits.GetForce();
        var enemyForce = GetEnemyForce();
        if (ourForce < enemyForce * 3) {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Steamroll the opponent.
    /// </summary>
    private void FinishHim() {
        if (Stance.HasFlag(Stance.Finisher)) {
            return;
        }

        var target = _warManager.ManagedUnits.GetCenter();
        AttackSupervisor.AssignTarget(target, 999, canHuntTheEnemy: true);

        if (Stance.HasFlag(Stance.Defend)) {
            DefenseSupervisor.Retire();
        }

        Stance |= Stance.Attack | Stance.Finisher;
        Stance &= ~Stance.Defend; // Unset the defend flag

        _debugger.Target = target.GetRegion();
    }

    /// <summary>
    /// Decide between attack and defense and pick good targets based on current map control.
    /// </summary>
    private void BigBrainPlay() {
        // Determine regions to defend
        var regionToDefend = GetRegionToDefend();
        DefenseSupervisor.AssignTarget(regionToDefend.Center, regionToDefend.ApproximatedRadius, canHuntTheEnemy: false);

        // Determine regions to attack
        var regionToAttack = GetRegionToAttack();
        AttackSupervisor.AssignTarget(regionToAttack.Center, regionToDefend.ApproximatedRadius, canHuntTheEnemy: true);

        AttackOrDefend(regionToAttack, regionToDefend);
    }

    /// <summary>
    /// Some Terran will fly their buildings.
    /// Check if they are basically dead and we should start dealing with the flying buildings.
    /// </summary>
    /// <returns>True if we should start handling flying terran buildings</returns>
    private bool ShouldFinishOffTerran() {
        if (Controller.EnemyRace != Race.Terran) {
            return false;
        }

        if (!Stance.HasFlag(Stance.Attack)) {
            return false;
        }

        if (Controller.Frame < TimeUtils.SecsToFrames(10 * 60)) {
            return false;
        }

        if (Controller.Frame % TimeUtils.SecsToFrames(5) != 0) {
            return false;
        }

        if (MapAnalyzer.ExplorationRatio < 0.80 || !ExpandAnalyzer.ExpandLocations.All(expandLocation => VisibilityTracker.IsExplored(expandLocation.Position))) {
            return false;
        }

        return Controller.GetUnits(UnitsTracker.EnemyUnits, Units.Buildings).All(building => building.IsFlying);
    }

    /// <summary>
    /// Create anti-air units to deal with terran flying buildings.
    /// </summary>
    private void FinishOffTerran() {
        if (Stance.HasFlag(Stance.TerranFinisher)) {
            return;
        }

        BuildRequests.Add(new TargetBuildRequest(BuildType.Build, Units.Spire, targetQuantity: 1, priority: BuildRequestPriority.VeryHigh, blockCondition: BuildBlockCondition.All));
        BuildRequests.Add(_antiTerranBuildRequest);

        Stance |= Stance.TerranFinisher;
        TaggingService.TagGame(TaggingService.Tag.TerranFinisher);
    }

    /// <summary>
    /// Orders the army to kill 1 of their own to free some supply
    /// </summary>
    private void FreeSomeSupply() {
        foreach (var supervisedUnit in AttackSupervisor.SupervisedUnits.Where(unit => unit.IsBurrowed)) {
            supervisedUnit.UseAbility(Abilities.BurrowRoachUp);
        }

        var unburrowedUnits = AttackSupervisor.SupervisedUnits.Where(unit => !unit.IsBurrowed).ToList();
        if (unburrowedUnits.Count > 0) {
            var unitToSacrifice = unburrowedUnits[0];
            foreach (var unburrowedUnit in unburrowedUnits) {
                unburrowedUnit.Attack(unitToSacrifice);
            }
        }
    }

    /// <summary>
    /// Determines which region to defend next
    /// </summary>
    /// <returns>The region to defend next</returns>
    private static Region GetRegionToDefend() {
        // TODO GD We sometimes try to defend a position that's behind the enemy army while we are losing
        return RegionAnalyzer.Regions.MaxBy(region => RegionTracker.GetDefenseScore(region))!;
    }

    /// <summary>
    /// Determines which region to attack next
    /// </summary>
    /// <returns>The region to attack next</returns>
    private static Region GetRegionToAttack() {
        var valuableEnemyRegions = RegionAnalyzer.Regions
            .Where(region => RegionTracker.GetValue(region, Alliance.Enemy) > UnitEvaluator.Value.Intriguing)
            .ToList();

        if (!valuableEnemyRegions.Any()) {
            valuableEnemyRegions = RegionAnalyzer.Regions;
        }

        // TODO GD Checking only the region's force doesn't take into account that maybe we are forced to go through more enemies to get to our target.
        // TODO GD Maybe consider forces in regions adjacent to the path?
        // TODO GD Also consider the distance to the region to simulate commitment? Or only change target if significant value change
        var regionToAttack = valuableEnemyRegions
            .MaxBy(region => RegionTracker.GetValue(region, Alliance.Enemy) / RegionTracker.GetForce(region, Alliance.Enemy))!;

        return regionToAttack;
    }

    /// <summary>
    /// Determines if we should attack the given region or defend our home.
    /// </summary>
    /// <param name="regionToAttack">The region that we would attack</param>
    /// <param name="regionToDefend">The region that we would defend</param>
    private void AttackOrDefend(Region regionToAttack, Region regionToDefend) {
        if (regionToAttack.IsObstructed) {
            Logger.Error("Trying to attack an obstructed region");
            return;
        }

        if (_warManager.ManagedUnits.Count == 0) {
            // TODO GD Should we do stuff here?
            return;
        }

        var armyRegion = _warManager.ManagedUnits.GetRegion();
        if (armyRegion == null) {
            Logger.Error("AttackOrDefend could not find the region of its army of size {0}", _warManager.ManagedUnits.Count);
            return;
        }

        var pathToTarget = Pathfinder.FindPath(armyRegion, regionToAttack);
        if (pathToTarget == null) {
            Logger.Error("AttackOrDefend could not find a path from the army to its target {0} -> {1}", armyRegion, regionToAttack);
            return;
        }

        var ourForce = _warManager.ManagedUnits.GetForce();

        // TODO GD Maybe consider units near the path as well?
        var enemyForce = pathToTarget.Sum(region => RegionTracker.GetForce(region, Alliance.Enemy));

        if (ourForce > enemyForce * RequiredForceRatioBeforeAttacking || Controller.CurrentSupply >= MaxSupplyBeforeAttacking) {
            if (Stance.HasFlag(Stance.Defend)) {
                DefenseSupervisor.Retire();
            }

            Stance |= Stance.Attack;
            Stance &= ~Stance.Defend; // Unset the defend flag
            _debugger.Target = regionToAttack;
        }
        else {
            if (Stance.HasFlag(Stance.Attack)) {
                AttackSupervisor.Retire();
            }

            Stance = Stance.Defend;
            _debugger.Target = regionToDefend;
        }
    }

    /// <summary>
    /// Request more or less army based on what the WarManager has and what the enemy has and is doing.
    /// </summary>
    private void AdjustBuildRequests() {
        var unitTypeToProduce = GetUnitTypeToProduce();
        if (_armyBuildRequest.UnitOrUpgradeType != unitTypeToProduce) {
            BuildRequests.Remove(_armyBuildRequest);

            _armyBuildRequest = new TargetBuildRequest(BuildType.Train, unitTypeToProduce, targetQuantity: 100, priority: BuildRequestPriority.Low);
            BuildRequests.Add(_armyBuildRequest);
        }

        // TODO GD Consider units in production too
        var ourForce = _warManager.ManagedUnits.GetForce();
        // TODO GD Exclude buildings?
        var enemyForce = GetEnemyForce();

        if (ourForce * 1.5 < enemyForce) {
            _armyBuildRequest.BlockCondition = BuildBlockCondition.MissingMinerals | BuildBlockCondition.MissingProducer | BuildBlockCondition.MissingTech;
        }
        else {
            _armyBuildRequest.BlockCondition = BuildBlockCondition.None;
        }

        // TODO GD Try to see if they are attacking or not. If they're not, keep droning
        if (ourForce < enemyForce) {
            _armyBuildRequest.Priority = BuildRequestPriority.Medium;
        }
        else {
            _armyBuildRequest.Priority = BuildRequestPriority.Low;
        }
    }

    /// <summary>
    /// Determines the unit type to produce given the current situation.
    /// Right now, it just checks if it can make roaches, but in the future it might check against the enemy composition.
    /// </summary>
    /// <returns>The unit type id to produce.</returns>
    private static uint GetUnitTypeToProduce() {
        if (Controller.IsUnlocked(Units.Roach, TechTree.UnitPrerequisites)) {
            return Units.Roach;
        }

        if (Controller.GetUnits(UnitsTracker.OwnedUnits, Units.SpawningPool).Any()) {
            return Units.Zergling;
        }

        // TODO GD Not sure if this is good
        return Units.Drone;
    }

    /// <summary>
    /// Determines if supply should be freed to let the TerranFinisher do its job
    /// </summary>
    /// <returns></returns>
    private bool ShouldFreeSomeSupply() {
        if (!Stance.HasFlag(Stance.TerranFinisher)) {
            return false;
        }

        if (Controller.AvailableSupply >= 2) {
            return false;
        }

        return _antiTerranBuildRequest.Fulfillment.Remaining > 0;
    }

    /// <summary>
    /// Returns the enemy force
    /// </summary>
    /// <returns></returns>
    private static float GetEnemyForce() {
        return UnitsTracker.EnemyMemorizedUnits.Values.Concat(UnitsTracker.EnemyUnits).GetForce();
    }
}
