using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.TriggerMonitors;

namespace SmartRoutines.Logic.AutomationEngine;

/// <summary>
/// Factory responsible for creating concrete ITrigger instances based on TriggerType.
/// Keeps construction logic centralized so new trigger types can be added easily.
/// </summary>
internal static class TriggerFactory
{
    /// <summary>
    /// Create a trigger instance for the provided type. Caller is responsible for calling Configure(json).
    /// Returns null if the trigger type is not supported.
    /// </summary>
    public static ITrigger? Create(TriggerType type)
        => type switch
        {
            TriggerType.Time => new TimeTrigger(),
            TriggerType.WiFi => new WiFiTrigger(),
            TriggerType.Battery => new BatteryTrigger(),
            TriggerType.Startup => new StartupTrigger(),
            TriggerType.Shutdown => new ShutdownTrigger(),
            TriggerType.Idle => new IdleTrigger(),
            TriggerType.AppLaunched => new AppLaunchedTrigger(),
            TriggerType.FileChanged => new FileChangedTrigger(),
            _ => null
        };
}
