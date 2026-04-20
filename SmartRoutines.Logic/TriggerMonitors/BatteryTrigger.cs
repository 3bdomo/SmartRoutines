using System.Text.Json;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.Helpers;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class BatteryTrigger : BaseTrigger<TriggerConfiguration>
    {
        public override string DisplayName => $"Battery drops below {Config?.BatteryThreshold}%";

        private string _lastStatus = "Checking...";

        public override Task<bool> ShouldFireAsync()
        {
            if (!IsEnabled || Config == null) return Task.FromResult(false);

            if (NativeMethods.GetSystemPowerStatus(out var status))
            {
                _lastStatus = $"{status.BatteryLifePercent}% ({(status.ACLineStatus == 1 ? "Plugged in" : "Battery")})";

                // Reset flag if battery goes above threshold
                if (status.BatteryLifePercent > Config.BatteryThreshold)
                {
                    Reset();
                    return Task.FromResult(false);
                }

                // Fire if battery is <= threshold, hasn't fired yet, AND not plugged in (ACLineStatus == 0)
                if (status.BatteryLifePercent <= Config.BatteryThreshold && !HasFired)
                {
                    if (status.ACLineStatus == 0)
                        return Task.FromResult(true);
                }
            }
            else
            {
                _lastStatus = "Error reading battery";
            }
            return Task.FromResult(false);
        }

        public override string GetDiagnosticInfo()
        {
            if (!IsEnabled) return "Disabled";
            return $"Battery: {_lastStatus} (Target: <={Config?.BatteryThreshold}%)";
        }
    }
}