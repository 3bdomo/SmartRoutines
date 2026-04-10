using SmartRoutines.Core.Enums;
using SmartRoutines.Core.Models;

namespace SmartRoutines.Core.Interfaces;

public interface IAction
{
    void Execute(ActionEntry entry,ActionContext context); // The core logic of the action

}