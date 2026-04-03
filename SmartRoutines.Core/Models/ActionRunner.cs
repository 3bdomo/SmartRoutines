using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces;
using SmartRoutines.Core.Models;

namespace SmartRoutines.Core;

public class ActionRunner(List<IAction> actions,ActionContext actionContext)
{
    public void Run()
    {
        foreach (var action in actions)
        {
            try
            {
                action.Execute(actionContext );
                // Notify Member 4 of SUCCESS
               // ReportToTracker(action.ActionName, "SUCCESS", null);
            }
            catch (ActionFailedException ex)
            {
               // [cite_start]// SRS Requirement: A failed action must not stop remaining actions 
                // Notify Member 4 of FAILURE with specific details
              //  ReportToTracker(ex.ActionType, "FAILURE", ex.Message);
            }
            catch (Exception ex)
            {
                // Catch-all for unexpected crashes to keep the app running
               // ReportToTracker("Unknown", "CRITICAL_FAILURE", ex.Message);
            }
        }
    }
}