using SmartRoutines.UI.Core.Theme;
using SmartRoutines.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_LiveConsole : SmartUserControl
    {
        private int _activeRoutines = 0;
        private bool _engineActive = false;

        public UC_LiveConsole()
        {
            InitializeComponent();

            // header
            pnlConsoleHeader.BackColor = SmartTheme.Surface;
            lblConsoleTitle.Text = ">_  Live Developer Console"; // Exact Figma Text
            lblConsoleTitle.Font = SmartTheme.FontBodyBold;
            lblConsoleTitle.ForeColor = SmartTheme.TextPrimary;

            // indicator circles
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

            // terminal / rich text
            richConsole.BackColor = SmartTheme.Background;
            richConsole.ForeColor = SmartTheme.Success;
            richConsole.Font = SmartTheme.FontMono;
            richConsole.BorderStyle = BorderStyle.None;
            richConsole.ReadOnly = true;
            richConsole.ScrollBars = RichTextBoxScrollBars.Vertical;

            // command textbox
            txtCommand.PlaceholderText = "> Enter command...";
            txtCommand.PlaceholderForeColor = SmartTheme.Success;
            txtCommand.FillColor = SmartTheme.SidebarBg;
            txtCommand.BackColor = SmartTheme.Background;
            txtCommand.ForeColor = SmartTheme.Success;
            txtCommand.Font = SmartTheme.FontMono;
            txtCommand.BorderRadius = 0;
            txtCommand.BorderColor = SmartTheme.Border;

            // events
            txtCommand.KeyDown += TxtCommand_KeyDown;
            //this.Load += UC_LiveConsole_Load;
        }

        private void UC_LiveConsole_Load(object? sender, EventArgs e)
        {
            
            AppendRaw("> System initialized", LogStatus.Success);
            AppendRaw("> Automation engine v2.1.0", LogStatus.Success);
           
            // make sure caret is visible after initial lines
            richConsole.SelectionStart = richConsole.TextLength;
            richConsole.ScrollToCaret();
        }

        // public API same as before but keep an overload that doesn't add timestamp
        public void AppendLine(string message, LogStatus status)
        {
            Action appendAction = () =>
            {
                // timestamp + colored message
                Color lineColor = status switch
                {
                    LogStatus.Success => SmartTheme.Success,
                    LogStatus.Error => SmartTheme.Danger,
                    LogStatus.Warning => SmartTheme.Warning,
                    _ => SmartTheme.TextSecondary
                };

                // timestamp (muted)
                richConsole.SelectionColor = Color.FromArgb(120, SmartTheme.TextMuted);
                richConsole.AppendText($"[{DateTime.Now:HH:mm:ss}] ");

                // message
                richConsole.SelectionColor = lineColor;
                richConsole.AppendText(message + Environment.NewLine);

                richConsole.SelectionColor = SmartTheme.Success;
                richConsole.ScrollToCaret();
            };
        }

        // append a line exactly as provided (no timestamp) — used to mimic design's green lines and prompt style
        public void AppendRaw(string message, LogStatus status = LogStatus.Success)
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

                richConsole.SelectionColor = lineColor;
                richConsole.AppendText(message + Environment.NewLine);
                richConsole.ScrollToCaret();
            }));
        }

        private void TxtCommand_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                var cmd = txtCommand.Text?.Trim();
                if (!string.IsNullOrEmpty(cmd))
                {
                    // echo the command (styled like other console lines)
                    AppendRaw($"> {cmd}", LogStatus.Success);

                    // simple execution simulation — replace with real handler/integration
                    ExecuteCommand(cmd);

                    txtCommand.Clear();
                }
            }
        }

        // simple command handler example — integrate with real engine as needed
        private void ExecuteCommand(string cmd)
        {
            // add executed confirmation
            AppendRaw($"Executed: {cmd}", LogStatus.Success);

            // demo: allow simple commands to influence console/engine state
            var lower = cmd.ToLowerInvariant();
            if (lower.StartsWith("start"))
            {
                // start increases active routines by 1 and ensures engine active
                SetEngineStatus(true, Math.Max(1, _activeRoutines + 1));
            }
            else if (lower.StartsWith("stop"))
            {
                // reduce active routines, deactivate if zero
                int newCount = Math.Max(0, _activeRoutines - 1);
                SetEngineStatus(newCount > 0, newCount);
            }
            else if (lower.StartsWith("status"))
            {
                AppendRaw($"> Engine active: {_engineActive}, active routines: {_activeRoutines}", LogStatus.Success);
            }
            // else: other commands — can be routed to backend
        }

        // control the green indicator and add a monitoring line
        public void SetEngineStatus(bool active, int activeRoutines)
        {
            _engineActive = active;
            _activeRoutines = activeRoutines;

            this.BeginInvoke(new Action(() =>
            {
                // visually toggle indicator (dim when inactive)
                pnlGreen.FillColor = _engineActive ? SmartTheme.Success : Color.FromArgb(90, 90, 90);
                pnlGreen.Invalidate();

                // write monitoring line
                AppendRaw($"> Monitoring {_activeRoutines} active routines", LogStatus.Success);
            }));
        }

        private void richConsole_TextChanged(object sender, EventArgs e)
        {
            // keep caret visible when text changes externally
            richConsole.SelectionStart = richConsole.TextLength;
            richConsole.ScrollToCaret();
        }

        private void pnlRed_Click(object sender, EventArgs e)
        {
            // optional: clear console on red click to mimic window close button UI
            richConsole.Clear();
        }
    }
}
