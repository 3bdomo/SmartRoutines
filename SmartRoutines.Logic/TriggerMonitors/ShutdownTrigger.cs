using System;
using System.Windows.Forms;
using SmartRoutines.Core.Interfaces.Logic;


namespace SmartRoutines.Logic.TriggerMonitors
{
    public class ShutdownTrigger : BaseTrigger
    {
        private bool _isShuttingDown = false;

        public override string DisplayName => "At System Shutdown";

        public override void Configure(string json)
        {
            Microsoft.Win32.SystemEvents.SessionEnding += OnSessionEnding;
        }

        private void OnSessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            _isShuttingDown = true;
        }

        public override bool ShouldFire()
        {
            return IsEnabled && _isShuttingDown && !HasFired;
        }

        public override void Dispose()
        {
            Microsoft.Win32.SystemEvents.SessionEnding -= OnSessionEnding;
            base.Dispose();
        }
    }
}