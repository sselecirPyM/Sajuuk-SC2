﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SC2APIProtocol;

namespace Bot.Wrapper;

public static class Debugger {
    private const float CreepHeight = 0.008f;

    private static readonly List<DebugText> DebugTexts = new List<DebugText>();
    private static readonly List<DebugSphere> DebugSpheres = new List<DebugSphere>();
    private static readonly List<DebugBox> DebugBoxes = new List<DebugBox>();
    private static readonly List<DebugLine> DebugLines = new List<DebugLine>();

    // TODO GD Add some optional persistence to the debug elements otherwise they last only 1 frame
    public static Request GetDebugRequest() {
        var request = RequestBuilder.DebugRequest(DebugTexts, DebugSpheres, DebugBoxes, DebugLines);

        DebugTexts.Clear();
        DebugSpheres.Clear();
        DebugBoxes.Clear();
        DebugLines.Clear();

        return request;
    }

    public static void AddDebugText(string text) {
        DebugTexts.Add(new DebugText { Text = text, Size = 18 });
    }

    public static void AddDebugText(IEnumerable<string> texts) {
        DebugTexts.AddRange(texts.Select(text => new DebugText { Text = text, Size = 18 }));
    }

    public static void AddSphere(Unit unit, Color color) {
        AddSphere(
            new Point { X = unit.Position.X, Y = unit.Position.Y, Z = unit.Position.Z + CreepHeight },
            unit.Radius * 1.25f,
            color
        );
    }

    public static void AddSphere(Point point, float radius, Color color) {
        DebugSpheres.Add(
            new DebugSphere
            {
                P = point,
                R = radius,
                Color = color,
            }
        );
    }

    public static void AddSquare(Point centerPoint, float width, Color color) {
        DebugBoxes.Add(
            new DebugBox
            {
                Min = new Point { X = centerPoint.X - width / 2, Y = centerPoint.Y - width / 2, Z = centerPoint.Z + CreepHeight },
                Max = new Point { X = centerPoint.X + width / 2, Y = centerPoint.Y + width / 2, Z = centerPoint.Z + CreepHeight },
                Color = color,
            }
        );
    }

    public static void AddLine(Vector3 start, Vector3 end, Color color) {
        DebugLines.Add(
            new DebugLine
            {
                Line = new Line
                {
                    P0 = new Point { X = start.X, Y = start.Y, Z = start.Z + CreepHeight },
                    P1 = new Point { X = end.X, Y = end.Y, Z = end.Z + CreepHeight },
                },
                Color = color,
            }
        );
    }
}

public static class Colors {
    public static Color White = new Color { R = 1, G = 1, B = 1 };
    public static Color Black = new Color { R = 255, G = 255, B = 255 };

    public static Color Red = new Color { R = 255, G = 1, B = 1 };
    public static Color Green = new Color { R = 1, G = 255, B = 1 };
    public static Color Blue = new Color { R = 1, G = 1, B = 255 };

    public static Color Yellow = new Color { R = 255, G = 255, B = 1 };
    public static Color Cyan = new Color { R = 1, G = 255, B = 255 };
    public static Color Magenta = new Color { R = 255, G = 1, B = 255 };

    public static Color DarkGreen = new Color { R = 1, G = 100, B = 1 };
    public static Color DarkBlue = new Color { R = 1, G = 1, B = 139 };
    public static Color Maroon3 = new Color { R = 176, G = 48, B = 96 };
    public static Color Burlywood = new Color { R = 222, G = 184, B = 135 };
    public static Color Cornflower = new Color { R = 100, G = 149, B = 237 };
}
