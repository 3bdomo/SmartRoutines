using SmartRoutines.Core.Enums;
using SmartRoutines.Core.Models;

namespace SmartRoutines.Core.Interfaces;

public interface IAction
{
    ActionType ActionType { get; }
    void Execute(ActionContext context); // The core logic of the action
    LogStatus Status { get; }
    string ErrorMessage { get; }
}