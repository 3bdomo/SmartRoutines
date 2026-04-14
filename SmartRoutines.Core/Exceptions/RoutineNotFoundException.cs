using System;

namespace SmartRoutines.Core.Exceptions;

/// <summary>
/// Thrown when a requested routine cannot be found in the data store.
/// </summary>
public sealed class RoutineNotFoundException : SmartRoutinesException
{
    public Guid RoutineId { get; }

    public RoutineNotFoundException(Guid id)
        : base($"Routine with id '{id}' was not found.")
    {
        RoutineId = id;
    }

    public RoutineNotFoundException(Guid id, Exception inner)
        : base($"Routine with id '{id}' was not found.", inner)
    {
        RoutineId = id;
    }
}
