namespace SmartRoutines.Core.Domain.Models
{
    public class TriggerConfiguration
    {
        public DateTime ScheduledTime { get; set; }
        public SmartRoutines.Core.Domain.Enums.DayOfWeek RepeatDays { get; set; }
        public int BatteryThreshold { get; set; }
        public int IdleMinutes { get; set; }
        public string SsidName { get; set; }
    }
}