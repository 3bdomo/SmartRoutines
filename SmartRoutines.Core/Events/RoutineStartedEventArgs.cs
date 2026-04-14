using System;

namespace SmartRoutines.Core.Events;

/// <summary>
/// Event arguments raised when a routine starts executing.
/// </summary>
public sealed class RoutineStartedEventArgs : EventArgs
{
    public Guid RoutineId { get; }
    public RoutineStartedEventArgs(Guid routineId) => RoutineId = routineId;
}
