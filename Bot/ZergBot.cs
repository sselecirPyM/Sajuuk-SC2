﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Managers;
using Bot.Wrapper;
using SC2APIProtocol;

namespace Bot;

using BuildOrder = Queue<BuildOrders.BuildStep>;

public class ZergBot: PoliteBot {
    private static readonly BuildOrder BuildOrder = BuildOrders.TestGasMining();
    private readonly List<IManager> _managers = new List<IManager>();

    public override string Name => "ZergBot";

    public override Race Race => Race.Zerg;

    protected override async Task DoOnFrame() {
        // TODO GD Replace this with some init code?
        if (Controller.Frame == 0) {
            _managers.Add(new EconomyManager());
        }

        // Some units died
        Controller.DeadOwnedUnits.ForEach(unit => unit.Died());

        await FollowBuildOrder();
        if (!IsBuildOrderBlocking()) {
            SpawnDrones();
        }

        // Execute managers
        _managers.ForEach(manager => manager.OnFrame());
    }

    private async Task FollowBuildOrder() {
        if (BuildOrder.Count == 0) {
            return;
        }

        while(BuildOrder.Count > 0) {
            var buildStep = BuildOrder.Peek();
            if (Controller.CurrentSupply < buildStep.AtSupply || !Controller.ExecuteBuildStep(buildStep)) {
                break;
            }

            buildStep.Quantity -= 1;
            if (BuildOrder.Peek().Quantity == 0) {
                BuildOrder.Dequeue();
            }
        }

        await PrintBuildOrderDebugInfo();
    }

    private async Task PrintBuildOrderDebugInfo() {
        var nextBuildStepsData = BuildOrder
            .Take(3)
            .Select(nextBuildStep => {
                var buildStepUnitOrUpgradeName = nextBuildStep.BuildType == BuildType.Research
                    ? GameData.GetUpgradeData(nextBuildStep.UnitOrUpgradeType).Name
                    : $"{nextBuildStep.Quantity} {GameData.GetUnitTypeData(nextBuildStep.UnitOrUpgradeType).Name}";

                return $"{nextBuildStep.BuildType.ToString()} {buildStepUnitOrUpgradeName} at {nextBuildStep.AtSupply} supply";
            })
            .ToList();

        nextBuildStepsData.Insert(0, "Next 3 builds:");

        await Debugger.ShowDebugText(nextBuildStepsData);
    }

    private bool IsBuildOrderBlocking() {
        // TODO GD Replace this by a 'Controller.ReserveMinerals' and 'Controller.ReserveGas' method
        // TODO GD Allow other stuff to happen if we have to wait for tech or something
        return BuildOrder.Count > 0 && Controller.CurrentSupply >= BuildOrder.Peek().AtSupply;
    }

    private void SpawnDrones() {
        while (Controller.TrainUnit(Units.Drone)) {}
    }
}
