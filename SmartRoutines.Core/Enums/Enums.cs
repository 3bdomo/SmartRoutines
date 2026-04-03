using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutines.Core.Enums
{
    public enum TriggerType : byte
    {
        Unknown = 0,
        Time = 1,
        Battery = 2,
        WiFi = 3,
        Startup = 4,
        Shutdown = 5,
        Idle = 6,
        AppOpen = 7,
        ProcessClose = 8
    }

    public enum ActionType : byte
    {
        Unknown = 0,
        LaunchApp = 1,
        OpenUrl = 2,
        SetVolume = 3,
        Mute = 4,
        KillProcess = 5,
        Notification = 6,
    }

    public enum LogStatus : byte
    {
        Unknown = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }

    [Flags]
    public enum DayOfWeek : byte
    {
        None = 0,
        Sun = 1,
        Mon = 2,
        Tue = 4,
        Wed = 8,
        Thu = 16,
        Fri = 32,
        Sat = 64
    }
}
