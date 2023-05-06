﻿using Bot.Builds;
using Bot.Debugging.GraphicalDebugging;
using Bot.GameSense;
using Bot.GameSense.EnemyStrategyTracking;
using Bot.Managers.EconomyManagement;
using Bot.Managers.ScoutManagement;
using Bot.Managers.ScoutManagement.ScoutingTasks;
using Bot.Managers.WarManagement;
using Bot.Managers.WarManagement.States;
using Bot.Tagging;

namespace Bot.Managers;

public class ManagerFactory : IManagerFactory {
    private readonly ITaggingService _taggingService;
    private readonly IEnemyStrategyTracker _enemyStrategyTracker;
    private readonly IUnitsTracker _unitsTracker;
    private readonly IEnemyRaceTracker _enemyRaceTracker;
    private readonly IVisibilityTracker _visibilityTracker;
    private readonly ITerrainTracker _terrainTracker;
    private readonly IRegionsTracker _regionsTracker;
    private readonly IBuildingTracker _buildingTracker;
    private readonly ICreepTracker _creepTracker;
    private readonly IEconomySupervisorFactory _economySupervisorFactory;
    private readonly IScoutSupervisorFactory _scoutSupervisorFactory;
    private readonly IWarManagerStateFactory _warManagerStateFactory;
    private readonly IBuildRequestFactory _buildRequestFactory;
    private readonly IGraphicalDebugger _graphicalDebugger;
    private readonly IScoutingTaskFactory _scoutingTaskFactory;

    public ManagerFactory(
        ITaggingService taggingService,
        IEnemyStrategyTracker enemyStrategyTracker,
        IUnitsTracker unitsTracker,
        IEnemyRaceTracker enemyRaceTracker,
        IVisibilityTracker visibilityTracker,
        ITerrainTracker terrainTracker,
        IRegionsTracker regionsTracker,
        IBuildingTracker buildingTracker,
        ICreepTracker creepTracker,
        IEconomySupervisorFactory economySupervisorFactory,
        IScoutSupervisorFactory scoutSupervisorFactory,
        IWarManagerStateFactory warManagerStateFactory,
        IBuildRequestFactory buildRequestFactory,
        IGraphicalDebugger graphicalDebugger,
        IScoutingTaskFactory scoutingTaskFactory
    ) {
        _taggingService = taggingService;
        _enemyStrategyTracker = enemyStrategyTracker;
        _unitsTracker = unitsTracker;
        _enemyRaceTracker = enemyRaceTracker;
        _visibilityTracker = visibilityTracker;
        _terrainTracker = terrainTracker;
        _regionsTracker = regionsTracker;
        _buildingTracker = buildingTracker;
        _creepTracker = creepTracker;
        _economySupervisorFactory = economySupervisorFactory;
        _scoutSupervisorFactory = scoutSupervisorFactory;
        _warManagerStateFactory = warManagerStateFactory;
        _buildRequestFactory = buildRequestFactory;
        _graphicalDebugger = graphicalDebugger;
        _scoutingTaskFactory = scoutingTaskFactory;
    }

    public BuildManager CreateBuildManager(IBuildOrder buildOrder) {
        return new BuildManager(buildOrder, _taggingService, _enemyStrategyTracker);
    }

    public SupplyManager CreateSupplyManager(BuildManager buildManager) {
        return new SupplyManager(buildManager, _unitsTracker, _buildRequestFactory);
    }

    public ScoutManager CreateScoutManager() {
        return new ScoutManager(_enemyRaceTracker, _visibilityTracker, _unitsTracker, _terrainTracker, _regionsTracker, _scoutSupervisorFactory, _graphicalDebugger, _scoutingTaskFactory);
    }

    public EconomyManager CreateEconomyManager(BuildManager buildManager) {
        return new EconomyManager(buildManager, _unitsTracker, _terrainTracker, _buildingTracker, _regionsTracker, _creepTracker, _economySupervisorFactory, _buildRequestFactory, _graphicalDebugger);
    }

    public WarManager CreateWarManager() {
        return new WarManager(_warManagerStateFactory, _graphicalDebugger);
    }

    public CreepManager CreateCreepManager() {
        return new CreepManager(_visibilityTracker, _unitsTracker, _terrainTracker, _buildingTracker, _regionsTracker, _creepTracker, _graphicalDebugger);
    }

    public UpgradesManager CreateUpgradesManager() {
        return new UpgradesManager(_unitsTracker, _buildRequestFactory);
    }
}
