namespace SmartRoutines.Core.Models;

public class ActionContext
{
    // The name of the routine currently running
    public string RoutineName { get; set; }

    // The timestamp when the trigger condition was met
    public DateTime TriggerTime { get; set; }

    // Any specific data provided by the trigger (e.g., Battery level, SSID)
    public Dictionary<string, object>? TriggerData { get; set; }

    // A flag to indicate if the user started this manually (UC-01)
    public bool IsManualTrigger { get; set; }
}