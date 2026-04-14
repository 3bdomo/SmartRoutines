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
            lblConsoleTitle.Text = ">_  Live Developer Console"; // Exact Figma Text
            lblConsoleTitle.Font = SmartTheme.FontBodyBold;
            lblConsoleTitle.ForeColor = SmartTheme.TextPrimary;

            //cercile color
            pnlRed.FillColor = SmartTheme.Danger;
            pnlYellow.FillColor = SmartTheme.Warning;
            pnlGreen.FillColor = SmartTheme.Success;

            // Anchor circles securely to the Absolute Right tracking Figma's positioning
            pnlRed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlYellow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlGreen.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Calculate strict starting positions from right edge boundary bypassing designer
            int farRight = pnlConsoleHeader.Width - 20;
            int dotSize = pnlGreen.Width > 0 ? pnlGreen.Width : 12;
            int midY = (pnlConsoleHeader.Height - dotSize) / 2;

            pnlGreen.Location = new Point(farRight - dotSize, midY);
            pnlYellow.Location = new Point(pnlGreen.Left - dotSize - 8, midY); // 8px gap
            pnlRed.Location = new Point(pnlYellow.Left - dotSize - 8, midY);

            //terminal
            richConsole.BackColor = SmartTheme.Surface;
            richConsole.ForeColor = SmartTheme.Success;
            richConsole.Font = SmartTheme.FontMono;
            richConsole.BorderStyle = BorderStyle.None;
            richConsole.ReadOnly = true;
            
            txtCommand.PlaceholderText = "> Enter command...";
            txtCommand.PlaceholderForeColor = SmartTheme.Success;
            txtCommand.FillColor = SmartTheme.SurfaceDeep; // Match Figma nested dark input box
            txtCommand.BackColor = SmartTheme.Surface;

            txtCommand.ForeColor = SmartTheme.Success;
            txtCommand.Font = SmartTheme.FontMono;
            txtCommand.BorderRadius = 0;
            txtCommand.BorderColor = SmartTheme.Border;
            
            // Native structural layout locks bypassing static designer
            pnlConsoleHeader.Dock = DockStyle.Top;
            txtCommand.Dock = DockStyle.Bottom;
            richConsole.Dock = DockStyle.Fill;
            
            // Fix WinForms Rendering Clipping
            pnlConsoleHeader.SendToBack();
            txtCommand.SendToBack();
            richConsole.BringToFront(); 

        }
        public void AppendLine(string message, LogStatus status)
        {
            Action appendAction = () =>
            {
                Color lineColor = status switch
                {
                    LogStatus.Success => SmartTheme.Success,
                    LogStatus.Error => SmartTheme.Danger,
                    LogStatus.Warning => SmartTheme.Warning,
                    _ => SmartTheme.TextSecondary
                };

                // Match exact Figma layout "> text" without timestamp
                richConsole.SelectionColor = SmartTheme.Success;
                richConsole.AppendText("> ");
                richConsole.SelectionColor = lineColor;
                richConsole.AppendText(message + "\n");
                richConsole.ScrollToCaret();
            };

            // Safely bypass cross-thread enforcement if we are natively executing on the constructor thread
            if (this.InvokeRequired)
            {
                if (this.IsHandleCreated) this.BeginInvoke(appendAction);
            }
            else
            {
                appendAction();
            }
        }
       
        private void richConsole_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
