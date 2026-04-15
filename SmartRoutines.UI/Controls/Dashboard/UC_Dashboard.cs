using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using SmartRoutines.UI.Core.Theme;
using SmartRoutines.Core.Interfaces.Logic;
using System.Collections.Generic;
using SmartRoutines.Core.DTOs;
using System.Threading.Tasks;
using SmartRoutines.UI.Core.Helper;
using SmartRoutines.UI.Controls.AddRoutine.UC_Step3;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_Dashboard : SmartUserControl
    {
        private System.Collections.Generic.List<UC_RoutineCard> _routineList = null!;
        private int _lastAvailableWidth = 0;
        private System.Windows.Forms.Timer _resizeDebounce = null!;

        // ── Cached stat values to avoid redundant LINQ on every event ──────
        private int _cachedTotal, _cachedActive, _cachedRunning, _cachedActions;

        private readonly IRoutineService _routineService;

        public UC_Dashboard(IRoutineService routineService)
        {
            _routineService = routineService;
            InitializeComponent();

            // Set styles BEFORE anything else — avoids mid-init repaints
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);   // ← added ResizeRedraw
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = SmartTheme.Background;

            // Defer everything until the handle exists → avoids layout before
            // the control has real pixel dimensions
            this.HandleCreated += (_, __) => InitializeDashboard();
        }

        // ─── Initialization ─────────────────────────────────────────────────
        private void InitializeDashboard()
        {
            // ── Freeze the entire form hierarchy in ONE call ───────────────
            var form = this.FindForm();
            form?.SuspendLayout();
            this.SuspendLayout();

            try
            {
                // ── Double-buffer designer panels (reflection, done once) ──
                EnableDoubleBuffered(tlpStatCards);
                EnableDoubleBuffered(flpRoutineCards);

                // ── Stat cards ─────────────────────────────────────────────
                ConfigureStatCards();

                // ── "Your Routines" label ──────────────────────────────────
                lblYourRoutines.Font = SmartTheme.FontSubheader;
                lblYourRoutines.ForeColor = SmartTheme.TextPrimary;
                lblYourRoutines.BackColor = Color.Transparent;

                // ── Routine card container ─────────────────────────────────
                flpRoutineCards.WrapContents = true;
                flpRoutineCards.AutoScroll = true;
                flpRoutineCards.Padding = new Padding(20, 4, 20, 20);
                flpRoutineCards.Margin = new Padding(0);
                flpRoutineCards.BackColor = SmartTheme.Background;

                // ── Build all cards, then add in ONE batch (no layout storms)
                _routineList = new System.Collections.Generic.List<UC_RoutineCard>();
                _ = ReloadDataAsync(); // Async load from service

                flpRoutineCards.SuspendLayout();

                // AddRange = single Controls.CollectionChanged event
                var controls = new Control[_routineList.Count];
                for (int i = 0; i < _routineList.Count; i++)
                {
                    var card = _routineList[i];
                    card.Margin = new Padding(0, 0, 10, 10);
                    card.StateChanged += OnCardStateChanged;
                    controls[i] = card;
                }
                flpRoutineCards.Controls.AddRange(controls);   // ← ONE layout pass
                flpRoutineCards.ResumeLayout(false);           // false = no immediate recalc

                // ── Compute stats once (no per-card LINQ during load) ──────
                RecalculateCachedStats();
                PushStatsToCards();

                // ── Debounced resize ───────────────────────────────────────
                _resizeDebounce = new System.Windows.Forms.Timer { Interval = 150 };
                _resizeDebounce.Tick += (_, __) => { _resizeDebounce.Stop(); AdjustCardWidthsAndGrid(); };
                flpRoutineCards.SizeChanged += OnDeferredResize;
                this.SizeChanged += OnDeferredResize;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard load error: " + ex.Message);
            }
            finally
            {
                // Always resume — even if something threw
                this.ResumeLayout(true);
                form?.ResumeLayout(true);

                // Layout is stable now — safe to measure real widths
                AdjustCardWidthsAndGrid();
            }
        }

        // ─── Stat Card Configuration ─────────────────────────────────────────
        private void ConfigureStatCards()
        {
            var cards = new[] { statCard1, statCard2, statCard3, statCard4 };

            var accentColors = new[]
            {
        Color.FromArgb(59,  130, 246),
        Color.FromArgb(34,  197, 94 ),
        Color.FromArgb(139, 92,  246),
        Color.FromArgb(243, 156, 18 )
    };

            var bgs = new[]
            {
        Color.FromArgb(15,  23,  42),
        Color.FromArgb(15,  28,  20),
        Color.FromArgb(24,  18,  43),
        Color.FromArgb(30,  20,  10)
    };

            var titles = new[] { "Total Routines", "Active Routines", "Running Now", "Total Actions" };
            var icons = new[] { "dashboard.png", "success.png", "activity.png", "add.png" };

            for (int i = 0; i < 4; i++)
            {
                cards[i].AccentColor = accentColors[i];
                cards[i].CardColor = bgs[i];
                cards[i].Title = titles[i];
                cards[i].CardIcon = icons[i];
                cards[i].Value = "0";
                cards[i].BorderRadius = 20;
            }
        }

        // ─── Seed sample data ────────────────────────────────────────────────
        public async Task ReloadDataAsync()
        {
            try
            {
                var cards = await _routineService.GetAllCardsAsync();
                
                this.InvokeIfRequired(() => {
                    flpRoutineCards.SuspendLayout();
                    flpRoutineCards.Controls.Clear();
                    _routineList.Clear();

                    foreach (var cardDto in cards)
                    {
                        var card = new UC_RoutineCard
                        {
                            Id = cardDto.Id,
                            RoutineName = cardDto.Name,
                            Description = cardDto.Description,
                            ScheduleText = cardDto.TriggerSummary,
                            ActionCountText = $"{cardDto.ActionCount} actions configured",
                            LastRunText = string.IsNullOrEmpty(cardDto.LastRunRelativeTime) ? "Never run" : $"Last run: {cardDto.LastRunRelativeTime}",
                            IsActive = cardDto.IsActive,
                            IsRunning = cardDto.IsRunningNow,
                            Margin = new Padding(0, 0, 10, 10)
                        };

                        card.StateChanged += OnCardStateChanged;
                        card.DeleteRequested += async (s, e) =>
                        {
                            if (s is UC_RoutineCard c)
                                await OnCardDeleteRequestedAsync(c);
                        };
                        _routineList.Add(card);
                        flpRoutineCards.Controls.Add(card);
                    }

                    flpRoutineCards.ResumeLayout(true);
                    RecalculateCachedStats();
                    PushStatsToCards();
                    AdjustCardWidthsAndGrid();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading routines: " + ex.Message);
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────
        public void AddNewRoutine(string name, string description)
        {
            var card = new UC_RoutineCard
            {
                RoutineName = name,
                Description = description,
                ScheduleText = "Manual trigger",
                ActionCountText = "0 actions configured",
                LastRunText = "Never run",
                IsActive = true,
                IsRunning = false,
                Margin = new Padding(0, 0, 10, 10)
            };
            card.StateChanged += OnCardStateChanged;
            _routineList.Add(card);

            // Add single card without touching the whole layout
            flpRoutineCards.SuspendLayout();
            flpRoutineCards.Controls.Add(card);
            flpRoutineCards.ResumeLayout(false);   // ← false keeps it lazy

            // Invalidate cache incrementally — no full LINQ scan
            _cachedTotal++;
            if (card.IsActive) _cachedActive++;
            if (card.IsRunning) _cachedRunning++;
            PushStatsToCards();

            AdjustCardWidthsAndGrid();
        }

        // ─── Card event handlers ──────────────────────────────────────────────
        private async Task OnCardDeleteRequestedAsync(UC_RoutineCard card)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete \"{card.RoutineName}\"?",
                "Delete Routine",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                await _routineService.DeleteAsync(card.Id);

                this.InvokeIfRequired(() =>
                {
                    flpRoutineCards.SuspendLayout();
                    flpRoutineCards.Controls.Remove(card);
                    _routineList.Remove(card);
                    flpRoutineCards.ResumeLayout(true);

                    RecalculateCachedStats();
                    PushStatsToCards();
                    AdjustCardWidthsAndGrid();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting routine: " + ex.Message);
            }
        }

        // ─── Cached stats (avoid LINQ on every event) ────────────────────────
        private void RecalculateCachedStats()
        {
            _cachedTotal = _routineList.Count;
            _cachedActive = 0;
            _cachedRunning = 0;
            _cachedActions = 0;

            // Single pass — no multiple LINQ iterations
            foreach (var c in _routineList)
            {
                if (c.IsActive) _cachedActive++;
                if (c.IsRunning) _cachedRunning++;
                _cachedActions += int.TryParse(
                    c.ActionCountText.Split(' ')[0], out int n) ? n : 0;
            }
        }

        private void PushStatsToCards()
        {
            statCard1.Value = _cachedTotal.ToString();
            statCard2.Value = _cachedActive.ToString();
            statCard3.Value = _cachedRunning.ToString();
            statCard4.Value = _cachedActions.ToString();
        }

        // ─── StateChanged handler (replaces inline lambda) ───────────────────
        private async void OnCardStateChanged(object? sender, EventArgs e)
        {
            if (sender is UC_RoutineCard card)
            {
                try 
                {
                    await _routineService.ToggleStatusAsync(card.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating routine status: " + ex.Message);
                }
            }

            RecalculateCachedStats();
            PushStatsToCards();
        }

        // ─── Debounced resize ────────────────────────────────────────────────
        private void OnDeferredResize(object? sender, EventArgs e)
        {
            _resizeDebounce.Stop();
            _resizeDebounce.Start();
        }

        // ─── Responsive layout ───────────────────────────────────────────────
        public void AdjustCardWidthsAndGrid()
        {
            if (_routineList == null || _routineList.Count == 0) return;
            if (this.IsDisposed) return;

            // ── Stat grid breakpoint ───────────────────────────────────────
            int formWidth = this.FindForm()?.Width ?? this.Width;
            bool wantTwoCols = formWidth < 850;

            if (wantTwoCols && tlpStatCards.ColumnCount != 2)
            {
                ApplyStatGrid(2, 2, 260);
            }
            else if (!wantTwoCols && tlpStatCards.ColumnCount != 4)
            {
                ApplyStatGrid(4, 1, 148);
            }

            // ── Routine card widths ────────────────────────────────────────
            int available = flpRoutineCards.ClientSize.Width
                          - flpRoutineCards.Padding.Horizontal
                          - SystemInformation.VerticalScrollBarWidth;

            if (Math.Abs(_lastAvailableWidth - available) < 8) return;
            _lastAvailableWidth = available;

            int cols;
            float scaleFactor;

            if (available < 620) { cols = 1; scaleFactor = 0.85f; }
            else if (available < 1000) { cols = 2; scaleFactor = 0.92f; }
            else { cols = 3; scaleFactor = 1.0f; }

            const int cardMarginH = 10;
            int targetWidth = Math.Max(240, (available / cols) - cardMarginH - 2);

            // ── Batch card resizes in one suspended block ──────────────────
            flpRoutineCards.SuspendLayout();
            foreach (Control c in flpRoutineCards.Controls)
            {
                if (c is UC_RoutineCard card && card.Width != targetWidth)
                {
                    card.Width = targetWidth;
                    card.ScaleUI(scaleFactor);
                }
            }
            flpRoutineCards.ResumeLayout(true);
        }

        // Extracted helper — removes duplicated tlpStatCards wiring ──────────
        private void ApplyStatGrid(int cols, int rows, int height)
        {
            tlpStatCards.SuspendLayout();
            tlpStatCards.ColumnCount = cols;
            tlpStatCards.RowCount = rows;

            tlpStatCards.ColumnStyles.Clear();
            float pct = 100f / cols;
            for (int i = 0; i < cols; i++)
                tlpStatCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, pct));

            tlpStatCards.RowStyles.Clear();
            float rowPct = 100f / rows;
            for (int i = 0; i < rows; i++)
                tlpStatCards.RowStyles.Add(new RowStyle(SizeType.Percent, rowPct));

            // Re-seat the 4 stat cards
            var cards = new[] { statCard1, statCard2, statCard3, statCard4 };
            for (int i = 0; i < 4; i++)
                tlpStatCards.SetCellPosition(cards[i],
                    new TableLayoutPanelCellPosition(i % cols, i / cols));

            tlpStatCards.Height = height;
            tlpStatCards.ResumeLayout(true);
        }

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