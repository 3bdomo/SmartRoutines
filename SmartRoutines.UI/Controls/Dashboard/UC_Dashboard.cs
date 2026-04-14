using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_Dashboard : SmartUserControl
    {
        // Runtime layout controls
        private TableLayoutPanel _statsGrid = null!;
        private TableLayoutPanel _rootLayout = null!;
        private FlowLayoutPanel _flpRoutines = null!;
        private System.Collections.Generic.List<UC_StatCard> _statCards = null!;
        private System.Collections.Generic.List<UC_RoutineCard> _routineList = null!;
        private int _lastAvailableWidth = 0;

        public UC_Dashboard()
        {
            InitializeComponent();

            // Performance: High quality rendering styles
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.UserPaint | 
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            // Remove designer placeholders entirely to fix "white square" artifacts 
            // where hidden designer controls occasionally bleed through z-order.
            this.Controls.Remove(pnlTopHeader);
            this.Controls.Remove(flpStatCards);
            this.Controls.Remove(flpRoutineCards);
            this.Controls.Remove(lblYourRoutines);

            this.Dock = DockStyle.Fill;
            this.BackColor = SmartTheme.Background;

            InitializeDashboard();
        }

        // ─── Initialization ────────────────────────────────────────────────
        private void InitializeDashboard()
        {
            try
            {
                this.SuspendLayout();
                // Removed global recursive call to prevents crashes during initialization
                this.DoubleBuffered = true;

                // ── Root: 4-row TableLayoutPanel ──────────────────────────
                _rootLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 3,
                    ColumnCount = 1,
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };
                _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170f)); // Row 0: stat cards
                _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));  // Row 1: "Your Routines" label
                _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // Row 2: routine cards

                // ── Row 0: Stats grid (4 equal columns, always fills width) ──
                _statsGrid = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 4,
                    Margin = new Padding(0),
                    Padding = new Padding(20, 16, 20, 16),
                    BackColor = Color.Transparent
                };
                _statsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                for (int j = 0; j < 4; j++)
                    _statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

                _statCards = new System.Collections.Generic.List<UC_StatCard>();

                // FIX: colors match Figma exactly
                var accents = new[] {
                    Color.FromArgb(59, 130, 246),   // Blue  – Total Routines
                    Color.FromArgb(34, 197, 94),    // Green – Active Routines
                    Color.FromArgb(139, 92, 246),   // Purple – Running Now
                    Color.FromArgb(243, 156, 18)    // Amber – Total Actions
                };
                var bgs = new[] {
                    Color.FromArgb(15, 23, 42),
                    Color.FromArgb(15, 28, 20),
                    Color.FromArgb(24, 18, 43),
                    Color.FromArgb(30, 20, 10)
                };
                var titles = new[] { "Total Routines", "Active Routines", "Running Now", "Total Actions" };
                var icons = new[] { "dashboard.png", "success.png", "activity.png", "add.png" };

                for (int i = 0; i < 4; i++)
                {
                    var sc = new UC_StatCard
                    {
                        AccentColor = accents[i],
                        CardColor = bgs[i],
                        Title = titles[i],
                        CardIcon = icons[i],
                        Value = "0",
                        Dock = DockStyle.Fill,
                        Margin = new Padding(6),
                        BorderRadius = 20
                    };
                    _statCards.Add(sc);
                    _statsGrid.Controls.Add(sc, i, 0);
                }

                // ── Row 1: "Your Routines" label ─────────────────────────
                lblYourRoutines.Dock = DockStyle.Fill;
                lblYourRoutines.Font = SmartTheme.FontSubheader;
                lblYourRoutines.ForeColor = SmartTheme.TextPrimary;
                lblYourRoutines.Padding = new Padding(24, 0, 0, 0);
                lblYourRoutines.TextAlign = ContentAlignment.MiddleLeft;
                lblYourRoutines.Text = "Your Routines"; // Removed emoji, will rely on clean typography or add separate icon

                // ── Row 2: Routine cards FlowLayoutPanel ───────────
                _flpRoutines = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    WrapContents = true,
                    AutoScroll = true,
                    Padding = new Padding(20, 8, 20, 0),
                    Margin = new Padding(0),
                    BackColor = SmartTheme.Background
                };
                SetDoubleBuffered(_flpRoutines);

                // ── Assemble hierarchy ────────────────────────────────────
                _rootLayout.Controls.Add(_statsGrid, 0, 0);
                _rootLayout.Controls.Add(lblYourRoutines, 0, 1);
                _rootLayout.Controls.Add(_flpRoutines, 0, 2);

                SetDoubleBuffered(_rootLayout);
                SetDoubleBuffered(_statsGrid);
                SetDoubleBuffered(_flpRoutines);

                this.Controls.Add(_rootLayout);
                _rootLayout.BringToFront();

                // ── Seed data ─────────────────────────────────────────────
                _routineList = new System.Collections.Generic.List<UC_RoutineCard>();
                SeedRoutines();

                foreach (var card in _routineList)
                {
                    card.Margin = new Padding(0, 0, 16, 16);
                    card.StateChanged += (s, ev) => RefreshStats();
                }

                // ── Responsive card width on resize ───────────────────────
                _flpRoutines.SizeChanged += (s, ev) => AdjustCardWidths();
                this.SizeChanged += (s, ev) => AdjustCardWidths();

                RenderPage();
                RefreshStats();
                AdjustCardWidths();
                
                // Force a single render cycle
                this.Update();

                this.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard load error: " + ex.Message);
            }
        }

        // ─── Pagination bar ────────────────────────────────────────────────

        // ─── Seed sample data ──────────────────────────────────────────────
        private void SeedRoutines()
        {
            var data = new[]
            {
                ("Morning Setup",    "Launches email, calendar, and sets volume",
                 "Every Mon–Fri at 08:00",    "2 actions configured", "✔ Last run: 11:34:59 PM", true,  false),
                ("Focus Mode",       "Silences notifications and closes distracting apps",
                 "When code.exe launches",    "2 actions configured", "✔ Last run: 12:34:59 AM", true,  true),
                ("Evening Shutdown", "Closes all work apps and backs up files",
                 "Every Mon–Fri at 18:00",   "1 action configured",  "✖ Last run: 12:34:59 AM", false, false),
                ("System Scan",      "Background maintenance operation",
                 "Every Sunday",             "3 actions configured", "Never",                    false, false)
            };

            foreach (var (name, desc, sched, actions, lastRun, active, running) in data)
            {
                var card = new UC_RoutineCard
                {
                    RoutineName = name,
                    Description = desc,
                    ScheduleText = sched,
                    ActionCountText = actions,
                    LastRunText = lastRun,
                    IsActive = active,
                    IsRunning = running,
                    Anchor = AnchorStyles.None
                };
                _routineList.Add(card);
            }
        }

        // ─── Public API: add new routine ───────────────────────────────────
        public void AddNewRoutine()
        {
            var card = new UC_RoutineCard
            {
                RoutineName = "New Routine",
                Description = "Custom automation sequence",
                ScheduleText = "Manually triggered",
                ActionCountText = "0 actions configured",
                LastRunText = "Never run",
                IsActive = true,
                IsRunning = false,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, 0, 16, 16)
            };
            card.StateChanged += (s, ev) => RefreshStats();
            _routineList.Add(card);

            RenderPage();
            RefreshStats();
        }

        // ─── Pagination render ─────────────────────────────────────────────
        private void RenderPage()
        {
            if (_routineList == null || _flpRoutines == null) return;

            _flpRoutines.SuspendLayout();
            _flpRoutines.Controls.Clear();

            foreach (var card in _routineList)
                _flpRoutines.Controls.Add(card);

            _flpRoutines.ResumeLayout(true);
            AdjustCardWidths();
        }

        // ─── Stats refresh ─────────────────────────────────────────────────
        private void RefreshStats()
        {
            if (_statCards == null || _statCards.Count < 4 || _routineList == null) return;

            _statCards[0].Value = _routineList.Count.ToString();
            _statCards[1].Value = _routineList.Count(x => x.IsActive).ToString();
            _statCards[2].Value = _routineList.Count(x => x.IsRunning).ToString();
            // FIX: was hardcoded to "5" — now sums actions
            // Placeholder until a real data source is wired:
            _statCards[3].Value = _routineList.Sum(x =>
                int.TryParse(x.ActionCountText.Split(' ')[0], out int n) ? n : 0).ToString();
        }

        // ─── Responsive card width ─────────────────────────────────────────
        // FIX: use the FlowLayoutPanel's own width — not this.ClientSize.Width
        // which incorrectly includes the sidebar.
        // ─── Responsive card width ─────────────────────────────────────────
        public void AdjustCardWidths()
        {
            if (_flpRoutines == null || _routineList == null || _routineList.Count == 0) return;
            if (this.IsDisposed) return;

            int available = _flpRoutines.ClientSize.Width - _flpRoutines.Padding.Horizontal - 24;

            // PERFORMANCE: Resize threshold to prevent "shaking" during dragging
            if (Math.Abs(_lastAvailableWidth - available) < 10) return; 
            _lastAvailableWidth = available;

            // Batch all card scaling into one layout cycle
            _flpRoutines.SuspendLayout();
            
            // Dynamic columns based on width breakpoints
            int cols = 3;
            float scalingFactor = 1.0f;

            if (available < 700) { cols = 1; scalingFactor = 0.85f; }
            else if (available < 1050) { cols = 2; scalingFactor = 0.92f; }
            else { cols = 3; scalingFactor = 1.0f; }

            int cardMargin = _flpRoutines.Controls[0].Margin.Horizontal;
            int targetWidth = (available / cols) - cardMargin - 2;
            targetWidth = Math.Max(220, targetWidth);

            foreach (Control c in _flpRoutines.Controls)
            {
                if (c is UC_RoutineCard card)
                {
                    if (card.Width != targetWidth) 
                    {
                        card.Width = targetWidth;
                        card.ScaleUI(scalingFactor);
                    }
                }
            }

            _flpRoutines.ResumeLayout(true);
        }

        // ─── Helpers ───────────────────────────────────────────────────────
        private static void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
            foreach (Control child in control.Controls)
                SetDoubleBuffered(child);
        }
    }
}