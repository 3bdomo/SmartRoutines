using SmartRoutines.Core.Domain.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class AppLaunchedTrigger : BaseTrigger<TriggerConfiguration>
    {
        public override string DisplayName => $"App launched: {Config?.SsidName ?? "Unknown"}";

        private bool _isRunning = false;

        public override Task<bool> ShouldFireAsync()
        {
            if (Config == null || string.IsNullOrWhiteSpace(Config.SsidName)) return Task.FromResult(false);

            string target = System.IO.Path.GetFileNameWithoutExtension(Config.SsidName);
            _isRunning = Process.GetProcessesByName(target).Any();

            if (_isRunning)
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
            string target = System.IO.Path.GetFileNameWithoutExtension(Config?.SsidName ?? "None");
            return _isRunning ? $"Running: {target}" : $"Not running: {target}";
        }
    }
}