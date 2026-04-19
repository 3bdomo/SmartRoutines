using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRoutines.Core.Events;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Core automation runtime engine responsible for starting/stopping and executing routines.
/// </summary>
public interface IAutomationEngine
{
    /// <summary>
    /// True when engine loop/process is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Identifiers of routines currently executing.
    /// </summary>
    IReadOnlyCollection<Guid> RunningRoutineIds { get; }

    /// <summary>
    /// Starts the background engine that evaluates triggers and executes routines.
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Stops the background engine gracefully.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Executes a routine immediately on-demand (manual trigger).
    /// </summary>
    Task ExecuteManualAsync(Guid routineId);

    /// <summary>
    /// Returns real-time diagnostic information for a specific routine's trigger.
    /// </summary>
    string GetRoutineDiagnostic(Guid routineId);

    /// <summary>
    /// Raised when a routine begins execution.
    /// </summary>
    event EventHandler<RoutineStartedEventArgs>? RoutineStarted;

    /// <summary>
    /// Raised when a routine completes execution.
    /// </summary>
    event EventHandler<RoutineCompletedEventArgs>? RoutineCompleted;

    /// <summary>
    /// Raised when the engine emits an activity log entry.
    /// </summary>
    event EventHandler<LogEmittedEventArgs>? LogEmitted;
}
