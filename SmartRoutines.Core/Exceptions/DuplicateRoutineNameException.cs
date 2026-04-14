using System;

namespace SmartRoutines.Core.Exceptions;

/// <summary>
/// Thrown when a routine name would violate uniqueness constraints.
/// </summary>
public sealed class DuplicateRoutineNameException : SmartRoutinesException
{
    public string RoutineName { get; }

    public DuplicateRoutineNameException(string name)
        : base($"A routine with the name '{name}' already exists.")
    {
        RoutineName = name;
    }

    public DuplicateRoutineNameException(string name, Exception inner)
        : base($"A routine with the name '{name}' already exists.", inner)
    {
        RoutineName = name;
    }
}
