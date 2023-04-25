﻿using System.Linq;
using System.Numerics;
using Bot.Algorithms;
using Bot.ExtensionMethods;
using Bot.MapKnowledge;
using Bot.Utils;
using Bot.VideoClips.Manim.Animations;

namespace Bot.VideoClips.Clips.RayCastingClips;

public class SingleRayCastingClip : Clip {
    private IMapAnalyzer _mapAnalyzer;

    public SingleRayCastingClip(IMapAnalyzer mapAnalyzer, Vector2 sceneLocation, int pauseAtEndOfClipDurationSeconds = 5): base(pauseAtEndOfClipDurationSeconds) {
        _mapAnalyzer = mapAnalyzer;

        var centerCameraAnimation = new CenterCameraAnimation(sceneLocation, startFrame: 0).WithDurationInSeconds(1);
        AddAnimation(centerCameraAnimation);

        var pauseAnimation = new PauseAnimation(centerCameraAnimation.AnimationEndFrame).WithDurationInSeconds(1);
        AddAnimation(pauseAnimation);

        var rayCast = RayCasting.RayCast(sceneLocation, MathUtils.DegToRad(30), cell => !_mapAnalyzer.IsWalkable(cell)).ToList();

        var castRayReadyFrame = CastRay(rayCast.First().RayIntersection, rayCast.Last().RayIntersection, pauseAnimation.AnimationEndFrame);
        ShowWall(rayCast.Last().RayIntersection.AsWorldGridCenter(), castRayReadyFrame);
    }

    private int CastRay(Vector2 rayStart, Vector2 rayEnd, int startAt) {
        var showPointReadyFrame = ShowPoint(rayStart, startAt);

        var pauseAnimation = new PauseAnimation(showPointReadyFrame).WithDurationInSeconds(1);
        AddAnimation(pauseAnimation);

        var lineDrawingAnimation = new LineDrawingAnimation(_mapAnalyzer.WithWorldHeight(rayStart), _mapAnalyzer.WithWorldHeight(rayEnd), ColorService.Instance.RayColor, pauseAnimation.AnimationEndFrame)
            .WithConstantRate(3);

        AddAnimation(lineDrawingAnimation);

        var centerCameraAnimation = new CenterCameraAnimation(rayEnd, lineDrawingAnimation.StartFrame).WithEndFrame(lineDrawingAnimation.AnimationEndFrame);
        AddAnimation(centerCameraAnimation);

        ShowPoint(rayEnd, lineDrawingAnimation.AnimationEndFrame);

        return lineDrawingAnimation.AnimationEndFrame;
    }

    private void ShowWall(Vector2 wall, int startAt) {
        var squareAnimation = new CellDrawingAnimation(_mapAnalyzer, _mapAnalyzer.WithWorldHeight(wall), startAt)
            .WithDurationInSeconds(0.5f);

        AddAnimation(squareAnimation);
    }

    private int ShowPoint(Vector2 origin, int startAt) {
        var sphereAnimation = new SphereDrawingAnimation(_mapAnalyzer.WithWorldHeight(origin), radius: 0.15f, ColorService.Instance.PointColor, startAt)
            .WithDurationInSeconds(0.5f);

        AddAnimation(sphereAnimation);

        return sphereAnimation.AnimationEndFrame;
    }
}
