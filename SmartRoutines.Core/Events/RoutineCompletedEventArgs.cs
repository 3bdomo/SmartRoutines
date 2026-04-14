using System;

namespace SmartRoutines.Core.Events;

/// <summary>
/// Event arguments raised when a routine completes execution.
/// </summary>
public sealed class RoutineCompletedEventArgs : EventArgs
{
    public Guid RoutineId { get; }
    public bool Succeeded { get; }
    public Exception? Error { get; }

    public RoutineCompletedEventArgs(Guid routineId, bool succeeded, Exception? error = null)
    {
        RoutineId = routineId;
        Succeeded = succeeded;
        Error = error;
    }
}
