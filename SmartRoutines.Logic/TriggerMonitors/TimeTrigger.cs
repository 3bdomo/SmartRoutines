using SmartRoutines.Core.Domain.Models;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class TimeTrigger : BaseTrigger<TriggerConfiguration>
    {
        private DateTime _lastResetDate = DateTime.MinValue;

        public override string DisplayName => $"Scheduled at {Config?.ScheduledTime:HH:mm}";

        public override bool ShouldFire()
        {
            if (!IsEnabled || Config == null) return false;

            // Reset flag for a new day
            if (DateTime.Today > _lastResetDate)
            {
                Reset();
                _lastResetDate = DateTime.Today;
            }

            if (HasFired) return false;

            var todayFlag = GetCurrentDayFlag();

            // If RepeatDays doesn't contain today's flag, don't fire
            if (!Config.RepeatDays.HasFlag(todayFlag)) return false;

            var now = DateTime.Now.TimeOfDay;
            var scheduled = Config.ScheduledTime.TimeOfDay;

            return now >= scheduled;
        }

        private Core.Domain.Enums.DayOfWeek GetCurrentDayFlag()
        {
            return DateTime.Today.DayOfWeek switch
            {
                System.DayOfWeek.Monday => Core.Domain.Enums.DayOfWeek.Mon,
                System.DayOfWeek.Tuesday => Core.Domain.Enums.DayOfWeek.Tue,
                System.DayOfWeek.Wednesday => Core.Domain.Enums.DayOfWeek.Wed,
                System.DayOfWeek.Thursday => Core.Domain.Enums.DayOfWeek.Thu,
                System.DayOfWeek.Friday => Core.Domain.Enums.DayOfWeek.Fri,
                System.DayOfWeek.Saturday => Core.Domain.Enums.DayOfWeek.Sat,
                System.DayOfWeek.Sunday => Core.Domain.Enums.DayOfWeek.Sun,
                _ => Core.Domain.Enums.DayOfWeek.None
            };
        }
    }
}
