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
           
                pnlHeader.BackColor = SmartTheme.Surface;
            
            
                lblTitle.Text = "Activity Log";
                lblTitle.Font = SmartTheme.FontSubheader;
                lblTitle.ForeColor = SmartTheme.TextPrimary;
            
            // Configure clear button if present
           
                btnClearLogs.Font = SmartTheme.FontBody;
                btnClearLogs.ForeColor = SmartTheme.Danger;
                btnClearLogs.BorderRadius = 8;
                btnClearLogs.FillColor = SmartTheme.DangerMuted;
                btnClearLogs.CustomBorderColor = SmartTheme.Danger;
                btnClearLogs.CustomBorderThickness = new Padding(1);
               
            
            // Make header controls responsive
            btnClearLogs.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.Resize += UC_LogsPage_Resize;
            // perform an initial layout pass
            AdjustResponsiveLayout();
            // Subscribe to service events only if we have a service instance
            //if (_logService != null)
            //{
            //   // _logService.OnLogAdded += LogService_OnLogAdded;
            //   // _logService.OnLogsCleared += LogService_OnLogsCleared;
            //}
        }
        private void UC_LogsPage_Resize(object? sender, EventArgs e) => AdjustResponsiveLayout();

        /// <summary>
        /// Adjusts sizes/font-sizes of header controls and child areas to be responsive to the page width.
        /// Keep the adjustments inexpensive as Resize fires frequently.
        /// </summary>
        private void AdjustResponsiveLayout()
        {
            if (this.IsDisposed) return;

            // Scale factor: 1200 is baseline width from design -> clamp to reasonable range
            double scale = Math.Clamp(this.Width / 900.0, 0.55, 1.45);

            // Button width: proportion of width but capped
            btnClearLogs.Width = Math.Min((int)(this.Width * 0.18), 420);

            try
            {
                lblTitle.Font = new Font(SmartTheme.FontSubheader.FontFamily,
                                         (float)(SmartTheme.FontSubheader.Size * scale),
                                         SmartTheme.FontSubheader.Style);

                btnClearLogs.Font = new Font(SmartTheme.FontBody.FontFamily,
                                             (float)(SmartTheme.FontBody.Size * Math.Max(0.45, scale)),
                                             SmartTheme.FontBody.Style);
            }
            catch
            {
                // If theme fonts are not available or invalid, ignore scaling (safe fallback).
            }

            // Ensure the execution history control width matches available area
            if (_ucHistory != null && !_ucHistory.IsDisposed)
            {
                _ucHistory.Width = Math.Max(300, pnlLogsArea.ClientSize.Width - 8);
            }
            if (ucConsole != null)
            {
                float consoleBase = 9f;
                ucConsole.Font = new Font(ucConsole.Font.FontFamily, Math.Max(8f, consoleBase * (float)scale));
            }
        }

        // Let
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
                Dock = DockStyle.Fill,    // fill available area instead of Top
                AutoSize = false
            };

            // Defer adding and loading until after first layout pass so sizes are valid.
            this.BeginInvoke(new Action(async () =>
            {
                pnlLogsArea.Controls.Add(_ucHistory);

                //// If no service (design / preview), show sample logs.
                //if (_logService == null)
                //{
                    var sample = new List<ActivityLog>
                    {
                        new ActivityLog(Guid.NewGuid(), "Morning Setup", LogStatus.Success, "All actions completed in 2.3s"),
                        new ActivityLog(Guid.NewGuid(), "Evening Shutdown", LogStatus.Error, "Failed to stop backup service"),
                        new ActivityLog(Guid.NewGuid(), "Focus Mode", LogStatus.Warning, "Some actions completed with warnings")
                    };

                    _ucHistory.LoadLogs(sample);
                    _ucHistory.BringToFront();
                    return;
               // }

                // Runtime: load persisted logs
                //try
                //{
                //    var (dtos, total) = await _logService.GetPagedLogsAsync(1, 50);
                //    if (dtos != null && dtos.Count > 0)
                //        _ucHistory.LoadLogsFromDtos(dtos.ToList());
                //    else
                //        _ucHistory.ClearAllLogs();
                //}
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Debug.WriteLine($"LoadLogsAsync error: {ex.Message}");
                //    _ucHistory.ClearAllLogs();
                //}
            }));
        }

        //private async void LoadLogsAsync()
        //{
        //    try
        //    {
        //        if (_logService == null) return;

        //        var (logs, totalCount) = await _logService.GetPagedLogsAsync(page: 1, pageSize: 10);

        //        var logEntities = logs.Select(dto => new ActivityLog(
        //            Guid.Empty,
        //            dto.RoutineName,
        //            dto.Status,
        //            dto.Message
        //        )).ToList();

        //        _ucHistory.LoadLogs(logEntities);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

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

        //private void LogService_OnLogAdded(object sender, ActivityLogEventArgs e)
        //{
        //    // Ensure UI exists before updating it
        //    if (this.IsDisposed) return;
        //    if (_ucHistory == null) return;

        //    this.BeginInvoke(new Action(() =>
        //    {
        //        if (_ucHistory == null) return;

        //        var logEntity = new ActivityLog(
        //            Guid.Empty,
        //            e.Log.RoutineName,
        //            e.Log.Status,
        //            e.Log.Message
        //        );

        //        _ucHistory.AddLogToTop(logEntity);
        //    }));
        //}

        //private void LogService_OnLogsCleared(object sender, EventArgs e)
        //{
        //    if (this.IsDisposed) return;
        //    if (_ucHistory == null) return;

        //    this.BeginInvoke(new Action(() =>
        //    {
        //        _ucHistory.ClearAllLogs();
        //    }));
        //}

        // Unsubscribe safely when the control is destroyed
        //protected override void OnHandleDestroyed(EventArgs e)
        //{
        //    if (_logService != null)
        //    {
        //      //  _logService.OnLogAdded -= LogService_OnLogAdded;
        //       // _logService.OnLogsCleared -= LogService_OnLogsCleared;
        //    }
        //    base.OnHandleDestroyed(e);
        //}
    }
}
