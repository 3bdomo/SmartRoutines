using System;
using System.Collections.Generic;

namespace SmartRoutines.Core.Models;

/// <summary>
/// Provides runtime metadata for executing a routine action.
/// </summary>
public class ActionContext
{
    /// <summary>
    /// Gets the name of the routine currently being executed.
    /// </summary>
    public string RoutineName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp when the trigger condition was met.
    /// </summary>
    public DateTime TriggerTime { get; init; }

    /// <summary>
    /// Gets trigger-specific data (for example, battery level or Wi-Fi SSID).
    /// </summary>
    public IReadOnlyDictionary<string, object?> TriggerData { get; init; } = new Dictionary<string, object?>();

    /// <summary>
    /// Gets a value indicating whether the routine was started manually.
    /// </summary>
    public bool IsManualTrigger { get; init; }
}