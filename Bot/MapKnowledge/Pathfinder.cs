﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Bot.Debugging.GraphicalDebugging;
using Bot.ExtensionMethods;

namespace Bot.MapKnowledge;

using CellPath = List<Vector2>;
using RegionPath = List<Region>;

public static class Pathfinder {
    private const bool DrawEnabled = false; // TODO GD Flag this

    /// <summary>
    /// This is public for the performance debugging report, please don't rely on this
    /// </summary>
    public static readonly Dictionary<Vector2, Dictionary<Vector2, CellPath>> CellPathsMemory = new Dictionary<Vector2, Dictionary<Vector2, CellPath>>();

    private static readonly Dictionary<Region, Dictionary<Region, RegionPath>> RegionPathsMemory = new Dictionary<Region, Dictionary<Region, RegionPath>>();

    /// <summary>
    /// <para>Finds a path between the origin and destination.</para>
    /// <para>The pathing considers rocks but not buildings or units.</para>
    /// <para>The results are cached so subsequent calls with the same origin and destinations are free.</para>
    /// </summary>
    /// <param name="origin">The origin position.</param>
    /// <param name="destination">The destination position.</param>
    /// <returns>The requested path, or null if the destination is unreachable from the origin.</returns>
    public static CellPath FindPath(Vector2 origin, Vector2 destination) {
        // Improve caching performance
        origin = origin.ClosestWalkable().AsWorldGridCorner();
        destination = destination.ClosestWalkable().AsWorldGridCorner();

        if (origin == destination) {
            return new CellPath();
        }

        var isPathKnown = TryGetPathFromMemory(origin, destination, CellPathsMemory, out var knownPath);
        if (isPathKnown) {
            DebugPath(knownPath, isKnown: true);
            return knownPath;
        }

        var maybeNullPath = AStar(origin, destination, (from, to) => from.DistanceTo(to), current => current.GetReachableNeighbors());
        if (maybeNullPath == null) {
            Logger.Info("No path found between {0} and {1}", origin, destination);
            SavePathToMemory(origin, destination, CellPathsMemory, null);
            return null;
        }

        var path = maybeNullPath.Select(step => step.AsWorldGridCenter()).ToList();
        DebugPath(path, isKnown: false);

        SavePathToMemory(origin, destination, CellPathsMemory, path);

        return path;
    }

    /// <summary>
    /// <para>Finds a path between the origin region and the destination region.</para>
    /// <para>The pathing considers rocks but not buildings or units.</para>
    /// <para>The results are cached so subsequent calls with the same origin and destinations are free.</para>
    /// </summary>
    /// <param name="origin">The origin region.</param>
    /// <param name="destination">The destination region.</param>
    /// <returns>The requested path, or null if the destination is unreachable from the origin.</returns>
    public static RegionPath FindPath(Region origin, Region destination) {
        if (origin == destination) {
            return new RegionPath();
        }

        var isPathKnown = TryGetPathFromMemory(origin, destination, RegionPathsMemory, out var knownPath);
        if (isPathKnown) {
            return knownPath;
        }

        var maybeNullPath = AStar(
            origin,
            destination,
            (from, to) => from.Center.DistanceTo(to.Center),
            current => current.Neighbors.Select(neighbor => neighbor.Region).Where(neighbor => !neighbor.IsObstructed)
        );

        if (maybeNullPath == null) {
            Logger.Info("No path found between {0} and {1}", origin, destination);
            SavePathToMemory(origin, destination, RegionPathsMemory, null);
            return null;
        }

        SavePathToMemory(origin, destination, RegionPathsMemory, maybeNullPath);

        return maybeNullPath;
    }

    public static void InvalidateCache() {
        CellPathsMemory.Clear();
        RegionPathsMemory.Clear();
    }

    /// <summary>
    /// Draws a path in green if it is known, or in blue if it has just been calculated.
    /// Nothing is drawn if drawing is not enabled.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isKnown"></param>
    private static void DebugPath(List<Vector2> path, bool isKnown) {
        if (!DrawEnabled) {
            return;
        }

        if (isKnown) {
            Program.GraphicalDebugger.AddPath(path, Colors.LightGreen, Colors.DarkGreen);
        }
        else {
            Program.GraphicalDebugger.AddPath(path, Colors.LightBlue, Colors.DarkBlue);
        }
    }

    /// <summary>
    /// <para>A textbook implementation of the A* search algorithm.</para>
    /// <para>See https://en.wikipedia.org/wiki/A*_search_algorithm</para>
    /// </summary>
    /// <param name="origin">The origin position.</param>
    /// <param name="destination">The destination position.</param>
    /// <param name="getEdgeLength">A function that computes the distance between two vertex.</param>
    /// <param name="getVertexNeighbors">A function that returns the neighbors of a vertex.</param>
    /// <returns>The requested path, or null if the destination is unreachable from the origin.</returns>
    private static List<TVertex> AStar<TVertex>(
        TVertex origin,
        TVertex destination,
        Func<TVertex, TVertex, float> getEdgeLength,
        Func<TVertex, IEnumerable<TVertex>> getVertexNeighbors
    ) {
        var cameFrom = new Dictionary<TVertex, TVertex>();

        var gScore = new Dictionary<TVertex, float>
        {
            [origin] = 0,
        };

        var fScore = new Dictionary<TVertex, float>
        {
            [origin] = getEdgeLength(origin, destination),
        };

        var openSetContents = new HashSet<TVertex>
        {
            origin,
        };
        var openSet = new PriorityQueue<TVertex, float>();
        openSet.Enqueue(origin, fScore[origin]);

        while (openSet.Count > 0) {
            var current = openSet.Dequeue();
            openSetContents.Remove(current);

            if (EqualityComparer<TVertex>.Default.Equals(current, destination)) {
                return BuildPath(cameFrom, current);
            }

            foreach (var neighbor in getVertexNeighbors(current)) {
                var neighborGScore = gScore[current] + getEdgeLength(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || neighborGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = neighborGScore;
                    fScore[neighbor] = neighborGScore + getEdgeLength(current, destination);

                    if (!openSetContents.Contains(neighbor)) {
                        openSetContents.Add(neighbor);
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return null;
    }

    private static List<TVertex> BuildPath<TVertex>(IReadOnlyDictionary<TVertex, TVertex> cameFrom, TVertex end) {
        var current = end;
        var path = new List<TVertex> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();

        return path;
    }

    private static bool TryGetPathFromMemory<TVertex>(TVertex origin, TVertex destination, IReadOnlyDictionary<TVertex, Dictionary<TVertex, List<TVertex>>> memory, out List<TVertex> path) {
        if (memory.ContainsKey(origin) && memory[origin].ContainsKey(destination)) {
            path = memory[origin][destination];
            return true;
        }

        if (memory.ContainsKey(destination) && memory[destination].ContainsKey(origin)) {
            path = Enumerable.Reverse(memory[destination][origin]).ToList();
            return true;
        }

        path = null;
        return false;
    }

    private static void SavePathToMemory<TVertex>(TVertex origin, TVertex destination, IDictionary<TVertex, Dictionary<TVertex, List<TVertex>>> memory, List<TVertex> path) {
        if (!memory.ContainsKey(origin)) {
            memory[origin] = new Dictionary<TVertex, List<TVertex>> { [destination] = path };
        }
        else {
            memory[origin][destination] = path;
        }
    }
}
