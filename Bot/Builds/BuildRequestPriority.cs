﻿namespace Bot.Builds;

/// <summary>
/// Represents the priority of a BuildRequest.
/// Higher priority means more important.
/// </summary>
public enum BuildRequestPriority {
    Normal = 0,
    BuildOrder = 1,
    Notable = 2,
    Important = 3,
}
