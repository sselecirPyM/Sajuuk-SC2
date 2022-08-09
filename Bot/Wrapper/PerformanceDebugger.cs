﻿using System;
using System.Diagnostics;
using Bot.ExtensionMethods;

namespace Bot.Wrapper;

public class PerformanceDebugger {
    private double _dataPointsCount = 0;

    private double _frameTotalTime = 0;
    private double _botTotalTime = 0;
    private double _controllerTotalTime = 0;
    private double _actionsTotalTime = 0;
    private double _debuggerTotalTime = 0;

    public readonly Stopwatch FrameStopWatch = new Stopwatch();
    public readonly Stopwatch BotStopWatch = new Stopwatch();
    public readonly Stopwatch ControllerStopWatch = new Stopwatch();
    public readonly Stopwatch ActionsStopWatch = new Stopwatch();
    public readonly Stopwatch DebuggerStopWatch = new Stopwatch();

    public void LogTimers(int actionCount) {
        var frameTime = FrameStopWatch.GetElapsedTimeMs();
        var controllerTime = ControllerStopWatch.GetElapsedTimeMs();
        var botTime = BotStopWatch.GetElapsedTimeMs();
        var actionsTime = ActionsStopWatch.GetElapsedTimeMs();
        var debuggerTime = DebuggerStopWatch.GetElapsedTimeMs();

        Logger.Debug(
            "Actions {0,3} | Frame {1,5:F2} ms | Controller ({2,3:P0}) {3,5:F2} ms | Bot ({4,3:P0}) {5,5:F2} ms | Actions ({6,3:P0}) {7,5:F2} ms | Debugger ({8,3:P0}) {9,5:F2} ms",
            actionCount,
            frameTime,
            controllerTime / frameTime,
            controllerTime,
            botTime / frameTime,
            botTime,
            actionsTime / frameTime,
            actionsTime,
            debuggerTime / frameTime,
            debuggerTime
        );
    }

    public void LogAveragePerformance() {
        var averageControllerPercent = _controllerTotalTime / _frameTotalTime;
        var averageBotPercent = _botTotalTime / _frameTotalTime;
        var averageActionsPercent = _actionsTotalTime / _frameTotalTime;
        var averageDebuggerPercent = _debuggerTotalTime / _frameTotalTime;

        var averageFrameTime = _frameTotalTime / _dataPointsCount;
        var averageControllerTime = _controllerTotalTime / _dataPointsCount;
        var averageBotTime = _botTotalTime / _dataPointsCount;
        var averageActionsTime = _actionsTotalTime / _dataPointsCount;
        var averageDebuggerTime = _debuggerTotalTime / _dataPointsCount;

        Logger.Debug(
            "Average performance: Frame {0,5:F2} ms | Controller ({1,3:P0}) {2,5:F2} ms | Bot ({3,3:P0}) {4,5:F2} ms | Actions ({5,3:P0}) {6,5:F2} ms | Debugger ({7,3:P0}) {8,5:F2} ms",
            averageFrameTime,
            averageControllerPercent,
            averageControllerTime,
            averageBotPercent,
            averageBotTime,
            averageActionsPercent,
            averageActionsTime,
            averageDebuggerPercent,
            averageDebuggerTime
        );
    }

    public void CompileData() {
        _dataPointsCount++;

        _frameTotalTime += FrameStopWatch.GetElapsedTimeMs();
        _botTotalTime += ControllerStopWatch.GetElapsedTimeMs();
        _controllerTotalTime += BotStopWatch.GetElapsedTimeMs();
        _actionsTotalTime += ActionsStopWatch.GetElapsedTimeMs();
        _debuggerTotalTime += DebuggerStopWatch.GetElapsedTimeMs();

        ResetTimers();
    }

    public void ResetTimers() {

        FrameStopWatch.Reset();
        ControllerStopWatch.Reset();
        BotStopWatch.Reset();
        ActionsStopWatch.Reset();
        DebuggerStopWatch.Reset();
    }
}