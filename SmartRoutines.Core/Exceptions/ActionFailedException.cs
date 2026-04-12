using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Core.Exceptions;

public class ActionFailedException : Exception
{
    public ActionType ActionType { get; }
    public DateTime Timestamp { get; }

    public ActionFailedException(ActionType actionType, string message) : base(message)
    {
        ActionType = actionType;
        Timestamp = DateTime.Now;
    }

    public ActionFailedException(ActionType actionType, string message, Exception innerException) 
        : base(message, innerException)
    {
        ActionType = actionType;
        Timestamp = DateTime.Now;
    }
}