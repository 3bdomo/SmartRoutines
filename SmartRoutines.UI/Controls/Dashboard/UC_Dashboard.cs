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
        // Only manages the list of cards — no code-created layout panels
        private System.Collections.Generic.List<UC_RoutineCard> _routineList = null!;
        private int _lastAvailableWidth = 0;
        private System.Windows.Forms.Timer _resizeDebounce = null!;

        public UC_Dashboard()
        {
            InitializeComponent();

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = SmartTheme.Background;

            InitializeDashboard();
        }

        // ─── Initialization ─────────────────────────────────────────────────
        private void InitializeDashboard()
        {
            try
            {
                this.SuspendLayout();

                // ── Double-buffer all Designer panels ──────────────────────
                EnableDoubleBuffered(tlpStatCards);
                EnableDoubleBuffered(flpRoutineCards);

                // ── Configure Designer stat cards (no new UC_StatCard created) ──
                ConfigureStatCards();

                // ── Configure "Your Routines" label ───────────────────────
                lblYourRoutines.Font = SmartTheme.FontSubheader;
                lblYourRoutines.ForeColor = SmartTheme.TextPrimary;
                lblYourRoutines.BackColor = Color.Transparent;

                // ── Configure routine card container ───────────────────────
                flpRoutineCards.WrapContents = true;
                flpRoutineCards.AutoScroll = true;
                flpRoutineCards.Padding = new Padding(20, 4, 20, 20);
                flpRoutineCards.Margin = new Padding(0);
                flpRoutineCards.BackColor = SmartTheme.Background;

                // ── Seed data ──────────────────────────────────────────────
                _routineList = new System.Collections.Generic.List<UC_RoutineCard>();
                SeedRoutines();

                foreach (var card in _routineList)
                {
                    card.Margin = new Padding(0, 0, 10, 10);
                    card.StateChanged += (s, ev) => RefreshStats();
                    flpRoutineCards.Controls.Add(card);
                }

                RefreshStats();

                // ── Debounced resize ───────────────────────────────────────
                _resizeDebounce = new System.Windows.Forms.Timer { Interval = 150 };
                _resizeDebounce.Tick += (s, e) =>
                {
                    _resizeDebounce.Stop();
                    AdjustCardWidthsAndGrid();
                };
                flpRoutineCards.SizeChanged += OnDeferredResize;
                this.SizeChanged += OnDeferredResize;

                AdjustCardWidthsAndGrid();
                this.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard load error: " + ex.Message);
            }
        }

        // ─── Stat Card Configuration ────────────────────────────────────────
        private void ConfigureStatCards()
        {
            // Colors match Figma reference image exactly
            var cards = new[] { statCard1, statCard2, statCard3, statCard4 };

            var accents = new[]
            {
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(34, 197, 94),
                Color.FromArgb(139, 92, 246),
                Color.FromArgb(243, 156, 18)
            };
            var bgs = new[]
            {
                Color.FromArgb(15, 23, 42),
                Color.FromArgb(15, 28, 20),
                Color.FromArgb(24, 18, 43),
                Color.FromArgb(30, 20, 10)
            };
            var titles = new[] { "Total Routines", "Active Routines", "Running Now", "Total Actions" };
            var icons  = new[] { "dashboard.png", "success.png", "activity.png", "add.png" };

            for (int i = 0; i < 4; i++)
            {
                cards[i].AccentColor  = accents[i];
                cards[i].CardColor    = bgs[i];
                cards[i].Title        = titles[i];
                cards[i].CardIcon     = icons[i];
                cards[i].Value        = "0";
                cards[i].BorderRadius = 20;
                // Dock.Fill already set in Designer; Margin set there too (6px uniform)
            }
        }

        // ─── Seed sample data ────────────────────────────────────────────────
        private void SeedRoutines()
        {
            var data = new[]
            {
                ("Morning Setup",    "Launches email, calendar, and sets volume",
                 "Every Mon, Tue, Wed, Thu, Fri at 08:00", "2 actions configured", "✔ Last run: 4:54:00 PM",  true,  false),
                ("Focus Mode",       "Silences notifications and closes distracting apps",
                 "When code.exe launches",                  "2 actions configured", "✔ Last run: 5:54:00 PM",  true,  true),
                ("Evening Shutdown", "Closes all work apps and backs up files",
                 "Every Mon, Tue, Wed, Thu, Fri at 18:00", "1 action configured",  "✖ Last run: 5:54:00 PM",  false, false),
                ("System Scan",      "Background maintenance operation",
                 "Every Sunday",                            "3 actions configured", "Never",                    false, false)
            };

            foreach (var (name, desc, sched, actions, lastRun, active, running) in data)
            {
                var card = new UC_RoutineCard
                {
                    RoutineName     = name,
                    Description     = desc,
                    ScheduleText    = sched,
                    ActionCountText = actions,
                    LastRunText     = lastRun,
                    IsActive        = active,
                    IsRunning       = running
                };
                _routineList.Add(card);
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────
        public void AddNewRoutine()
        {
            var card = new UC_RoutineCard
            {
                RoutineName     = "New Routine",
                Description     = "Custom automation sequence",
                ScheduleText    = "Manually triggered",
                ActionCountText = "0 actions configured",
                LastRunText     = "Never run",
                IsActive        = true,
                IsRunning       = false,
                Margin          = new Padding(0, 0, 10, 10)
            };
            card.StateChanged += (s, ev) => RefreshStats();
            _routineList.Add(card);
            flpRoutineCards.Controls.Add(card);
            RefreshStats();
            AdjustCardWidthsAndGrid();
        }

        // ─── Stats refresh ───────────────────────────────────────────────────
        private void RefreshStats()
        {
            if (_routineList == null) return;

            statCard1.Value = _routineList.Count.ToString();
            statCard2.Value = _routineList.Count(x => x.IsActive).ToString();
            statCard3.Value = _routineList.Count(x => x.IsRunning).ToString();
            statCard4.Value = _routineList.Sum(x =>
                int.TryParse(x.ActionCountText.Split(' ')[0], out int n) ? n : 0).ToString();
        }

        // ─── Debounced resize handler ────────────────────────────────────────
        private void OnDeferredResize(object? sender, EventArgs e)
        {
            _resizeDebounce.Stop();
            _resizeDebounce.Start();
        }

        // ─── Responsive layout: cards + stat grid breakpoint ────────────────
        public void AdjustCardWidthsAndGrid()
        {
            if (_routineList == null || _routineList.Count == 0) return;
            if (this.IsDisposed) return;

            // ── Stat grid: 2×2 below 850px, 4×1 above ─────────────────────
            int formWidth = this.FindForm()?.Width ?? this.Width;
            if (formWidth < 850)
            {
                // 2-column × 2-row grid
                if (tlpStatCards.ColumnCount != 2)
                {
                    tlpStatCards.SuspendLayout();
                    tlpStatCards.ColumnCount = 2;
                    tlpStatCards.RowCount = 2;
                    tlpStatCards.ColumnStyles.Clear();
                    tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                    tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                    tlpStatCards.RowStyles.Clear();
                    tlpStatCards.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
                    tlpStatCards.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
                    // Re-position cards for 2×2
                    tlpStatCards.SetCellPosition(statCard1, new TableLayoutPanelCellPosition(0, 0));
                    tlpStatCards.SetCellPosition(statCard2, new TableLayoutPanelCellPosition(1, 0));
                    tlpStatCards.SetCellPosition(statCard3, new TableLayoutPanelCellPosition(0, 1));
                    tlpStatCards.SetCellPosition(statCard4, new TableLayoutPanelCellPosition(1, 1));
                    tlpStatCards.Height = 260; // taller for 2-row
                    tlpStatCards.ResumeLayout(true);
                }
            }
            else
            {
                // 4-column × 1-row grid
                if (tlpStatCards.ColumnCount != 4)
                {
                    tlpStatCards.SuspendLayout();
                    tlpStatCards.ColumnCount = 4;
                    tlpStatCards.RowCount = 1;
                    tlpStatCards.ColumnStyles.Clear();
                    tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                    tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                    tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                    tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                    tlpStatCards.RowStyles.Clear();
                    tlpStatCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                    // Re-position cards for 4×1
                    tlpStatCards.SetCellPosition(statCard1, new TableLayoutPanelCellPosition(0, 0));
                    tlpStatCards.SetCellPosition(statCard2, new TableLayoutPanelCellPosition(1, 0));
                    tlpStatCards.SetCellPosition(statCard3, new TableLayoutPanelCellPosition(2, 0));
                    tlpStatCards.SetCellPosition(statCard4, new TableLayoutPanelCellPosition(3, 0));
                    tlpStatCards.Height = 148;
                    tlpStatCards.ResumeLayout(true);
                }
            }

            // ── Routine card widths ────────────────────────────────────────
            int available = flpRoutineCards.ClientSize.Width
                          - flpRoutineCards.Padding.Horizontal
                          - SystemInformation.VerticalScrollBarWidth;

            if (Math.Abs(_lastAvailableWidth - available) < 8) return;
            _lastAvailableWidth = available;

            int cols;
            float scaleFactor;

            if (available < 620)       { cols = 1; scaleFactor = 0.85f; }
            else if (available < 1000) { cols = 2; scaleFactor = 0.92f; }
            else                       { cols = 3; scaleFactor = 1.0f;  }

            // Card margin horizontal contribution from Padding(0,0,10,10)
            const int cardMarginH = 10;
            int targetWidth = (available / cols) - cardMarginH - 2;
            targetWidth = Math.Max(240, targetWidth);

            flpRoutineCards.SuspendLayout();
            foreach (Control c in flpRoutineCards.Controls)
            {
                if (c is UC_RoutineCard card)
                {
                    if (card.Width != targetWidth)
                    {
                        card.Width = targetWidth;
                        card.ScaleUI(scaleFactor);
                    }
                }
            }
            flpRoutineCards.ResumeLayout(true);
        }

        // Keep public alias for FrmMain.DisplayPage callback
        public void AdjustCardWidths() => AdjustCardWidthsAndGrid();

        // ─── Helper ──────────────────────────────────────────────────────────
        private static void EnableDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}