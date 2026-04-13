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

            pnlHeader.BackColor = SmartTheme.Surface;
            lblTitle.Text = "Activity Log";
            lblTitle.Font = SmartTheme.FontSubheader;
            lblTitle.ForeColor = SmartTheme.TextPrimary;

            // ── Clear Button ──

            btnClearLogs.Font = SmartTheme.FontBody;
            btnClearLogs.ForeColor = SmartTheme.Danger;
            btnClearLogs.BorderRadius = 8;

            // ✅ Outlined = خلفية شفافة + بوردر أحمر
            btnClearLogs.FillColor = SmartTheme.DangerMuted;
            btnClearLogs.CustomBorderColor = SmartTheme.Danger;
            btnClearLogs.CustomBorderThickness = new Padding(1);




            // ── Divider ──
            pnlDivider.BackColor = SmartTheme.Border;

            // ── Load ──

            _ucHistory = new UC_ExecutionHistory();
            _ucHistory.Dock = DockStyle.Fill;
            pnlLogsArea.Controls.Add(_ucHistory);
            //_ucHistory.LoadLogs(_allLogs);
        }
        //    protected override void OnLoad(EventArgs e)
        //    {
        //        base.OnLoad(e);


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

