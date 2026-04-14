using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class IdleTrigger : BaseTrigger<TriggerConfiguration>
    {
        public override string DisplayName => $"System idle for {Config?.IdleMinutes} minutes";

        public override bool ShouldFire()
        {
            if (!IsEnabled || Config == null) return false;

            var lastInput = new NativeMethods.LASTINPUTINFO();
            lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);

            if (NativeMethods.GetLastInputInfo(ref lastInput))
            {
                uint idleTimeMs = (uint)Environment.TickCount - lastInput.dwTime;
                TimeSpan idleTime = TimeSpan.FromMilliseconds(idleTimeMs);

                if (idleTime.TotalMinutes >= Config.IdleMinutes)
                {
                    if (!HasFired) return true;
                }
                else
                {
                    // Reset if user moved mouse or typed
                    Reset();
                }
            }
            return false;
        }
    }
}