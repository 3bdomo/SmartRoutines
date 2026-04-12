using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.UI.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_LiveConsole : SmartUserControl
    {
        public UC_LiveConsole()
        {
            InitializeComponent();
            //header
            pnlConsoleHeader.BackColor = SmartTheme.Surface;
            lblConsoleTitle.Text = "Live Developer Console";
            lblConsoleTitle.Font = SmartTheme.FontBodyBold;
            lblConsoleTitle.ForeColor = SmartTheme.TextPrimary;

            //cercile color
            pnlRed.FillColor = SmartTheme.Danger;
            pnlYellow.FillColor = SmartTheme.Warning;
            pnlGreen.FillColor = SmartTheme.Success;

            //terminal
            richConsole.BackColor = SmartTheme.Background;
            richConsole.ForeColor = SmartTheme.Success;
            richConsole.Font = SmartTheme.FontMono;
            richConsole.BorderStyle = BorderStyle.None;
            // richConsole.ReadOnly = true;
            richConsole.ReadOnly = true;
            txtCommand.PlaceholderText = "> Enter command...";
            txtCommand.PlaceholderForeColor = SmartTheme.Success;
            txtCommand.FillColor = SmartTheme.SidebarBg;

            txtCommand.BackColor = SmartTheme.Background;
            txtCommand.ForeColor = SmartTheme.Success;
            txtCommand.Font = SmartTheme.FontMono;
            txtCommand.BorderRadius = 0;
             txtCommand.BorderColor = SmartTheme.Border;
            //txtCommand.BorderColor = SmartTheme.Border;
            

        }
        public void AppendLine(string message, LogStatus status)
        {
            this.BeginInvoke(new Action(() =>
            {
                Color lineColor = status switch
                {
                    LogStatus.Success => SmartTheme.Success,
                    LogStatus.Error => SmartTheme.Danger,
                    LogStatus.Warning => SmartTheme.Warning,
                    _ => SmartTheme.TextSecondary
                };

                richConsole.SelectionColor = Color.FromArgb(100, 255, 255, 255);
                richConsole.AppendText($"[{DateTime.Now:HH:mm:ss}] ");
                richConsole.SelectionColor = lineColor;
                richConsole.AppendText(message + "\n");
                richConsole.ScrollToCaret();
            }));
        }
       
        private void richConsole_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
