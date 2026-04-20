using System.Diagnostics;
using System.Text.RegularExpressions;
using SmartRoutines.Core.Domain.Models;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class WiFiTrigger : BaseTrigger<TriggerConfiguration>
    {
        private static string _sharedCachedSsid = string.Empty;
        private static DateTime _sharedLastUpdate = DateTime.MinValue;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(2.0);
        private static readonly object CacheLock = new();

        public override string DisplayName => $"WiFi: {Config?.SsidName ?? "Any"}";

        public override Task<bool> ShouldFireAsync()
        {
            if (!IsEnabled || Config == null || string.IsNullOrWhiteSpace(Config.SsidName)) return Task.FromResult(false);

            string currentSsid = GetSsidWithCache().Trim();
            string targetSsid = Config.SsidName.Trim();

            if (string.Equals(currentSsid, targetSsid, StringComparison.OrdinalIgnoreCase))
            {
                if (!HasFired) return Task.FromResult(true);
            }
            else
            {
                Reset();
            }

            return Task.FromResult(false);
        }

        public override string GetDiagnosticInfo()
        {
            if (!IsEnabled) return "Disabled";
            var current = GetSsidWithCache();
            if (string.IsNullOrEmpty(current)) return "No WiFi connected";
            return $"Connected to: '{current}'";
        }

        private string GetSsidWithCache()
        {
            lock (CacheLock)
            {
                if (DateTime.Now - _sharedLastUpdate > CacheTtl)
                {
                    _sharedCachedSsid = GetCurrentSsid();
                    _sharedLastUpdate = DateTime.Now;
                }
                return _sharedCachedSsid;
            }
        }

        private string GetCurrentSsid()
        {
            try
            {
                // FIGMA FIX: Using netsh instead of ManagementObjectSearcher as it doesn't require Admin privileges
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "wlan show interfaces",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Look for the "SSID : Name" line
                var match = Regex.Match(output, @"^\s+SSID\s+:\s+(.*)$", RegexOptions.Multiline);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            catch
            {
                // Silently ignore errors
            }
            return string.Empty;
        }
    }
}