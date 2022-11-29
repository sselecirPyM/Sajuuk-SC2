﻿using System.Collections.Generic;
using Bot.Builds;
using Bot.Debugging;
using Bot.MapKnowledge;
using SC2APIProtocol;

namespace Bot.Managers.WarManagement.States.MidGame;

public class MidGameBehaviourDebugger {
    public float OwnForce { get; set; }
    public float EnemyForce { get; set; }
    public Stance CurrentStance { get; set; }
    public Region Target { get; set; }

    public BuildRequestPriority BuildPriority { get; set; }
    public BuildBlockCondition BuildBlockCondition { get; set; }

    public void Debug() {
        if (!DebuggingFlagsTracker.IsActive(DebuggingFlags.WarManager)) {
            return;
        }

        var texts = new List<string>
        {
            "Mid Game Behaviour",
            $"Own force:   {OwnForce:F1}",
            $"Enemy force: {EnemyForce:F1}",
        };

        if (Target != null) {
            texts.Add($"Stance: {CurrentStance} region {Target.Id}");
        }

        texts.Add($"Build");
        texts.Add($" - Priority: {BuildPriority}");
        texts.Add($" - Blocking: {BuildBlockCondition}");

        Program.GraphicalDebugger.AddTextGroup(texts, virtualPos: new Point { X = 0.30f, Y = 0.02f });
    }
}
