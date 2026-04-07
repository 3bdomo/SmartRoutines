using SmartRoutines.Core.Interfaces.Logic;
using System.Text.Json;

namespace SmartRoutines.Logic.TriggerMonitors
{
    public class TimeTrigger : ITrigger
    {
        private TimeSpan _targetTime;
        private bool _hasTriggeredToday;

        public bool IsSatisfied
        {
            get
            {
                var now = DateTime.Now.TimeOfDay;
                // If it's the exact minute and we haven't triggered today
                if (now.Hours == _targetTime.Hours && now.Minutes == _targetTime.Minutes)
                {
                    if (!_hasTriggeredToday)
                    {
                        _hasTriggeredToday = true;
                        return true;
                    }
                }
                else
                {
                    _hasTriggeredToday = false; // Reset for next day
                }
                return false;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Initialize(string configJson)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(configJson);
                if (dict != null && dict.TryGetValue("Time", out var timeStr))
                {
                    _targetTime = TimeSpan.Parse(timeStr);
                }
            }
            catch
            {
                _targetTime = TimeSpan.Zero;
            }
        }
    }
}
