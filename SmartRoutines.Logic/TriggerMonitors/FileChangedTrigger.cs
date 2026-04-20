using System;
using System.IO;
using System.Threading.Tasks;
using SmartRoutines.Core.Domain.Models;

namespace SmartRoutines.Logic.TriggerMonitors
{
    /// <summary>
    /// Monitors a specific file for changes based on its LastWriteTime.
    /// Reuses SsidName in the config to store the File Path.
    /// </summary>
    public class FileChangedTrigger : BaseTrigger<TriggerConfiguration>
    {
        private DateTime? _lastRecordedWriteTime;

        public override string DisplayName => $"File changed: {Path.GetFileName(Config?.SsidName ?? "None")}";

        public override Task<bool> ShouldFireAsync()
        {
            if (!IsEnabled || Config == null || string.IsNullOrWhiteSpace(Config.SsidName))
                return Task.FromResult(false);

            try
            {
                if (!File.Exists(Config.SsidName))
                    return Task.FromResult(false);

                var currentWriteTime = File.GetLastWriteTime(Config.SsidName);

                if (!_lastRecordedWriteTime.HasValue)
                {
                    // Initial baseline
                    _lastRecordedWriteTime = currentWriteTime;
                    return Task.FromResult(false);
                }

                if (currentWriteTime > _lastRecordedWriteTime.Value)
                {
                    if (!HasFired)
                    {
                        _lastRecordedWriteTime = currentWriteTime;
                        return Task.FromResult(true);
                    }
                }
                else if (currentWriteTime < _lastRecordedWriteTime.Value)
                {
                    // If file was replaced or modified with an older timestamp somehow, reset baseline
                    _lastRecordedWriteTime = currentWriteTime;
                    Reset();
                }
            }
            catch
            {
                // Silently ignore IO errors (e.g. file locked by another process)
            }

            return Task.FromResult(false);
        }

        public override string GetDiagnosticInfo()
        {
            if (!IsEnabled) return "Disabled";
            if (Config == null || string.IsNullOrWhiteSpace(Config.SsidName)) return "No file path configured";
            if (!File.Exists(Config.SsidName)) return "File not found";
            
            return _lastRecordedWriteTime.HasValue 
                ? $"Monitoring: {Path.GetFileName(Config.SsidName)} (Last wait: {_lastRecordedWriteTime:HH:mm:ss})"
                : "Initializing monitor...";
        }

        public override void Reset()
        {
            base.Reset();
            // Optional: reset baseline if we want it to trigger on the NEXT change after reset.
            // For now we keep the last recorded time so it only triggers on NEW changes.
        }
    }
}
