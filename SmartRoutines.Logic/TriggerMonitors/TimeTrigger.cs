using SmartRoutines.Core.Domain.Models;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class TimeTrigger : BaseTrigger<TriggerConfiguration>
    {
        private DateTime _lastResetDate = DateTime.Today;
        private bool _isInitialized = false;
        private bool _wasSkippedAtStartup = false;

        public override string DisplayName => $"Scheduled at {Config?.ScheduledTime:HH:mm}";

        public override Task<bool> ShouldFireAsync()
        {
            if (!IsEnabled || Config == null) return Task.FromResult(false);

            // Handle initialization: If we just started and the time for today has already passed,
            // we mark it as fired so it doesn't "catch up" immediately.
            // We allow a 1-minute grace period so entered current-time triggers can fire.
            if (!_isInitialized)
            {
                var nowTime = DateTime.Now.TimeOfDay;
                var schedTime = Config.ScheduledTime.TimeOfDay;
                var gracePeriod = TimeSpan.FromMinutes(1);

                if (nowTime > schedTime.Add(gracePeriod))
                {
                    HasFired = true;
                    _wasSkippedAtStartup = true;
                }
                _isInitialized = true;
            }

            // Reset flag for a new day
            if (DateTime.Today > _lastResetDate)
            {
                Reset();
                _lastResetDate = DateTime.Today;
                _isInitialized = true; // Still initialized, just new day
            }

            var todayFlag = GetCurrentDayFlag();

            // If RepeatDays doesn't contain today's flag, don't fire
            if (!Config.RepeatDays.HasFlag(todayFlag)) return Task.FromResult(false);

            var now = DateTime.Now.TimeOfDay;
            var scheduled = Config.ScheduledTime.TimeOfDay;

            // Fire if we are within 1 minute of the scheduled time
            // This ensures precision while allowing for small engine heartbeat fluctuations
            return Task.FromResult(now >= scheduled && now < scheduled.Add(TimeSpan.FromMinutes(1)));
        }

        public override string GetDiagnosticInfo()
        {
            if (!IsEnabled) return "Disabled";
            if (Config == null) return "Invalid Configuration";

            if (HasFired) 
            {
                return _wasSkippedAtStartup ? "Already passed for today" : "Fired today";
            }
            
            var todayFlag = GetCurrentDayFlag();
            if (!Config!.RepeatDays.HasFlag(todayFlag)) return "Not scheduled for today";

            var now = DateTime.Now.TimeOfDay;
            var scheduled = Config.ScheduledTime.TimeOfDay;

            // Diagnostic: Show if we are inside the firing window
            if (now >= scheduled && now < scheduled.Add(TimeSpan.FromMinutes(1)))
            {
                return "🔥 FIRING NOW!";
            }
            
            return $"Scheduled for {Config.ScheduledTime:HH:mm}";
        }

        public override void Reset()
        {
            base.Reset();
            _isInitialized = false;
            _wasSkippedAtStartup = false;
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
