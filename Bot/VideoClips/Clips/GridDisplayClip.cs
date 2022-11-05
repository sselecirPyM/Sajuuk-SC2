﻿using System;
using System.Linq;
using System.Numerics;
using Bot.ExtensionMethods;
using Bot.MapKnowledge;
using Bot.Utils;
using Bot.VideoClips.Manim.Animations;

namespace Bot.VideoClips.Clips;

public class GridDisplayClip : Clip {
    public GridDisplayClip(Vector2 origin, Vector2 sceneLocation, int pauseAtEndOfClipDurationSeconds = 5): base(pauseAtEndOfClipDurationSeconds) {
        var cameraReadyFrame = CenterCamera(origin, sceneLocation);
        ShowGrid(sceneLocation, cameraReadyFrame);
    }

    private int ShowGrid(Vector2 origin, int startAt) {
        var grid = MapAnalyzer.BuildSearchRadius(origin, 15).ToList();
        var maxDistance = grid.Max(cell => cell.DistanceTo(origin));
        var animationTotalDuration = TimeUtils.SecsToFrames(2);

        var endFrame = startAt;
        foreach (var cell in grid) {
            var relativeDistance = cell.DistanceTo(origin) / maxDistance;
            var startFrame = startAt + (int)(relativeDistance * animationTotalDuration);
            var squareAnimation = new CellDrawingAnimation(cell.ToVector3(), startFrame).WithDurationInSeconds(0.5f);

            AddAnimation(squareAnimation);

            endFrame = Math.Max(endFrame, squareAnimation.EndFrame);
        }

        return endFrame;
    }
}