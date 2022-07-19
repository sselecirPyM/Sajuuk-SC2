﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Bot.GameData;
using Bot.Wrapper;

namespace Bot;

public static class Pathfinder {
    private static List<List<float>> _heightMap;
    private static List<List<bool>> _walkMap;
    private static int _maxX;
    private static int _maxY;

    public static bool IsInitialized = false;

    public static void Init() {
        if (IsInitialized) {
            return;
        }

        _maxX = Controller.GameInfo.StartRaw.MapSize.X;
        _maxY = Controller.GameInfo.StartRaw.MapSize.Y;

        InitHeightMap();
        InitWalkMap();

        IsInitialized = true;
    }

    private static void InitHeightMap() {
        _heightMap = new List<List<float>>();
        for (var x = 0; x < _maxX; x++) {
            _heightMap.Add(new List<float>(new float[_maxY]));
        }

        var heightVector = Controller.GameInfo.StartRaw.TerrainHeight.Data
            .ToByteArray()
            .Select(ByteToFloat)
            .ToList();

        for (var x = 0; x < _maxX; x++) {
            for (var y = 0; y < _maxY; y++) {
                _heightMap[x][y] = heightVector[y * _maxX + x]; // heightVector[4] is (4, 0)
            }
        }
    }

    private static float ByteToFloat(byte byteValue) {
        // Computed from 3 unit positions and 3 height map bytes
        // Seems to work fine
        return 0.125f * byteValue - 15.888f;
    }

    private static void InitWalkMap() {
        _walkMap = new List<List<bool>>();
        for (var x = 0; x < _maxX; x++) {
            _walkMap.Add(new List<bool>(new bool[_maxY]));
        }

        var walkVector = Controller.GameInfo.StartRaw.PathingGrid.Data
            .ToByteArray()
            .SelectMany(ByteToBoolArray)
            .ToList();

        for (var x = 0; x < _maxX; x++) {
            for (var y = 0; y < _maxY; y++) {
                _walkMap[x][y] = walkVector[y * _maxX + x]; // walkVector[4] is (4, 0)
            }
        }

        // The walk data makes cells occupied by buildings impassable
        // However, if I want to find a path from my hatch to the enemy, the pathfinding will fail because the hatchery is impassable
        // Lucky for us, when we init the walk map, there's only 1 building so we'll make its cells walkable
        var startingTownHall = Controller.GetUnits(Controller.OwnedUnits, Units.Hatchery).First();
        var townHallCells = MapAnalyzer.BuildSearchGrid(startingTownHall.Position, Buildings.GetRadius(startingTownHall.UnitType));

        foreach (var cell in townHallCells) {
            _walkMap[(int)cell.X][(int)cell.Y] = true;
        }
    }

    private static bool[] ByteToBoolArray(byte byteValue)
    {
        // Each byte represents 8 grid cells
        var values = new bool[8];

        values[7] = (byteValue & 1) != 0;
        values[6] = (byteValue & 2) != 0;
        values[5] = (byteValue & 4) != 0;
        values[4] = (byteValue & 8) != 0;
        values[3] = (byteValue & 16) != 0;
        values[2] = (byteValue & 32) != 0;
        values[1] = (byteValue & 64) != 0;
        values[0] = (byteValue & 128) != 0;

        return values;
    }

    public static List<Vector3> FindPath(Vector3 origin, Vector3 destination) {
        var path = AStar(
                MapAnalyzer.AsWorldGridCorner(origin.X, origin.Y),
                MapAnalyzer.AsWorldGridCorner(destination.X, destination.Y),
                (from, to) => from.HorizontalDistance(to))
            .Select(MapAnalyzer.AsWorldGridCenter)
            .ToList();

        GraphicalDebugger.AddSphere(origin, 3, Colors.Cyan);
        GraphicalDebugger.AddSphere(destination, 3, Colors.DarkBlue);
        for (var i = 0; i < path.Count; i++) {
            GraphicalDebugger.AddSquare(path[i], MapAnalyzer.GameGridCellWidth, Colors.Gradient(Colors.Cyan, Colors.DarkBlue, (float)i / path.Count));
        }

        return path;
    }

    private static IEnumerable<Vector3> AStar(Vector3 origin, Vector3 destination, Func<Vector3, Vector3, float> getHeuristicCost) {
        var cameFrom = new Dictionary<Vector3, Vector3>();

        var gScore = new Dictionary<Vector3, float>
        {
            [origin] = 0,
        };

        var fScore = new Dictionary<Vector3, float>
        {
            [origin] = getHeuristicCost(origin, destination),
        };

        var openSetContents = new HashSet<Vector3>
        {
            origin,
        };
        var openSet = new PriorityQueue<Vector3, float>();
        openSet.Enqueue(origin, fScore[origin]);

        while (openSet.Count > 0) {
            var current = openSet.Dequeue();
            openSetContents.Remove(current);

            if (current == destination) {
                return BuildPath(cameFrom, current);
            }

            foreach (var neighbor in GetNeighbors(current).Where(IsInBounds).Where(IsWalkable)) {
                var neighborGScore = gScore[current] + getHeuristicCost(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || neighborGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = neighborGScore;
                    fScore[neighbor] = neighborGScore + getHeuristicCost(current, destination);

                    if (!openSetContents.Contains(neighbor)) {
                        openSetContents.Add(neighbor);
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return null;
    }

    private static IEnumerable<Vector3> GetNeighbors(Vector3 position) {
        yield return position.Translate(xTranslation: -1, yTranslation: -1);
        yield return position.Translate(xTranslation: -1);
        yield return position.Translate(xTranslation: -1, yTranslation: 1);

        yield return position.Translate(yTranslation: -1);
        yield return position.Translate(yTranslation: 1);

        yield return position.Translate(xTranslation: 1, yTranslation: -1);
        yield return position.Translate(xTranslation: 1);
        yield return position.Translate(xTranslation: 1, yTranslation: 1);
    }

    private static bool IsInBounds(Vector3 position) {
        return position.X < _maxX && position.X >= 0 && position.Y < _maxY && position.Y >= 0;
    }

    private static bool IsWalkable(Vector3 position) {
        return _walkMap[(int)position.X][(int)position.Y];
    }

    private static IEnumerable<Vector3> BuildPath(IReadOnlyDictionary<Vector3, Vector3> cameFrom, Vector3 end) {
        var current = end;
        var path = new List<Vector3> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();

        return path.Select(WithWorldHeight);
    }

    private static Vector3 WithWorldHeight(Vector3 position) {
        return new Vector3
        {
            X = position.X,
            Y = position.Y,
            Z = _heightMap[(int)position.X][(int)position.Y],
        };
    }
}
