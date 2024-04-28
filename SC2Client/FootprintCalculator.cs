﻿using System.Numerics;
using Algorithms.ExtensionMethods;
using SC2Client.ExtensionMethods;
using SC2Client.GameData;

namespace SC2Client;

public class FootprintCalculator {
    private readonly ILogger _logger;

    public FootprintCalculator(ILogger logger) {
        _logger = logger;
    }

    public List<Vector2> GetFootprint(IUnit obstacle) {
        if (Units.MineralFields.Contains(obstacle.UnitType)) {
            return GetMineralFootprint(obstacle);
        }

        if (Units.Obstacles.Contains(obstacle.UnitType)) {
            return GetRockFootprint(obstacle);
        }

        return GetGenericFootprint(obstacle);
    }

    private static List<Vector2> GetMineralFootprint(IUnit mineral) {
        // Mineral fields are 1x2
        return new List<Vector2>
        {
            mineral.Position.Translate(xTranslation: -0.5f).AsWorldGridCenter().ToVector2(),
            mineral.Position.Translate(xTranslation: 0.5f).AsWorldGridCenter().ToVector2(),
        };
    }

    private List<Vector2> GetRockFootprint(IUnit rock) {
        var footprint = new List<Vector2>();
        switch (rock.UnitType) {
            case Units.DestructibleDebris4x4:
            case Units.DestructibleRock4x4:
            case Units.DestructibleRockEx14x4:
                footprint.AddRange(new Vector2[]
                {
                    new(-1.5f,  1.5f), new(-0.5f,  1.5f), new(0.5f,  1.5f), new(1.5f,  1.5f),
                    new(-1.5f,  0.5f), new(-0.5f,  0.5f), new(0.5f,  0.5f), new(1.5f,  0.5f),
                    new(-1.5f, -0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f), new(1.5f, -0.5f),
                    new(-1.5f, -1.5f), new(-0.5f, -1.5f), new(0.5f, -1.5f), new(1.5f, -1.5f),
                });
                break;
            case Units.DestructibleCityDebris6x6:
            case Units.DestructibleDebris6x6:
            case Units.DestructibleRock6x6:
            case Units.DestructibleRockEx16x6:
                footprint.AddRange(new Vector2[]
                {
                                       new(-1.5f,  2.5f), new(-0.5f,  2.5f), new(0.5f,  2.5f), new(1.5f,  2.5f),
                    new(-2.5f,  1.5f), new(-1.5f,  1.5f), new(-0.5f,  1.5f), new(0.5f,  1.5f), new(1.5f,  1.5f), new(2.5f,  1.5f),
                    new(-2.5f,  0.5f), new(-1.5f,  0.5f), new(-0.5f,  0.5f), new(0.5f,  0.5f), new(1.5f,  0.5f), new(2.5f,  0.5f),
                    new(-2.5f, -0.5f), new(-1.5f, -0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f), new(1.5f, -0.5f), new(2.5f, -0.5f),
                    new(-2.5f, -1.5f), new(-1.5f, -1.5f), new(-0.5f, -1.5f), new(0.5f, -1.5f), new(1.5f, -1.5f), new(2.5f, -1.5f),
                                       new(-1.5f, -2.5f), new(-0.5f, -2.5f), new(0.5f, -2.5f), new(1.5f, -2.5f),
                });
                break;
            case Units.DestructibleRampDiagonalHugeBLUR:
            case Units.DestructibleDebrisRampDiagonalHugeBLUR:
            case Units.DestructibleCityDebrisHugeDiagonalBLUR:
            case Units.DestructibleRockEx1DiagonalHugeBLUR:
                footprint.AddRange(new Vector2[]
                {
                                                                                                                                     new(1.5f,  4.5f), new(2.5f,  4.5f), new(3.5f,  4.5f),
                                                                                                                   new(0.5f,  3.5f), new(1.5f,  3.5f), new(2.5f,  3.5f), new(3.5f,  3.5f), new(4.5f,  3.5f),
                                                                                                new(-0.5f,  2.5f), new(0.5f,  2.5f), new(1.5f,  2.5f), new(2.5f,  2.5f), new(3.5f,  2.5f), new(4.5f,  2.5f),
                                                                             new(-1.5f,  1.5f), new(-0.5f,  1.5f), new(0.5f,  1.5f), new(1.5f,  1.5f), new(2.5f,  1.5f), new(3.5f,  1.5f), new(4.5f,  1.5f),
                                                          new(-2.5f,  0.5f), new(-1.5f,  0.5f), new(-0.5f,  0.5f), new(0.5f,  0.5f), new(1.5f,  0.5f), new(2.5f,  0.5f), new(3.5f,  0.5f),
                                       new(-3.5f, -0.5f), new(-2.5f, -0.5f), new(-1.5f, -0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f), new(1.5f, -0.5f), new(2.5f, -0.5f),
                    new(-4.5f, -1.5f), new(-3.5f, -1.5f), new(-2.5f, -1.5f), new(-1.5f, -1.5f), new(-0.5f, -1.5f), new(0.5f, -1.5f), new(1.5f, -1.5f),
                    new(-4.5f, -2.5f), new(-3.5f, -2.5f), new(-2.5f, -2.5f), new(-1.5f, -2.5f), new(-0.5f, -2.5f), new(0.5f, -2.5f),
                    new(-4.5f, -3.5f), new(-3.5f, -3.5f), new(-2.5f, -3.5f), new(-1.5f, -3.5f), new(-0.5f, -3.5f),
                                       new(-3.5f, -4.5f), new(-2.5f, -4.5f), new(-1.5f, -4.5f),
                });
                break;
            case Units.DestructibleRampDiagonalHugeULBR:
            case Units.DestructibleDebrisRampDiagonalHugeULBR:
            case Units.DestructibleRockEx1DiagonalHugeULBR:
                footprint.AddRange(new Vector2[]
                {
                                       new(-3.5f,  4.5f), new(-2.5f,  4.5f), new(-1.5f,  4.5f),
                    new(-4.5f,  3.5f), new(-3.5f,  3.5f), new(-2.5f,  3.5f), new(-1.5f,  3.5f), new(-0.5f,  3.5f),
                    new(-4.5f,  2.5f), new(-3.5f,  2.5f), new(-2.5f,  2.5f), new(-1.5f,  2.5f), new(-0.5f,  2.5f), new(0.5f,  2.5f),
                    new(-4.5f,  1.5f), new(-3.5f,  1.5f), new(-2.5f,  1.5f), new(-1.5f,  1.5f), new(-0.5f,  1.5f), new(0.5f,  1.5f), new(1.5f,  1.5f),
                                       new(-3.5f,  0.5f), new(-2.5f,  0.5f), new(-1.5f,  0.5f), new(-0.5f,  0.5f), new(0.5f,  0.5f), new(1.5f,  0.5f), new(2.5f,  0.5f),
                                                          new(-2.5f, -0.5f), new(-1.5f, -0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f), new(1.5f, -0.5f), new(2.5f, -0.5f), new(3.5f, -0.5f),
                                                                             new(-1.5f, -1.5f), new(-0.5f, -1.5f), new(0.5f, -1.5f), new(1.5f, -1.5f), new(2.5f, -1.5f), new(3.5f, -1.5f), new(4.5f, -1.5f),
                                                                                                new(-0.5f, -2.5f), new(0.5f, -2.5f), new(1.5f, -2.5f), new(2.5f, -2.5f), new(3.5f, -2.5f), new(4.5f, -2.5f),
                                                                                                                   new(0.5f, -3.5f), new(1.5f, -3.5f), new(2.5f, -3.5f), new(3.5f, -3.5f), new(4.5f, -3.5f),
                                                                                                                                     new(1.5f, -4.5f), new(2.5f, -4.5f), new(3.5f, -4.5f),
                });
                break;
            case Units.DestructibleRampVerticalHuge:
                footprint.AddRange(new Vector2[]
                {
                    new(-1.5f,  5.5f), new(-0.5f,  5.5f), new(0.5f,  5.5f), new(1.5f,  5.5f),
                    new(-1.5f,  4.5f), new(-0.5f,  4.5f), new(0.5f,  4.5f), new(1.5f,  4.5f),
                    new(-1.5f,  3.5f), new(-0.5f,  3.5f), new(0.5f,  3.5f), new(1.5f,  3.5f),
                    new(-1.5f,  2.5f), new(-0.5f,  2.5f), new(0.5f,  2.5f), new(1.5f,  2.5f),
                    new(-1.5f,  1.5f), new(-0.5f,  1.5f), new(0.5f,  1.5f), new(1.5f,  1.5f),
                    new(-1.5f,  0.5f), new(-0.5f,  0.5f), new(0.5f,  0.5f), new(1.5f,  0.5f),
                    new(-1.5f, -0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f), new(1.5f, -0.5f),
                    new(-1.5f, -1.5f), new(-0.5f, -1.5f), new(0.5f, -1.5f), new(1.5f, -1.5f),
                    new(-1.5f, -2.5f), new(-0.5f, -2.5f), new(0.5f, -2.5f), new(1.5f, -2.5f),
                    new(-1.5f, -3.5f), new(-0.5f, -3.5f), new(0.5f, -3.5f), new(1.5f, -3.5f),
                    new(-1.5f, -4.5f), new(-0.5f, -4.5f), new(0.5f, -4.5f), new(1.5f, -4.5f),
                    new(-1.5f, -5.5f), new(-0.5f, -5.5f), new(0.5f, -5.5f), new(1.5f, -5.5f),
                });
                break;
            case Units.DestructibleRockEx1HorizontalHuge:
                footprint.AddRange(new Vector2[]
                {
                    new(-5.5f,  1.5f), new(-4.5f,  1.5f), new(-3.5f,  1.5f), new(-2.5f,  1.5f), new(-1.5f,  1.5f), new(-0.5f,  1.5f), new(0.5f,  1.5f), new(1.5f,  1.5f), new(2.5f,  1.5f), new(3.5f,  1.5f), new(4.5f,  1.5f), new(5.5f,  1.5f),
                    new(-5.5f,  0.5f), new(-4.5f,  0.5f), new(-3.5f,  0.5f), new(-2.5f,  0.5f), new(-1.5f,  0.5f), new(-0.5f,  0.5f), new(0.5f,  0.5f), new(1.5f,  0.5f), new(2.5f,  0.5f), new(3.5f,  0.5f), new(4.5f,  0.5f), new(5.5f,  0.5f),
                    new(-5.5f, -0.5f), new(-4.5f, -0.5f), new(-3.5f, -0.5f), new(-2.5f, -0.5f), new(-1.5f, -0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f), new(1.5f, -0.5f), new(2.5f, -0.5f), new(3.5f, -0.5f), new(4.5f, -0.5f), new(5.5f, -0.5f),
                    new(-5.5f, -1.5f), new(-4.5f, -1.5f), new(-3.5f, -1.5f), new(-2.5f, -1.5f), new(-1.5f, -1.5f), new(-0.5f, -1.5f), new(0.5f, -1.5f), new(1.5f, -1.5f), new(2.5f, -1.5f), new(3.5f, -1.5f), new(4.5f, -1.5f), new(5.5f, -1.5f),
                });
                break;
            case Units.UnbuildablePlatesDestructible:
                // You can walk on it, no footprint (please I hope I never rely on this for placement, yikes)
                break;
            default:
                _logger.Warning($"No footprint found for rock {rock.Name}[{rock.UnitType}] at {rock.Position}");
                return GetGenericFootprint(rock);
        }

        return footprint.Select(cell => cell + rock.Position.ToVector2()).ToList();
    }

    private static List<Vector2> GetGenericFootprint(IUnit obstacle) {
        return BuildSearchGrid(obstacle.Position.ToVector2(), (int)obstacle.Radius)
            .Select(cell => cell.AsWorldGridCenter())
            .ToList();
    }

    private static IEnumerable<Vector2> BuildSearchGrid(Vector2 centerPosition, int gridRadius, float stepSize = KnowledgeBase.GameGridCellWidth) {
        var grid = new List<Vector2>();
        for (var x = centerPosition.X - gridRadius; x <= centerPosition.X + gridRadius; x += stepSize) {
            for (var y = centerPosition.Y - gridRadius; y <= centerPosition.Y + gridRadius; y += stepSize) {
                grid.Add(new Vector2(x, y));
            }
        }

        return grid.OrderBy(position => Vector2.Distance(centerPosition, position));
    }
}