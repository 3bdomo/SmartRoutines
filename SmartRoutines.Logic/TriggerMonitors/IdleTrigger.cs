using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.Helpers;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class IdleTrigger : BaseTrigger<TriggerConfiguration>
    {
        public override string DisplayName => $"System idle for {Config?.IdleMinutes} minutes";

        private string _lastIdleInfo = "Initializing...";

        public override Task<bool> ShouldFireAsync()
        {
            if (!IsEnabled || Config == null || !Config.IdleMinutes.HasValue) 
                return Task.FromResult(false);

            var lastInput = new NativeMethods.LASTINPUTINFO();
            lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);

            if (NativeMethods.GetLastInputInfo(ref lastInput))
            {
                // Use TickCount64 to avoid negative values, but handle 32-bit dwTime rollover
                long now = Environment.TickCount64;
                uint idleTimeMs = (uint)(now & 0xFFFFFFFF) - lastInput.dwTime;
                TimeSpan idleTime = TimeSpan.FromMilliseconds(idleTimeMs);

                _lastIdleInfo = $"{Math.Floor(idleTime.TotalMinutes)}m {idleTime.Seconds}s";

                if (idleTime.TotalMinutes >= Config.IdleMinutes.Value)
                {
                    if (!HasFired) return Task.FromResult(true);
                }
                else
                {
                    // Reset if user moved mouse or typed (idle time is small)
                    Reset();
                }
            }
            return Task.FromResult(false);
        }

        public override string GetDiagnosticInfo()
        {
            if (!IsEnabled) return "Disabled";
            return $"Idle for: {_lastIdleInfo} (Target: {Config?.IdleMinutes}m)";
        }
    }
}