using System;

namespace SmartRoutines.Core.Exceptions
{
    /// <summary>
    /// Base class for all custom exceptions in SmartRoutines.
    /// Inherit from this to provide domain-specific exceptions.
    /// </summary>
    public abstract class SmartRoutinesException : Exception
    {
        protected SmartRoutinesException()
        {
        }

        protected SmartRoutinesException(string message) : base(message)
        {
        }

        protected SmartRoutinesException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
