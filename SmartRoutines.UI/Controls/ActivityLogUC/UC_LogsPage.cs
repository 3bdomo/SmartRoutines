using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_LogsPage : SmartUserControl
    {
        private UC_ExecutionHistory _ucHistory = null!;
        private readonly IActivityLogService? _logService;

        // Parameterless ctor required by designer / DisplayPage<T>() usage
        public UC_LogsPage()
        {
            InitializeComponent();
            CommonInit();
        }

        // DI ctor used when resolving via IServiceProvider
        public UC_LogsPage(IActivityLogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            InitializeComponent();
            CommonInit();
        }

        // Shared initialization executed after InitializeComponent()
        private void CommonInit()
        {
            // Visual styles / performance
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            // Header visuals (protect against designer being incomplete)
            if (pnlHeader != null)
            {
                pnlHeader.BackColor = SmartTheme.Surface;
            }
            if (lblTitle != null)
            {
                lblTitle.Text = "Activity Log";
                lblTitle.Font = SmartTheme.FontSubheader;
                lblTitle.ForeColor = SmartTheme.TextPrimary;
            }

            // Configure clear button if present
            if (btnClearLogs != null)
            {
                btnClearLogs.Font = SmartTheme.FontBody;
                btnClearLogs.ForeColor = SmartTheme.Danger;
                btnClearLogs.BorderRadius = 8;
                btnClearLogs.FillColor = SmartTheme.DangerMuted;
                btnClearLogs.CustomBorderColor = SmartTheme.Danger;
                btnClearLogs.CustomBorderThickness = new Padding(1);
                btnClearLogs.Click += BtnClearLogs_Click;
            }

            // Subscribe to service events only if we have a service instance
            if (_logService != null)
            {
                _logService.OnLogAdded += LogService_OnLogAdded;
                _logService.OnLogsCleared += LogService_OnLogsCleared;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (pnlLogsArea == null)
            {
                // defensively fail: avoid NRE and provide a visible message in debug
                System.Diagnostics.Debug.WriteLine("UC_LogsPage: pnlLogsArea is null in OnLoad.");
                return;
            }

            _ucHistory = new UC_ExecutionHistory
            {
                Dock = DockStyle.Top,
                AutoSize = false
            };

            pnlLogsArea.Controls.Add(_ucHistory);

            // If no IActivityLogService is provided at runtime, show sample logs so you can preview the UI
            if (_logService == null)
            {
                var sample = new List<ActivityLog>
                {
                    new ActivityLog(Guid.NewGuid(), "Morning Setup", LogStatus.Success, "All actions completed in 2.3s"),
                    new ActivityLog(Guid.NewGuid(), "Evening Shutdown", LogStatus.Error, "Failed to stop backup service"),
                    new ActivityLog(Guid.NewGuid(), "Focus Mode", LogStatus.Warning, "Some actions completed with warnings"),
                      new ActivityLog(Guid.NewGuid(), "Focus Mode", LogStatus.Warning, "Some actions completed with warnings"),
                      new ActivityLog(Guid.NewGuid(), "Focus Mode", LogStatus.Warning, "Some actions completed with warnings")
                };
                _ucHistory.LoadLogs(sample);
                return;
            }

            // otherwise load from service
            LoadLogsAsync();
        }

        private async void LoadLogsAsync()
        {
            try
            {
                if (_logService == null) return;

                var (logs, totalCount) = await _logService.GetPagedLogsAsync(page: 1, pageSize: 10);

                var logEntities = logs.Select(dto => new ActivityLog(
                    Guid.Empty,
                    dto.RoutineName,
                    dto.Status,
                    dto.Message
                )).ToList();

                _ucHistory.LoadLogs(logEntities);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnClearLogs_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete all logs permanently?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_logService != null)
                    {
                        await _logService.ClearAllLogsAsync();
                    }
                    _ucHistory?.ClearAllLogs();
                    MessageBox.Show("All logs have been cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error clearing logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LogService_OnLogAdded(object sender, ActivityLogEventArgs e)
        {
            // Ensure UI exists before updating it
            if (this.IsDisposed) return;
            if (_ucHistory == null) return;

            this.BeginInvoke(new Action(() =>
            {
                if (_ucHistory == null) return;

                var logEntity = new ActivityLog(
                    Guid.Empty,
                    e.Log.RoutineName,
                    e.Log.Status,
                    e.Log.Message
                );

                _ucHistory.AddLogToTop(logEntity);
            }));
        }

        private void LogService_OnLogsCleared(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;
            if (_ucHistory == null) return;

            this.BeginInvoke(new Action(() =>
            {
                _ucHistory.ClearAllLogs();
            }));
        }

        // Unsubscribe safely when the control is destroyed
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_logService != null)
            {
                _logService.OnLogAdded -= LogService_OnLogAdded;
                _logService.OnLogsCleared -= LogService_OnLogsCleared;
            }
            base.OnHandleDestroyed(e);
        }
    }
}
