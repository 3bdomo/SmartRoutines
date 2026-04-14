using System;
using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Core.Domain.Models;

/// <summary>
/// Execution-time model representing a single action inside a routine.
/// This is an immutable, in-memory snapshot used by the engine and action runners.
/// </summary>
public sealed class RuntimeAction
{
    /// <summary>
    /// Gets the action type.
    /// </summary>
    public ActionType Type { get; init; }

    /// <summary>
    /// Gets the raw arguments (JSON or plain string) required to execute the action.
    /// </summary>
    public string Arguments { get; init; } = string.Empty;

    /// <summary>
    /// Gets the 1-based execution order for this action within its routine.
    /// </summary>
    public int ExecutionOrder { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="RuntimeAction"/>.
    /// </summary>
    /// <param name="type">Action type.</param>
    /// <param name="arguments">Raw arguments (JSON or string).</param>
    /// <param name="executionOrder">1-based execution order; must be >= 1.</param>
    public RuntimeAction(ActionType type, string arguments, int executionOrder)
    {
        if (arguments is null)
            throw new ArgumentNullException(nameof(arguments));

        if (executionOrder < 1)
            throw new ArgumentOutOfRangeException(nameof(executionOrder), "ExecutionOrder must be 1 or greater.");

        Type = type;
        Arguments = arguments;
        ExecutionOrder = executionOrder;
    }
}
