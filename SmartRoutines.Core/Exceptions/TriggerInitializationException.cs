using System;

namespace SmartRoutines.Core.Exceptions;

/// <summary>
/// Thrown when a trigger configuration cannot be parsed or initialized.
/// </summary>
public sealed class TriggerInitializationException : SmartRoutinesException
{
    public TriggerInitializationException(string message) : base(message) { }
    public TriggerInitializationException(string message, Exception inner) : base(message, inner) { }
}
