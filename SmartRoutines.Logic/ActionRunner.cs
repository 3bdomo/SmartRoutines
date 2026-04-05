using System;
using System.Collections.Generic;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces;
using SmartRoutines.Core.Models;

namespace SmartRoutines.Logic
{
    public class ActionRunner
    {
        private readonly List<IAction> _actions;
        private readonly ActionContext _actionContext;

        public ActionRunner(List<IAction> actions, ActionContext actionContext)
        {
            _actions = actions;
            _actionContext = actionContext;
        }

        public void Run()
        {
            foreach (var action in _actions)
            {
                try
                {
                    action.Execute(_actionContext);
                    // Notify Member 4 of SUCCESS
                    // ReportToTracker(action.ActionName, "SUCCESS", null);
                }
                catch (ActionFailedException ex)
                {
                    // SRS Requirement: A failed action must not stop remaining actions 
                    // Notify Member 4 of FAILURE with specific details
                    // ReportToTracker(ex.ActionType, "FAILURE", ex.Message);
                }
                catch (Exception ex)
                {
                    // Catch-all for unexpected crashes to keep the app running
                    // ReportToTracker("Unknown", "CRITICAL_FAILURE", ex.Message);
                }
            }
        }
    }
}
