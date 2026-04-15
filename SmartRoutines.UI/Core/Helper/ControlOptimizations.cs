using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SmartRoutines.UI.Core.Helper
{
    public static class ControlOptimizations
    {
        public static void EnableDoubleBuffered(Control control)
        {
            if (control is null) return;

            try
            {
                PropertyInfo? cp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                cp?.SetValue(control, true, null);
            }
            catch { }

            foreach (Control child in control.Controls)
            {
                EnableDoubleBuffered(child);
            }
        }

        public const int WS_EX_COMPOSITED = 0x02000000;

        /// <summary>
        /// Applies double buffering recursively. (Note: WS_EX_COMPOSITED is handled via CreateParams in the Form)
        /// </summary>
        public static void ApplyModernRendering(Control control)
        {
            EnableDoubleBuffered(control);
        }

        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
