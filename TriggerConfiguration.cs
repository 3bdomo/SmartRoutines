using System;
using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Core.Domain.Models
{
    /// <summary>
    /// Holds the configuration values for various trigger types.
    /// This object is serialized to JSON and saved in the database.
    /// </summary>
    public class TriggerConfiguration
    {
        // For TimeTrigger
        public DateTime ScheduledTime { get; set; }
        public DayOfWeek RepeatDays { get; set; }

        // For BatteryTrigger
        public int BatteryThreshold { get; set; }

        // For IdleTrigger
        public int IdleMinutes { get; set; }

        // For WiFiTrigger
        public string SsidName { get; set; }
    }
}