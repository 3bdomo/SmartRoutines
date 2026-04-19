using System;
using System.Runtime.InteropServices;

namespace SmartRoutines.Logic.Helpers
{
    /// <summary>
    /// Centralized P/Invoke declarations for system monitoring.
    /// </summary>
    public static class NativeMethods
    {
        // ── Battery / Power Status ───────────────────────────────────────────
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemPowerStatus(out SystemPowerStatus sps);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemPowerStatus
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte SystemStatusFlag;
            public uint BatteryLifeTime;
            public uint BatteryFullLifeTime;
        }

        // ── Idle / User Input Status ─────────────────────────────────────────
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }
}
