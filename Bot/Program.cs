﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bot.Algorithms;
using Bot.Builds;
using Bot.Builds.BuildOrders;
using Bot.Debugging;
using Bot.Debugging.GraphicalDebugging;
using Bot.GameSense;
using Bot.GameSense.EnemyStrategyTracking;
using Bot.GameSense.RegionsEvaluationsTracking;
using Bot.Managers;
using Bot.Managers.EconomyManagement;
using Bot.Managers.ScoutManagement;
using Bot.Managers.ScoutManagement.ScoutingStrategies;
using Bot.Managers.ScoutManagement.ScoutingTasks;
using Bot.Managers.WarManagement;
using Bot.Managers.WarManagement.ArmySupervision;
using Bot.Managers.WarManagement.States;
using Bot.MapAnalysis;
using Bot.MapAnalysis.ExpandAnalysis;
using Bot.MapAnalysis.RegionAnalysis;
using Bot.Scenarios;
using Bot.Tagging;
using Bot.VideoClips;
using Bot.Wrapper;
using SC2APIProtocol;

namespace Bot;

[ExcludeFromCodeCoverage]
public class Program {
    private static readonly List<IScenario> Scenarios = new List<IScenario>
    {
        //new WorkerRushScenario(MapAnalyzer.Instance),
        //new FlyingTerranScumScenario(MapAnalyzer.Instance),
        //new SpawnStuffScenario(UnitsTracker.Instance, MapAnalyzer.Instance),
    };

    private const string Version = "4_0_4";

    public static string MapFileName = Maps.Season_2022_4.FileNames.Berlingrad;
    private const Race OpponentRace = Race.Random;
    private const Difficulty OpponentDifficulty = Difficulty.CheatInsane;

    private const bool RealTime = false;

    public static GameConnection GameConnection { get; private set; }
    public static bool DebugEnabled { get; private set; }

    public static void Main(string[] args) {
        try {
            switch (args.Length) {
                case 1 when args[0] == "--generateData":
                    PlayDataGeneration();
                    break;
                case 1 when args[0] == "--videoClip":
                    PlayVideoClip();
                    break;
                case 0:
                    PlayLocalGame();
                    break;
                default:
                    PlayLadderGame(args);
                    break;
            }
        }
        catch (Exception ex) {
            Logger.Error(ex.ToString());
        }

        Logger.Info("Terminated.");
    }

    private static void PlayDataGeneration() {
        Logger.Info("Game launched in data generation mode");
        DebugEnabled = true;

        // TODO GD Kinda whack but won't be needed once DI is finished
        var getThoseWhoNeedUpdating = () => new List<INeedUpdating>
        {
            VisibilityTracker.Instance, // DI: ✔️ Depends on nothing

            UnitsTracker.Instance,      // DI: ✔️ Depends on VisibilityTracker
            TerrainTracker.Instance,    // DI: ✔️ Depends on VisibilityTracker and UnitsTracker

            BuildingTracker.Instance,   // DI: ✔️ Depends on UnitsTracker and TerrainTracker

            ExpandAnalyzer.Instance,    // DI: ✔️ Depends on UnitsTracker, TerrainTracker and BuildingTracker
            RegionAnalyzer.Instance,    // DI: ✔️ Depends on TerrainTracker and ExpandAnalyzer
        };

        foreach (var mapFileName in Maps.Season_2022_4.FileNames.GetAll()) {
            MapFileName = mapFileName;

            // Ensure clean state
            Controller.Reset();
            getThoseWhoNeedUpdating().ForEach(needsUpdating => needsUpdating.Reset()); // We need to reset so that the analyzers get the right file name

            // Those are not yet injected
            Pathfinder.Instance.Reset();
            Clustering.Instance.Reset();

            GraphicalDebugger.Instance = new Sc2GraphicalDebugger(TerrainTracker.Instance);

            // TODO GD Instead of doing this, we should be able to use a different Controller and another GameConnection
            // DI Should create new instances for each run
            Controller.ThoseWhoNeedUpdating = getThoseWhoNeedUpdating();

            Logger.Important($"Generating data for {mapFileName}");
            GameConnection = CreateGameConnection();
            GameConnection.RunLocal(new MapAnalysisRunner(() => Controller.Frame), mapFileName, Race.Zerg, Difficulty.VeryEasy, realTime: false, runDataAnalyzersOnly: true).Wait();
        }
    }

    private static void PlayVideoClip() {
        Logger.Info("Game launched in video clip mode");

        DebugEnabled = true;
        GraphicalDebugger.Instance = new Sc2GraphicalDebugger(TerrainTracker.Instance);

        GameConnection = CreateGameConnection(stepSize: 1);
        GameConnection.RunLocal(new VideoClipPlayer(MapFileName, DebuggingFlagsTracker.Instance, UnitsTracker.Instance, TerrainTracker.Instance, GraphicalDebugger.Instance), MapFileName, Race.Terran, Difficulty.VeryEasy, realTime: true).Wait();
    }

    private static void PlayLocalGame() {
        DebugEnabled = true;
        GraphicalDebugger.Instance = new Sc2GraphicalDebugger(TerrainTracker.Instance);

        GameConnection = CreateGameConnection();
        GameConnection.RunLocal(CreateSajuuk(Version, Scenarios), MapFileName, OpponentRace, OpponentDifficulty, RealTime).Wait();
    }

    private static void PlayLadderGame(string[] args) {
        DebugEnabled = false;
        // TODO GD That doesn't work because static trackers will get an undefined reference
        GraphicalDebugger.Instance = new NullGraphicalDebugger();

        GameConnection = CreateGameConnection();
        GameConnection.RunLadder(CreateSajuuk(Version, Scenarios), args).Wait();
    }

    private static IBot CreateSajuuk(string version, List<IScenario> scenarios) {
        var buildRequestFactory = new BuildRequestFactory(UnitsTracker.Instance);

        var buildOrderFactory = new BuildOrderFactory(UnitsTracker.Instance, buildRequestFactory);

        var economySupervisorFactory = new EconomySupervisorFactory(
            UnitsTracker.Instance,
            BuildingTracker.Instance,
            RegionsTracker.Instance,
            CreepTracker.Instance,
            buildRequestFactory,
            GraphicalDebugger.Instance
        );

        var scoutSupervisorFactory = new ScoutSupervisorFactory(
            UnitsTracker.Instance
        );

        var armySupervisorStateFactory = new ArmySupervisorStateFactory(
            VisibilityTracker.Instance,
            UnitsTracker.Instance,
            TerrainTracker.Instance,
            RegionsTracker.Instance,
            RegionsEvaluationsTracker.Instance,
            GraphicalDebugger.Instance
        );

        var warSupervisorFactory = new WarSupervisorFactory(
            UnitsTracker.Instance,
            TerrainTracker.Instance,
            RegionsTracker.Instance,
            RegionsEvaluationsTracker.Instance,
            GraphicalDebugger.Instance,
            armySupervisorStateFactory
        );

        var scoutingTaskFactory = new ScoutingTaskFactory(
            VisibilityTracker.Instance,
            UnitsTracker.Instance,
            TerrainTracker.Instance,
            GraphicalDebugger.Instance
        );

        var warManagerBehaviourFactory = new WarManagerBehaviourFactory(
            TaggingService.Instance,
            DebuggingFlagsTracker.Instance,
            UnitsTracker.Instance,
            RegionsTracker.Instance,
            RegionsEvaluationsTracker.Instance,
            VisibilityTracker.Instance,
            TerrainTracker.Instance,
            EnemyRaceTracker.Instance,
            scoutSupervisorFactory,
            warSupervisorFactory,
            buildRequestFactory,
            GraphicalDebugger.Instance,
            scoutingTaskFactory
        );

        var warManagerStateFactory = new WarManagerStateFactory(
            UnitsTracker.Instance,
            TerrainTracker.Instance,
            warManagerBehaviourFactory
        );

        var scoutingStrategyFactory = new ScoutingStrategyFactory(
            RegionsTracker.Instance,
            scoutingTaskFactory,
            UnitsTracker.Instance,
            EnemyRaceTracker.Instance
        );

        var managerFactory = new ManagerFactory(
            TaggingService.Instance,
            EnemyStrategyTracker.Instance,
            UnitsTracker.Instance,
            EnemyRaceTracker.Instance,
            VisibilityTracker.Instance,
            TerrainTracker.Instance,
            RegionsTracker.Instance,
            BuildingTracker.Instance,
            CreepTracker.Instance,
            economySupervisorFactory,
            scoutSupervisorFactory,
            warManagerStateFactory,
            buildRequestFactory,
            GraphicalDebugger.Instance,
            scoutingStrategyFactory
        );

        var botDebugger = new BotDebugger(
            VisibilityTracker.Instance,
            DebuggingFlagsTracker.Instance,
            UnitsTracker.Instance,
            IncomeTracker.Instance,
            TerrainTracker.Instance,
            EnemyStrategyTracker.Instance,
            EnemyRaceTracker.Instance,
            GraphicalDebugger.Instance
        );

        return new SajuukBot(
            version,
            scenarios,
            TaggingService.Instance,
            UnitsTracker.Instance,
            TerrainTracker.Instance,
            managerFactory,
            buildRequestFactory,
            buildOrderFactory,
            botDebugger
        );
    }

    private static GameConnection CreateGameConnection(uint stepSize = 2) {
        return new GameConnection(
            UnitsTracker.Instance,
            ExpandAnalyzer.Instance,
            RegionAnalyzer.Instance,
            GraphicalDebugger.Instance,
            stepSize
        );
    }
}
