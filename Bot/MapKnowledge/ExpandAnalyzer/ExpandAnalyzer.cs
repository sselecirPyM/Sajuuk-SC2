﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Bot.ExtensionMethods;
using Bot.GameData;
using Bot.GameSense;
using Bot.Wrapper;
using SC2APIProtocol;

namespace Bot.MapKnowledge;

public partial class ExpandAnalyzer: INeedUpdating {
    public static readonly ExpandAnalyzer Instance = new ExpandAnalyzer();

    public static bool IsInitialized = false;

    public static List<List<Unit>> ResourceClusters;
    public static List<Vector3> ExpandLocations;

    private const int ExpandSearchRadius = 5;
    private const int ExpandRadius = 3; // It's 2.5, we put 3 to be safe

    private ExpandAnalyzer() {}

    public void Update(ResponseObservation observation) {
        if (IsInitialized) {
            return;
        }

        if (_precomputedExpandLocations.ContainsKey(Controller.GameInfo.MapName)) {
            Logger.Info("Initializing ExpandAnalyzer from precomputed locations for {0}", Controller.GameInfo.MapName);
            ExpandLocations = _precomputedExpandLocations[Controller.GameInfo.MapName];

            // This isn't expensive
            ResourceClusters = FindResourceClusters().ToList();

            IsInitialized = true;
            return;
        }

        // For some reason it doesn't work before a few seconds after the game starts
        // Also, this might take a couple of frames, so we let the bot start the game
        if (Controller.Frame < Controller.SecsToFrames(5)) {
            return;
        }

        Logger.Info("Analyzing expand locations, this can take some time...");

        ResourceClusters = FindResourceClusters().ToList();
        Logger.Info("ExpandAnalyzer found {0} resource clusters", ResourceClusters.Count);

        ExpandLocations = FindExpandLocations().ToList();
        ExpandLocations.Add(MapAnalyzer.StartingLocation); // Not found because already built on (by us)
        Logger.Info("Found {0} expand locations", ExpandLocations.Count);

        Logger.Info("You should add the following to _preComputedExpandLocations");
        Logger.Info("Map: {0}", Controller.GameInfo.MapName);
        foreach (var expandLocation in ExpandLocations) {
            Logger.Info("new Vector3({0}f, {1}f, {2}f),", expandLocation.X, expandLocation.Y, expandLocation.Z);
        }

        IsInitialized = ExpandLocations.Count == ResourceClusters.Count;

        Logger.Info("{0}", IsInitialized ? "Success!" : "Failed...");
    }

    // TODO GD Doesn't take into account the building dimensions, but good enough for creep spread since it's 1x1
    public static bool IsNotBlockingExpand(Vector3 position) {
        return ExpandLocations.MinBy(expandLocation => expandLocation.HorizontalDistanceTo(position)).HorizontalDistanceTo(position) > ExpandRadius + 1;
    }

    private static IEnumerable<List<Unit>> FindResourceClusters() {
        var minerals = Controller.GetUnits(UnitsTracker.NeutralUnits, Units.MineralFields);
        var gasses = Controller.GetUnits(UnitsTracker.NeutralUnits, Units.GasGeysers);
        var resources = minerals.Concat(gasses).ToList();

        return Clustering.DBSCAN(resources, epsilon: 7, minPoints: 4).clusters;
    }

    private static IEnumerable<Vector3> FindExpandLocations() {
        var expandLocations = new List<Vector3>();

        foreach (var resourceCluster in ResourceClusters) {
            var centerPosition = Clustering.GetBoundingBoxCenter(resourceCluster).AsWorldGridCenter();
            var searchGrid = MapAnalyzer.BuildSearchGrid(centerPosition, gridRadius: ExpandSearchRadius);

            var goodBuildSpot = searchGrid.FirstOrDefault(buildSpot => BuildingTracker.CanPlace(Units.Hatchery, buildSpot));
            if (goodBuildSpot != default) {
                expandLocations.Add(goodBuildSpot);
                Program.GraphicalDebugger.AddSphere(goodBuildSpot, KnowledgeBase.GameGridCellRadius, Colors.Green);
                Program.GraphicalDebugger.AddSphere(centerPosition, KnowledgeBase.GameGridCellRadius, Colors.Yellow);
            }
        }

        return expandLocations;
    }
}
