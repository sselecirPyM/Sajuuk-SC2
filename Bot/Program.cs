﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bot.Actions;
using Bot.Algorithms;
using Bot.Builds;
using Bot.Builds.BuildOrders;
using Bot.Debugging;
using Bot.Debugging.GraphicalDebugging;
using Bot.GameData;
using Bot.GameSense;
using Bot.GameSense.EnemyStrategyTracking;
using Bot.GameSense.EnemyStrategyTracking.StrategyInterpretation;
using Bot.GameSense.RegionsEvaluationsTracking;
using Bot.GameSense.RegionsEvaluationsTracking.RegionsEvaluations;
using Bot.Managers;
using Bot.Managers.EconomyManagement;
using Bot.Managers.ScoutManagement;
using Bot.Managers.ScoutManagement.ScoutingStrategies;
using Bot.Managers.ScoutManagement.ScoutingTasks;
using Bot.Managers.WarManagement;
using Bot.Managers.WarManagement.ArmySupervision;
using Bot.Managers.WarManagement.ArmySupervision.RegionalArmySupervision;
using Bot.Managers.WarManagement.ArmySupervision.UnitsControl;
using Bot.Managers.WarManagement.ArmySupervision.UnitsControl.SneakAttackUnitsControl;
using Bot.Managers.WarManagement.States;
using Bot.MapAnalysis;
using Bot.MapAnalysis.ExpandAnalysis;
using Bot.MapAnalysis.RegionAnalysis;
using Bot.Scenarios;
using Bot.Tagging;
using Bot.UnitModules;
using Bot.VideoClips;
using Bot.VideoClips.Manim.Animations;
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

    private const string MapFileName = Maps.Season_2022_4.FileNames.Berlingrad;
    private const Race OpponentRace = Race.Random;
    private const Difficulty OpponentDifficulty = Difficulty.CheatInsane;

    private const bool RealTime = false;

    public static bool DebugEnabled { get; private set; }

    public static void Main(string[] args) {
        try {
            switch (args.Length) {
                case 1 when args[0] == "--generateData":
                    PlayDataGeneration(Maps.Season_2022_4.FileNames.GetAll());
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

    private static void PlayDataGeneration(IEnumerable<string> mapFileNames) {
        Logger.Info("Game launched in data generation mode");
        DebugEnabled = true;

        foreach (var mapFileName in mapFileNames) {
            var services = CreateServices(graphicalDebugging: false, dataGeneration: true);
            // TODO GD Create a game connection for data analysis
            var botRunner = new DeprecatedBotRunner(
                services.UnitsTracker,
                services.ExpandAnalyzer,
                services.RegionAnalyzer,
                services.GraphicalDebugger,
                services.KnowledgeBase,
                services.FrameClock,
                services.Controller,
                services.RequestBuilder,
                services.Pathfinder,
                services.ActionService,
                services.Sc2Client,
                stepSize: 1
            );

            Logger.Important($"Generating data for {mapFileName}");
            botRunner.RunLocal(
                new MapAnalysisRunner(services.FrameClock),
                mapFileName,
                Race.Zerg,
                Difficulty.VeryEasy,
                realTime: false,
                runDataAnalyzersOnly: true // TODO GD Should be handled by a specialized game connection instead
            ).Wait();
        }
    }

    private static void PlayVideoClip() {
        DebugEnabled = true;

        var services = CreateServices(graphicalDebugging: true);
        var videoClipPlayer = new VideoClipPlayer(
            services.DebuggingFlagsTracker,
            services.UnitsTracker,
            services.TerrainTracker,
            services.FrameClock,
            services.RequestBuilder,
            services.Sc2Client,
            new AnimationFactory(services.TerrainTracker, services.GraphicalDebugger, services.Controller, services.RequestBuilder, services.Sc2Client),
            MapFileName
        );

        var botRunner = new LocalBotRunner(
            services.Sc2Client,
            services.RequestBuilder,
            services.KnowledgeBase,
            services.FrameClock,
            services.Controller,
            services.ActionService,
            services.GraphicalDebugger,
            services.UnitsTracker,
            services.Pathfinder,
            videoClipPlayer,
            MapFileName,
            Race.Terran,
            Difficulty.VeryEasy,
            stepSize: 1,
            realTime: true
        );

        Logger.Info("Game launched in video clip mode");
        botRunner.PlayGame().Wait();
    }

    private static void PlayLocalGame() {
        DebugEnabled = true;

        var services = CreateServices(graphicalDebugging: true);
        var botRunner = new LocalBotRunner(
            services.Sc2Client,
            services.RequestBuilder,
            services.KnowledgeBase,
            services.FrameClock,
            services.Controller,
            services.ActionService,
            services.GraphicalDebugger,
            services.UnitsTracker,
            services.Pathfinder,
            CreateSajuuk(services, Version, Scenarios),
            MapFileName,
            OpponentRace,
            OpponentDifficulty,
            stepSize: 2,
            RealTime
        );

        Logger.Info("Game launched in local play mode");
        botRunner.PlayGame().Wait();
    }

    private static void PlayLadderGame(string[] args) {
        DebugEnabled = false;

        var services = CreateServices(graphicalDebugging: false);
        var commandLineArgs = new CommandLineArguments(args);
        var botRunner = new LadderBotRunner(
            services.Sc2Client,
            services.RequestBuilder,
            services.KnowledgeBase,
            services.FrameClock,
            services.Controller,
            services.ActionService,
            services.GraphicalDebugger,
            services.UnitsTracker,
            services.Pathfinder,
            CreateSajuuk(services, Version, Scenarios),
            stepSize: 2,
            commandLineArgs.LadderServer,
            commandLineArgs.GamePort,
            commandLineArgs.StartPort
        );

        Logger.Info("Game launched in ladder play mode");
        botRunner.PlayGame().Wait();
    }

    private static IBot CreateSajuuk(Services services, string version, List<IScenario> scenarios) {
        var buildRequestFactory = new BuildRequestFactory(
            services.UnitsTracker,
            services.Controller,
            services.KnowledgeBase
        );

        var buildOrderFactory = new BuildOrderFactory(
            services.UnitsTracker,
            services.Controller,
            buildRequestFactory
        );

        var economySupervisorFactory = new EconomySupervisorFactory(
            services.UnitsTracker,
            buildRequestFactory,
            services.GraphicalDebugger,
            services.FrameClock,
            services.UnitModuleInstaller
        );

        var scoutSupervisorFactory = new ScoutSupervisorFactory(
            services.UnitsTracker
        );

        var sneakAttackStateFactory = new SneakAttackStateFactory(
            services.UnitsTracker,
            services.TerrainTracker,
            services.FrameClock,
            services.DetectionTracker,
            services.Clustering,
            services.UnitEvaluator
        );

        var unitsControlFactory = new UnitsControlFactory(
            services.UnitsTracker,
            services.TerrainTracker,
            services.GraphicalDebugger,
            services.RegionsTracker,
            services.RegionsEvaluationsTracker,
            services.FrameClock,
            services.Controller,
            services.DetectionTracker,
            services.UnitEvaluator,
            services.Clustering,
            sneakAttackStateFactory
        );

        var armySupervisorStateFactory = new ArmySupervisorStateFactory(
            services.VisibilityTracker,
            services.UnitsTracker,
            services.TerrainTracker,
            services.RegionsTracker,
            services.RegionsEvaluationsTracker,
            services.GraphicalDebugger,
            unitsControlFactory,
            services.FrameClock,
            services.Controller,
            services.UnitEvaluator,
            services.Pathfinder
        );

        var regionalArmySupervisorStateFactory = new RegionalArmySupervisorStateFactory(
            services.RegionsTracker,
            services.RegionsEvaluationsTracker,
            unitsControlFactory,
            services.UnitEvaluator,
            services.Pathfinder
        );

        var warSupervisorFactory = new WarSupervisorFactory(
            services.UnitsTracker,
            services.GraphicalDebugger,
            armySupervisorStateFactory,
            unitsControlFactory,
            regionalArmySupervisorStateFactory,
            services.Clustering,
            services.UnitEvaluator
        );

        var scoutingTaskFactory = new ScoutingTaskFactory(
            services.VisibilityTracker,
            services.UnitsTracker,
            services.TerrainTracker,
            services.GraphicalDebugger,
            services.KnowledgeBase,
            services.FrameClock
        );

        var warManagerBehaviourFactory = new WarManagerBehaviourFactory(
            services.TaggingService,
            services.DebuggingFlagsTracker,
            services.UnitsTracker,
            services.RegionsTracker,
            services.RegionsEvaluationsTracker,
            services.VisibilityTracker,
            services.TerrainTracker,
            services.EnemyRaceTracker,
            scoutSupervisorFactory,
            warSupervisorFactory,
            buildRequestFactory,
            services.GraphicalDebugger,
            scoutingTaskFactory,
            services.TechTree,
            services.Controller,
            services.FrameClock,
            services.UnitEvaluator,
            services.Pathfinder,
            services.UnitModuleInstaller
        );

        var warManagerStateFactory = new WarManagerStateFactory(
            services.UnitsTracker,
            services.TerrainTracker,
            warManagerBehaviourFactory,
            services.FrameClock,
            services.UnitEvaluator
        );

        var scoutingStrategyFactory = new ScoutingStrategyFactory(
            services.RegionsTracker,
            scoutingTaskFactory,
            services.UnitsTracker,
            services.EnemyRaceTracker,
            services.FrameClock
        );

        var managerFactory = new ManagerFactory(
            services.TaggingService,
            services.EnemyStrategyTracker,
            services.UnitsTracker,
            services.EnemyRaceTracker,
            services.TerrainTracker,
            economySupervisorFactory,
            scoutSupervisorFactory,
            warManagerStateFactory,
            buildRequestFactory,
            services.GraphicalDebugger,
            scoutingStrategyFactory,
            services.Controller,
            services.FrameClock,
            services.KnowledgeBase,
            services.SpendingTracker,
            services.UnitModuleInstaller
        );

        var botDebugger = new BotDebugger(
            services.VisibilityTracker,
            services.DebuggingFlagsTracker,
            services.UnitsTracker,
            services.IncomeTracker,
            services.TerrainTracker,
            services.EnemyStrategyTracker,
            services.EnemyRaceTracker,
            services.GraphicalDebugger,
            services.Controller,
            services.KnowledgeBase,
            services.SpendingTracker
        );

        return new SajuukBot(
            version,
            scenarios,
            services.TaggingService,
            services.UnitsTracker,
            services.TerrainTracker,
            managerFactory,
            buildRequestFactory,
            buildOrderFactory,
            botDebugger,
            services.FrameClock,
            services.Controller,
            services.SpendingTracker,
            services.ChatService
        );
    }

    private static Services CreateServices(bool graphicalDebugging, bool dataGeneration = false) {
        var knowledgeBase = new KnowledgeBase();
        var requestBuilder = new RequestBuilder(knowledgeBase);
        var sc2Client = new Sc2Client(requestBuilder);

        var frameClock = new FrameClock();
        var visibilityTracker = new VisibilityTracker(frameClock);

        var actionBuilder = new ActionBuilder(knowledgeBase);
        var actionService = new ActionService();
        var unitsTracker = new UnitsTracker(visibilityTracker);

        var prerequisiteFactory = new PrerequisiteFactory(unitsTracker);
        var techTree = new TechTree(prerequisiteFactory);

        var terrainTracker = new TerrainTracker(visibilityTracker, unitsTracker, knowledgeBase);

        IGraphicalDebugger graphicalDebugger = graphicalDebugging ? new Sc2GraphicalDebugger(terrainTracker, requestBuilder) : new NullGraphicalDebugger();

        var pathfinder = new Pathfinder(terrainTracker, graphicalDebugger);
        var clustering = new Clustering(terrainTracker, graphicalDebugger);

        var buildingTracker = new BuildingTracker(unitsTracker, terrainTracker, knowledgeBase, graphicalDebugger, requestBuilder, sc2Client);

        var chatTracker = new ChatTracker();
        var debuggingFlagsTracker = new DebuggingFlagsTracker(chatTracker);
        var regionsDataRepository = new RegionsDataRepository(terrainTracker, clustering, pathfinder);
        var expandUnitsAnalyzer = new ExpandUnitsAnalyzer(unitsTracker, terrainTracker, knowledgeBase, clustering);
        var regionsTracker = new RegionsTracker(terrainTracker, debuggingFlagsTracker, unitsTracker, regionsDataRepository, expandUnitsAnalyzer, graphicalDebugger);

        var creepTracker = new CreepTracker(visibilityTracker, unitsTracker, terrainTracker, frameClock, graphicalDebugger);
        var unitEvaluator = new UnitEvaluator(regionsTracker);
        var regionsEvaluatorFactory = new RegionsEvaluatorFactory(unitsTracker, frameClock, unitEvaluator, pathfinder);
        var regionsEvaluationsTracker = new RegionsEvaluationsTracker(debuggingFlagsTracker, terrainTracker, regionsTracker, graphicalDebugger, regionsEvaluatorFactory);

        var chatService = new ChatService(actionService, actionBuilder);
        var taggingService = new TaggingService(frameClock, chatService);
        var enemyRaceTracker = new EnemyRaceTracker(taggingService, unitsTracker);
        var strategyInterpreterFactory = new StrategyInterpreterFactory(frameClock, knowledgeBase, enemyRaceTracker, regionsTracker);
        var enemyStrategyTracker = new EnemyStrategyTracker(taggingService, unitsTracker, strategyInterpreterFactory);

        var incomeTracker = new IncomeTracker(taggingService, unitsTracker, frameClock);

        var expandAnalyzer = new ExpandAnalyzer(terrainTracker, buildingTracker, expandUnitsAnalyzer, frameClock, graphicalDebugger, clustering, pathfinder);
        var regionAnalyzer = new RegionAnalyzer(terrainTracker, expandAnalyzer, graphicalDebugger, clustering, pathfinder, regionsDataRepository);

        var spendingTracker = new SpendingTracker(incomeTracker, knowledgeBase);

        // TODO GD Compute update order. For now they are in declaration order (which is fine, but prone to errors)
        // TODO GD Kinda whack but that'll do until we get multiple controllers and game connections
        var trackers = dataGeneration
            ? new List<INeedUpdating>
            {
                frameClock,
                actionService,
                visibilityTracker,
                unitsTracker,
                terrainTracker,
                buildingTracker,
                expandAnalyzer,
                regionAnalyzer,
            }
            : new List<INeedUpdating>
            {
                frameClock,
                actionService,
                visibilityTracker,
                unitsTracker,
                terrainTracker,
                buildingTracker,
                chatTracker,
                debuggingFlagsTracker,
                regionsTracker,
                creepTracker,
                regionsEvaluationsTracker,
                enemyRaceTracker,
                enemyStrategyTracker,
                incomeTracker,
            };

        var controller = new Controller(
            unitsTracker,
            buildingTracker,
            terrainTracker,
            regionsTracker,
            techTree,
            knowledgeBase,
            pathfinder,
            chatService,
            trackers
        );

        var detectionTracker = new DetectionTracker(unitsTracker, controller, knowledgeBase);
        var unitModuleInstaller = new UnitModuleInstaller(unitsTracker, graphicalDebugger, buildingTracker, regionsTracker, creepTracker, pathfinder, visibilityTracker, terrainTracker, frameClock);

        // We do this to avoid circular dependencies between unit, unitsTracker, terrainTracker and regionsTracker
        // I don't 100% like it but it seems worth it.
        var unitsFactory = new UnitFactory(frameClock, knowledgeBase, actionBuilder, actionService, terrainTracker, regionsTracker, unitsTracker);
        unitsTracker.WithUnitsFactory(unitsFactory);

        // Logger is not yet an instance
        Logger.SetFrameClock(frameClock);

        return new Services
        {
            FrameClock = frameClock,
            VisibilityTracker = visibilityTracker,
            UnitsTracker = unitsTracker,
            TechTree = techTree,
            KnowledgeBase = knowledgeBase,
            TerrainTracker= terrainTracker,
            BuildingTracker = buildingTracker,
            ChatTracker = chatTracker,
            DebuggingFlagsTracker = debuggingFlagsTracker,
            RegionsTracker = regionsTracker,
            CreepTracker = creepTracker,
            GraphicalDebugger = graphicalDebugger,
            RegionsEvaluationsTracker = regionsEvaluationsTracker,
            TaggingService = taggingService,
            EnemyRaceTracker = enemyRaceTracker,
            EnemyStrategyTracker = enemyStrategyTracker,
            IncomeTracker = incomeTracker,
            RequestBuilder = requestBuilder,
            Controller = controller,
            SpendingTracker = spendingTracker,
            Clustering = clustering,
            Pathfinder = pathfinder,
            UnitEvaluator = unitEvaluator,
            DetectionTracker = detectionTracker,
            ChatService = chatService,
            ActionService = actionService,
            Sc2Client = sc2Client,
            UnitModuleInstaller = unitModuleInstaller,
            ExpandAnalyzer = expandAnalyzer, // TODO GD These should not be here when not running in analysis mode, needs a different BotRunner implementation
            RegionAnalyzer = regionAnalyzer, // TODO GD These should not be here when not running in analysis mode, needs a different BotRunner implementation
        };
    }

    private readonly struct Services {
        public IFrameClock FrameClock { get; init; }
        public IVisibilityTracker VisibilityTracker { get; init; }
        public IUnitsTracker UnitsTracker { get; init; }
        public TechTree TechTree { get; init; } // TODO GD Needs interface
        public KnowledgeBase KnowledgeBase { get; init; } // TODO GD Needs interface
        public ITerrainTracker TerrainTracker { get; init; }
        public IBuildingTracker BuildingTracker { get; init; }
        public IChatTracker ChatTracker { get; init; }
        public IDebuggingFlagsTracker DebuggingFlagsTracker { get; init; }
        public IRegionsTracker RegionsTracker { get; init; }
        public ICreepTracker CreepTracker { get; init; }
        public IGraphicalDebugger GraphicalDebugger { get; init; }
        public IRegionsEvaluationsTracker RegionsEvaluationsTracker { get; init; }
        public ITaggingService TaggingService { get; init; }
        public IEnemyRaceTracker EnemyRaceTracker { get; init; }
        public IEnemyStrategyTracker EnemyStrategyTracker { get; init; }
        public IIncomeTracker IncomeTracker { get; init; }
        public IRequestBuilder RequestBuilder { get; init; }
        public IController Controller { get; init; }
        public ISpendingTracker SpendingTracker { get; init; }
        public IClustering Clustering { get; init; }
        public IPathfinder Pathfinder { get; init; }
        public IUnitEvaluator UnitEvaluator { get; init; }
        public IDetectionTracker DetectionTracker { get; init; }
        public IChatService ChatService { get; init; }
        public IActionService ActionService { get; init; }
        public ISc2Client Sc2Client { get; init; }
        public IUnitModuleInstaller UnitModuleInstaller { get; init; }

        public IExpandAnalyzer ExpandAnalyzer { get; init; }
        public IRegionAnalyzer RegionAnalyzer { get; init; }
    }
}
