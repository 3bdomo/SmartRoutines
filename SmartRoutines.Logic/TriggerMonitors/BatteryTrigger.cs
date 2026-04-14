using System.Text.Json;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class BatteryTrigger : BaseTrigger<TriggerConfiguration>
    {
        public override string DisplayName => $"Battery drops below {Config?.BatteryThreshold}%";

        public override bool ShouldFire()
        {
            if (!IsEnabled || Config == null) return false;

            if (NativeMethods.GetSystemPowerStatus(out var status))
            {
                // Reset flag if battery goes above threshold
                if (status.BatteryLifePercent > Config.BatteryThreshold)
                {
                    Reset();
                    return false;
                }

                // Fire if battery is <= threshold, hasn't fired yet, AND not plugged in (ACLineStatus == 0)
                if (status.BatteryLifePercent <= Config.BatteryThreshold && !HasFired)
                {
                    if (status.ACLineStatus == 0)
                        return true;
                }
            }
            return false;
        }
    }
}