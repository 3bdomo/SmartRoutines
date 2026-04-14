using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class StartupTrigger : BaseTrigger
    {
        public override string DisplayName => "At System Startup";

        public override bool ShouldFire()
        {
            return IsEnabled && !HasFired;
        }
    }
}