using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutines.UI.Core.Healper
{
    internal static class TooltipHelper
    {
        private static readonly Guna2HtmlToolTip _tip = new Guna2HtmlToolTip();

        // Static Constructor
        static TooltipHelper()
        {
            _tip.BackColor = SmartTheme.Surface;
            _tip.ForeColor = SmartTheme.TextPrimary;
            _tip.BorderColor = SmartTheme.Primary;
            _tip.Font = SmartTheme.FontCaption;

            _tip.AutoPopDelay = 5000;
            _tip.InitialDelay = 500;
            _tip.ReshowDelay = 100;
        }

        // Apply Tooltip
        public static void Set(Control control, string text)
        {
            _tip.SetToolTip(control, text);
        }
    }

}
