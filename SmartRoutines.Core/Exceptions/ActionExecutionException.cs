using System;

namespace SmartRoutines.Core.Exceptions;

/// <summary>
/// Thrown when an action fails during execution in the automation engine.
/// </summary>
public sealed class ActionExecutionException : SmartRoutinesException
{
    public ActionExecutionException(string message) : base(message) { }
    public ActionExecutionException(string message, Exception inner) : base(message, inner) { }
}
