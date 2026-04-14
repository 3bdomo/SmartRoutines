using System.Linq;
using System.Management;
using System.Text.Json;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class WiFiTrigger : BaseTrigger<TriggerConfiguration>
    {
        public override string DisplayName => $"Connects to WiFi: '{Config?.SsidName}'";

        public override bool ShouldFire()
        {
            if (!IsEnabled || Config == null || string.IsNullOrWhiteSpace(Config.SsidName)) return false;

            string currentSsid = GetCurrentSsid();

            if (currentSsid == Config.SsidName)
            {
                if (!HasFired) return true;
            }
            else
            {
                Reset();
            }

            return false;
        }

        private string GetCurrentSsid()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSNdis_80211_ServiceSetIdentifier WHERE Active=True");
                var activeConnection = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

                if (activeConnection != null)
                {
                    var ssidBytes = (byte[])activeConnection["Ndis80211SsId"];
                    var ssid = System.Text.Encoding.ASCII.GetString(ssidBytes).Trim('\0');
                    return ssid;
                }
            }
            catch
            {
                // Silently ignore WMI permission errors
            }
            return string.Empty;
        }
    }
}