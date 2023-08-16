﻿namespace Sajuuk.Builds.BuildRequests.Fulfillment;

public interface IBuildRequestFulfillment {
    /// <summary>
    /// The current status of the fulfillment.
    /// </summary>
    BuildRequestFulfillmentStatus Status { get; }

    /// <summary>
    /// Updates the fulfillment status.
    /// </summary>
    void UpdateStatus();

    /// <summary>
    /// Aborts this fulfillment.
    /// </summary>
    void Abort();

    /// <summary>
    /// Cancels this fulfillment, if possible.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Determines whether this fulfillment could fulfill the given build request
    /// </summary>
    /// <param name="buildRequest"></param>
    /// <returns></returns>
    bool CanSatisfy(IBuildRequest buildRequest);
}
