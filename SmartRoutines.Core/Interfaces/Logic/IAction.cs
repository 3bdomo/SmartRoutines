using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Enums;

namespace SmartRoutines.Core.Interfaces;

public interface IAction
{
    void Execute(ActionEntry entry,ActionContext context); // The core logic of the action

}