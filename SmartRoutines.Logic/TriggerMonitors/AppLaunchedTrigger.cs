using SmartRoutines.Core.DTOs.Configurations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class AppLaunchedTrigger : BaseTrigger<AppLaunchedTriggerConfig>
    {
        private string _processName = string.Empty;

        public override string DisplayName => $"App launched: {_processName}";

        public override void Configure(string json)
        {
            _processName = json;
        }

        public override bool ShouldFire()
        {
            if (string.IsNullOrEmpty(_processName)) return false;

            var isRunning = Process.GetProcessesByName(_processName).Any();

            return IsEnabled && isRunning && !HasFired;
        }
    }
}