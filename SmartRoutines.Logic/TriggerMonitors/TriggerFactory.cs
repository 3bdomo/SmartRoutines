using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public static class TriggerFactory
    {
        public static ITrigger Create(TriggerType type)
        {
            return type switch
            {
                TriggerType.Time => new TimeTrigger(),
                TriggerType.Startup => new StartupTrigger(),
                TriggerType.Shutdown => new ShutdownTrigger(),
              TriggerType.Idle => new IdleTrigger(),
              TriggerType.Battery => new BatteryTrigger(),
              TriggerType.WiFi => new WiFiTrigger(),
                TriggerType.AppLaunched => new AppLaunchedTrigger(),
                TriggerType.FileChanged => new FileChangedTrigger(),
                _ => throw new NotSupportedException($"Trigger {type} not supported")
            };
        }
    }
}
