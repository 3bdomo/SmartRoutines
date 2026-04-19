using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class StartupTrigger : BaseTrigger
    {
        public override string DisplayName => "At System Startup";

        public override Task<bool> ShouldFireAsync()
        {
            return Task.FromResult(IsEnabled && !HasFired);
        }

        public override string GetDiagnosticInfo() => HasFired ? "Already fired at startup" : "Awaiting startup trigger";
    }
}