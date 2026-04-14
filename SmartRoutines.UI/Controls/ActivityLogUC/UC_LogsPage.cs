using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_LogsPage : SmartUserControl

    {
        //private List<ActivityLog> _allLogs = new();

        private UC_ExecutionHistory _ucHistory;
        public UC_LogsPage()
        {
            InitializeComponent();

            // Hide designer placeholders to rebuild cleanly
            pnlHeader.Visible = false;
            pnlDivider.Visible = false;
            pnlLogsArea.Visible = false;
            ucConsole.Visible = false;

            this.Dock = DockStyle.Fill;
            this.BackColor = SmartTheme.Background;
            this.Padding = new Padding(26);

            BuildLayout();
        }

        private void BuildLayout()
        {
            // ── Top Header Section ──
            var headerPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent
            };

            var picLogo = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("activity.png", 32),
                Size = new Size(32, 32),
                Location = new Point(0, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(picLogo);

            lblTitle.Text = "Activity Logs";
            lblTitle.Font = new Font("Segoe UI", 18f, FontStyle.Bold); 
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(picLogo.Right + 12, 10);
            
            var lblSubtitle = new Label
            {
                Text = "Monitor routine executions and system events",
                Font = SmartTheme.FontBody,
                ForeColor = SmartTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(lblTitle.Left, lblTitle.Bottom + 4)
            };

            btnClearLogs.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearLogs.Location = new Point(this.Width - this.Padding.Horizontal - btnClearLogs.Width, 20);
            btnClearLogs.FillColor = SmartTheme.Surface2;
            btnClearLogs.ForeColor = SmartTheme.TextPrimary;
            btnClearLogs.Font = SmartTheme.FontBody;
            btnClearLogs.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("delete.png", 18);
            btnClearLogs.ImageAlign = HorizontalAlignment.Left;
            btnClearLogs.ImageOffset = new Point(4, 0);
            btnClearLogs.TextOffset = new Point(8, 0);
            
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(btnClearLogs);

            // ── Main Body Splitter ──
            var bodyLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); 
            bodyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220f));

            // ── Execution History Main Card ──
            var ucHistoryContainer = new Guna.UI2.WinForms.Guna2Panel
            {
                Dock = DockStyle.Fill,
                BorderRadius = 12,
                FillColor = SmartTheme.Surface, // Correct Figma elevated color
                BorderColor = SmartTheme.Border,
                BorderThickness = 1,
                Margin = new Padding(0, 0, 0, 16) // Spacing below history before console
            };

            _ucHistory = new UC_ExecutionHistory { Dock = DockStyle.Fill };
            ucHistoryContainer.Controls.Add(_ucHistory);

            // ── Live Developer Console Card ──
            var liveConsoleContainer = new Guna.UI2.WinForms.Guna2Panel
            {
                Dock = DockStyle.Fill,
                BorderRadius = 12,
                FillColor = SmartTheme.Surface, // Correct Figma elevated color
                BorderColor = SmartTheme.Border,
                BorderThickness = 1,
                Margin = new Padding(0)
            };

            var liveConsole = new UC_LiveConsole { Dock = DockStyle.Fill };
            liveConsoleContainer.Controls.Add(liveConsole);

            // Add to layout
            bodyLayout.Controls.Add(ucHistoryContainer, 0, 0);
            bodyLayout.Controls.Add(liveConsoleContainer, 0, 1);

            // ── Assembly ──
            this.Controls.Add(bodyLayout);
            this.Controls.Add(headerPanel);

            // Send to back so Dock.Top layouts properly before Dock.Fill
            headerPanel.SendToBack();

            // ── Seed Mock Data ──
            var allLogs = new System.Collections.Generic.List<SmartRoutines.Core.Domain.Entities.ActivityLog>
            {
                new SmartRoutines.Core.Domain.Entities.ActivityLog(System.Guid.NewGuid(), "Morning Setup",
                    SmartRoutines.Core.Domain.Enums.LogStatus.Success, "All actions completed in 2.3s"),
                new SmartRoutines.Core.Domain.Entities.ActivityLog(System.Guid.NewGuid(), "Focus Mode",
                    SmartRoutines.Core.Domain.Enums.LogStatus.Success, "All actions completed in 1.1s"),
                new SmartRoutines.Core.Domain.Entities.ActivityLog(System.Guid.NewGuid(), "Evening Shutdown",
                    SmartRoutines.Core.Domain.Enums.LogStatus.Error, "Error: backup.bat not found")
            };

            _ucHistory.LoadLogs(allLogs);

            liveConsole.AppendLine("System initialized", SmartRoutines.Core.Domain.Enums.LogStatus.Success);
            liveConsole.AppendLine("Automation engine v2.1.0", SmartRoutines.Core.Domain.Enums.LogStatus.Success);
            liveConsole.AppendLine("Monitoring 3 active routines", SmartRoutines.Core.Domain.Enums.LogStatus.Success);
        }


        //        _allLogs = new List<ActivityLog>
        //{
        //    new ActivityLog(Guid.NewGuid(), "Morning Setup",
        //        LogStatus.Success, "All actions completed in 2.3s"),
        //     new ActivityLog(Guid.NewGuid(), "Morning Setup",
        //        LogStatus.Success, "All actions completed in 2.3s"),
        //      new ActivityLog(Guid.NewGuid(), "Morning Setup",
        //        LogStatus.Success, "All actions completed in 2.3s"),
        //       new ActivityLog(Guid.NewGuid(), "Morning Setup",
        //        LogStatus.Success, "All actions completed in 2.3s"),
        //        new ActivityLog(Guid.NewGuid(), "Morning Setup",
        //        LogStatus.Success, "All actions completed in 2.3s"),
        //         new ActivityLog(Guid.NewGuid(), "Morning Setup",
        //        LogStatus.Success, "All actions completed in 2.3s"),
        //    new ActivityLog(Guid.NewGuid(), "Evening Shutdown",
        //        LogStatus.Error, "Error: backup.bat not found"),
        //    new ActivityLog(Guid.NewGuid(), "Focus Mode",
        //        LogStatus.Warning, "Some actions completed with warnings")
        //};

        //        _ucHistory.LoadLogs(_allLogs);
        //    }

    }
}

