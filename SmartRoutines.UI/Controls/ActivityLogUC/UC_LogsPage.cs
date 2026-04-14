using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_LogsPage : SmartUserControl

    {

        private UC_ExecutionHistory _ucHistory;
        private List<ActivityLog> _allLogs;

        public UC_LogsPage()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            this.SuspendLayout();
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
            // pnlDivider.BackColor = SmartTheme.Border;

            // ── Load ──


        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);



            _ucHistory = new UC_ExecutionHistory();
            _ucHistory.Dock = DockStyle.Top;
            _ucHistory.AutoSize = false;
            pnlLogsArea.Controls.Add(_ucHistory);

            _allLogs = new List<ActivityLog>
            {
                new ActivityLog(Guid.NewGuid(), "Morning Setup",
                    LogStatus.Success, "All actions completed in 2.3s"),
                 new ActivityLog(Guid.NewGuid(), "Morning Setup",
                    LogStatus.Success, "All actions completed in 2.3s"),
                  new ActivityLog(Guid.NewGuid(), "Morning Setup",
                    LogStatus.Success, "All actions completed in 2.3s"),
                   new ActivityLog(Guid.NewGuid(), "Morning Setup",
                    LogStatus.Success, "All actions completed in 2.3s"),
                    new ActivityLog(Guid.NewGuid(), "Morning Setup",
                    LogStatus.Success, "All actions completed in 2.3s"),
                     new ActivityLog(Guid.NewGuid(), "Morning Setup",
                    LogStatus.Success, "All actions completed in 2.3s"),
                     new ActivityLog(Guid.NewGuid(), "Evening Shutdown",
                    LogStatus.Error, "Error: backup.bat not found"),
                new ActivityLog(Guid.NewGuid(), "Focus Mode",
                    LogStatus.Warning, "Some actions completed with warnings")
            };

            _ucHistory.LoadLogs(_allLogs);
        }
       // private readonly ILogRepository _logRepository;
        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            //var result = MessageBox.Show("Are you sure you want to delete all logs permanently?",
            //                   "Confirm Delete",
            //                   MessageBoxButtons.YesNo,
            //                   MessageBoxIcon.Warning);

            //if (result == DialogResult.Yes)
            //{
            //    try
            //    {
            //        // 1. المسح من قاعدة البيانات/المخزن
            //       // await _logRepository.;

            //        // 2. تحديث القائمة المحلية
            //        _allLogs.Clear();

            //        // 3. تحديث الواجهة (الـ UserControl اللي عملناه)
            //        if (_ucHistory != null)
            //        {
            //            _ucHistory.ClearAllLogs();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Error clearing logs: {ex.Message}");
            //    }
            //}
        }
    }
}
