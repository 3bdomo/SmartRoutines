using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;
using SmartRoutines.UI.Core.Tray;
using System.Reflection;
using SmartRoutines.UI.Controls.Common;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace SmartRoutines.UI.Forms
{
    public partial class FrmMain : Form
    {
        private Guna2Button? _activeNavButton;
        private bool _sidebarCollapsed = false;
        private const int SidebarExpandedWidth = 310;
        private const int SidebarCollapsedWidth = 60;

        // --- System Tray ---
        private TrayManager _trayManager = null!;
        private bool _forceExit = false;

        // --- Timers stored as fields so they can be disposed ---
        private System.Windows.Forms.Timer _blinkTimer = null!;
        private System.Windows.Forms.Timer _hoverTimer = null!;
        // Debounces window resize events so ScaleUI / AdjustCardWidths aren't called every pixel
        private System.Windows.Forms.Timer _resizeDebounce = null!;
        // Guards against concurrent sidebar animation timers
        private bool _sidebarAnimating = false;

        // Cached nav font - prevents allocating new Font() on every nav click
        private Font _navFont = null!;

        private Label _lblContentTitle = null!;
        private Label _lblContentSubtitle = null!;
        
        // Page caching to eliminate 5-10s load times
        private readonly System.Collections.Generic.Dictionary<Type, UserControl> _pageCache = new();

        private readonly IServiceProvider _serviceProvider;
        // Only reload dashboard data after a routine is saved — not on every navigation
        private bool _dashboardNeedsRefresh = true;

        public FrmMain(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            
            // Performance: High quality rendering styles
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.UserPaint | 
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            // Removed recursive call from constructor to prevent startup crash

            this.MinimumSize = new Size(900, 600);

            // 4px transparent safety buffer for borderless resizing
            this.Padding = new Padding(4);

            // Debounce resize so layout code doesn't run on every drag pixel
            _resizeDebounce = new System.Windows.Forms.Timer { Interval = 150 };
            _resizeDebounce.Tick += (s, e) =>
            {
                _resizeDebounce.Stop();
                // Sidebar breakpoint
                if (this.Width < 1050 && !_sidebarCollapsed)
                {
                    _sidebarCollapsed = true;
                    AnimateSidebar(_sidebarCollapsed);
                }
                else if (this.Width >= 1050 && _sidebarCollapsed)
                {
                    _sidebarCollapsed = false;
                    AnimateSidebar(_sidebarCollapsed);
                }
                // Propagate to dashboard if visible
                if (_currentPage is Controls.UC_Dashboard db) db.AdjustCardWidths();
            };

            ApplyTheme();
            InitializeTray();

            // Pre-warm all pages in the background so the first nav click is instant.
            // WarmUpPagesAsync runs after the handle is created and the window is visible.
            this.HandleCreated += async (s, ev) =>
            {
                await WarmUpPagesAsync();
            };
        }

        // ─── System Tray ──────────────────────────────────────────────────
        private void InitializeTray()
        {
            _trayManager = new TrayManager(this);

            _trayManager.RestoreRequested += RestoreFromTray;

            _trayManager.ExitRequested += () =>
            {
                _forceExit = true;
                Application.Exit();
            };

            _trayManager.DisableAllRequested += () =>
            {
                _trayManager.ShowBalloon("Smart Routines", "All routines have been disabled.", ToolTipIcon.Warning);
            };

            _trayManager.QuickRunRequested += (routineName) =>
            {
                _trayManager.ShowBalloon("Quick Run", $"Running \"{routineName}\"...", ToolTipIcon.Info);
            };
        }

        public void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();
        }

        private void MinimizeToTray()
        {
            this.Hide();
            _trayManager.ShowBalloon("Smart Routines", "Running in background. Click the tray icon to restore.", ToolTipIcon.Info);
        }

        // ─── Close / Resize ────────────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_forceExit && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MinimizeToTray();
                return;
            }

            _blinkTimer?.Stop();
            _blinkTimer?.Dispose();
            _hoverTimer?.Stop();
            _hoverTimer?.Dispose();
            _resizeDebounce?.Stop();
            _resizeDebounce?.Dispose();
            _navFont?.Dispose();
            _trayManager.Dispose();

            base.OnFormClosing(e);
        }

        // WS_EX_COMPOSITED removed to vastly accelerate initial Form Load time / responsiveness

        // Borderless resize hit-test
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTLEFT = 10, HTRIGHT = 11;
            const int HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14;
            const int HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                int resizerSize = 10;
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = this.PointToClient(screenPoint);

                bool onLeft = clientPoint.X <= resizerSize;
                bool onRight = clientPoint.X >= this.ClientSize.Width - resizerSize;
                bool onTop = clientPoint.Y <= resizerSize;
               bool onBottom = clientPoint.Y >= this.ClientSize.Height - resizerSize;

                if (onTop && onLeft) { m.Result = (IntPtr)HTTOPLEFT; return; }
                else if (onTop && onRight) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                else if (onBottom && onLeft) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                else if (onBottom && onRight) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                else if (onTop) { m.Result = (IntPtr)HTTOP; return; }
                else if (onBottom) { m.Result = (IntPtr)HTBOTTOM; return; }
                else if (onLeft) { m.Result = (IntPtr)HTLEFT; return; }
                else if (onRight) { m.Result = (IntPtr)HTRIGHT; return; }
            }

            base.WndProc(ref m);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
                return;
            }

            // Kick off debounce — actual layout work happens 150ms after the last resize event
            _resizeDebounce?.Stop();
            _resizeDebounce?.Start();
        }

        // ─── Background page warm-up ───────────────────────────────────────
        // Creates each page on the UI thread (WinForms requires it) but defers
        // the work to after the window has painted its first frame.
        private async System.Threading.Tasks.Task WarmUpPagesAsync()
        {
            // Yield so the first frame renders fully before we start constructing pages.
            await System.Threading.Tasks.Task.Delay(300);

            if (this.IsDisposed) return;

            // Warm up LogsPage
            if (!_pageCache.ContainsKey(typeof(Controls.UC_LogsPage)))
            {
                this.Invoke(new Action(() =>
                {
                    if (this.IsDisposed) return;
                    var page = new Controls.UC_LogsPage();
                    page.Dock = DockStyle.Fill;
                    page.Visible = false;
                    pnlMainContent.Controls.Add(page);
                    _pageCache[typeof(Controls.UC_LogsPage)] = page;
                }));
            }

            await System.Threading.Tasks.Task.Delay(100);
            if (this.IsDisposed) return;

            // Warm up Settings
            if (!_pageCache.ContainsKey(typeof(Controls.UC_Settings)))
            {
                this.Invoke(new Action(() =>
                {
                    if (this.IsDisposed) return;
                    var page = new Controls.UC_Settings();
                    page.Dock = DockStyle.Fill;
                    page.Visible = false;
                    pnlMainContent.Controls.Add(page);
                    _pageCache[typeof(Controls.UC_Settings)] = page;
                }));
            }
        }

        // ─── Page Navigation ───────────────────────────────────────────────
        private UserControl? _currentPage;

        public void DisplayPage<T>() where T : UserControl
        {
            Type pageType = typeof(T);
            
            // 1. Hide current page (don't dispose!)
            if (_currentPage != null)
            {
                _currentPage.Visible = false;
            }

            // 2. Get or create page (handles caching and DI)
            _currentPage = GetPage<T>();

            _currentPage.Visible = true;
            _currentPage.BringToFront();
            
            // Refresh dashboard layout if it's being shown
            if (_currentPage is Controls.UC_Dashboard dashboard)
            {
                dashboard.AdjustCardWidths();
                // Only reload data if a new routine was saved — preserves IsRunning state
                if (_dashboardNeedsRefresh)
                {
                    _dashboardNeedsRefresh = false;
                    _ = dashboard.ReloadDataAsync();
                }
            }
        }

        public T GetPage<T>() where T : UserControl
        {
            Type pageType = typeof(T);
            if (!_pageCache.TryGetValue(pageType, out var page))
            {
                page = _serviceProvider.GetRequiredService<T>();
                page.Dock = DockStyle.Fill;
                pnlMainContent.Controls.Add(page);
                _pageCache[pageType] = page;
                page.Visible = false;
            }
            return (T)page;
        }

        /// <summary>
        /// Called by the wizard after saving a routine to trigger a fresh dashboard load.
        /// </summary>
        public void RequestDashboardRefresh() => _dashboardNeedsRefresh = true;

        public void ShowToast(string message)
        {
            var toast = new UC_Toast(message);
            this.Controls.Add(toast);
            
            // Position: Top-Right
            int margin = 20;
            toast.Location = new Point(
                this.Width - toast.Width - margin,
                margin + 40 // Offset for the thin title bar
            );
            
            toast.BringToFront();
        }

        private void SetActiveNavButton(Guna2Button btn)
        {
            // Lazily create the font once instead of on every nav click
            _navFont ??= new Font("Segoe UI", 11f, FontStyle.Regular);

            if (_activeNavButton != null)
            {
                _activeNavButton.FillColor = Color.Transparent;
                _activeNavButton.ForeColor = SmartTheme.TextSecondary;
                _activeNavButton.CustomBorderThickness = new Padding(0);
                _activeNavButton.BorderThickness = 0;
                _activeNavButton.Font = _navFont;
                if (_activeNavButton.Tag is string oldIconName)
                    _activeNavButton.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon(oldIconName, 22);
            }

            _activeNavButton = btn;
            _activeNavButton.FillColor = Color.FromArgb(20, SmartTheme.Primary);
            _activeNavButton.ForeColor = SmartTheme.Primary;
            _activeNavButton.CustomBorderColor = SmartTheme.Primary;
            _activeNavButton.BorderColor = Color.Transparent;
            _activeNavButton.BorderThickness = 0;
            _activeNavButton.CustomBorderThickness = new Padding(3, 0, 0, 0);
            _activeNavButton.Font = _navFont;

            if (_activeNavButton.Tag is string newIconName)
                _activeNavButton.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon(newIconName, 22);

            // Update content header title to match active nav
            UpdateContentTitle(btn);
        }

        private void UpdateContentTitle(Guna2Button btn)
        {
            if (_lblContentTitle == null || _lblContentSubtitle == null) return;

            if (btn == btnNavDashboard)
            {
                _lblContentTitle.Text = "Dashboard";
                _lblContentSubtitle.Text = "Manage your automation routines";
            }
            else if (btn == btnNavLogs)
            {
                _lblContentTitle.Text = "Activity Logs";
                _lblContentSubtitle.Text = "Monitor execution history and live output";
            }
            else if (btn == btnNavSettings)
            {
                _lblContentTitle.Text = "Settings";
                _lblContentSubtitle.Text = "Configure application preferences";
            }
        }

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnNavDashboard);
            DisplayPage<Controls.UC_Dashboard>();
        }

        private void btnNavLogs_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnNavLogs);
            DisplayPage<Controls.UC_LogsPage>();
        }

        private void btnNavSettings_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnNavSettings);
            DisplayPage<Controls.UC_Settings>();
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        private void btnMinimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        // ─── Create New Routine button (FIX: was referenced but never defined) ─
        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            DisplayPage<Controls.UC_ActionsMain>();
        }

        // ─── Theme / Layout ────────────────────────────────────────────────
        private void ApplyTheme()
        {
            this.BackColor = SmartTheme.Background;

            // --- Header panel (use designer pnlHeader, do NOT rebuild it) ---
            pnlHeader.FillColor = SmartTheme.Background;
            pnlHeader.BorderColor = SmartTheme.Border;
            pnlHeader.Height = 36; // FIX: match Figma thin title bar (was 60)

            // Style the designer's existing window buttons
            StyleWindowButton(btnClose, "✕", SmartTheme.Danger);
            StyleWindowButton(btnMaximize, "□", SmartTheme.TextSecondary);
            StyleWindowButton(btnMinimize, "─", SmartTheme.TextSecondary);

            // Tiny title label already in designer
            lblAppTitle.Font = SmartTheme.FontSmallBold;
            lblAppTitle.ForeColor = SmartTheme.TextPrimary;
            lblAppTitle.Text = "Smart Routines";
            lblAppSubtitle.Visible = false; // hide in 36px bar

            // Make header draggable
            BindDragLogic(pnlHeader);
            BindDragLogic(lblAppTitle);

            // --- Content header (single panel, built once) ---
            BuildContentHeader();

            // --- Sidebar ---
            pnlSidebar.FillColor = Color.FromArgb(24, 24, 27);
            pnlSidebar.Size = new Size(SidebarExpandedWidth, pnlSidebar.Height); // FIX: was 274

            // Brand logo gradient (FIX: was overwriting FillColor twice, killing gradient)
            pnlLogoBase.FillColor = Color.FromArgb(139, 92, 246); // Purple
            pnlLogoBase.FillColor2 = Color.FromArgb(59, 130, 246); // Blue
            picLogoCircle.FillColor = Color.White;

            lblBrandName.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblBrandName.ForeColor = Color.White;
            lblBrandSubtitle.Font = SmartTheme.FontCaption;
            lblBrandSubtitle.ForeColor = SmartTheme.TextMuted;
            lblBrandSubtitle.AutoSize = true;
            lblBrandSubtitle.Text = "Automation Engine";

            // Collapse button
            btnSidebarCollapse.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnSidebarCollapse.FillColor = SmartTheme.Primary;
            btnSidebarCollapse.ForeColor = Color.White;
            btnSidebarCollapse.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnSidebarCollapse.Text = "<";
            btnSidebarCollapse.HoverState.FillColor = SmartTheme.PrimaryHover;
            btnSidebarCollapse.Size = new Size(24, 24);
            btnSidebarCollapse.Left = pnlSidebar.Width - 12;

            // Separators
            Panel sepBrand = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.FromArgb(42, 42, 42) };
            pnlSidebarBrand.Controls.Add(sepBrand);
            Panel sepFooter = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(42, 42, 42) };
            pnlSidebarFooter.Controls.Add(sepFooter);

            // Engine dot blink (FIX: stored as field so it can be disposed)
            _blinkTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _blinkTimer.Tick += (s, e) => pnlEngineDot.Visible = !pnlEngineDot.Visible;
            _blinkTimer.Start();

            pnlEngineStatusBase.FillColor = Color.FromArgb(32, 32, 35);
            pnlEngineDot.FillColor = SmartTheme.Success;
            lblEngineStatus.Font = SmartTheme.FontSmallBold;
            lblEngineStatus.ForeColor = Color.White;
            lblEngineSubtitle.Font = SmartTheme.FontCaption;
            lblEngineSubtitle.ForeColor = SmartTheme.TextSecondary;

            lblEngineStatus.Location = new Point(36, 14);
            lblEngineSubtitle.Location = new Point(36, 36);
            pnlEngineDot.Location = new Point(16, 18);

            InitializeEngineHover();

            // Nav buttons
            StyleNavButton(btnNavDashboard, "dashboard.png", "Dashboard");
            StyleNavButton(btnNavLogs, "activity.png", "Activity Logs");
            StyleNavButton(btnNavSettings, "settings.png", "Settings");

            // Main content
            pnlMainContent.FillColor = SmartTheme.Background;

            // Apply optimizations selectively to main panels after they are initialized
            SmartRoutines.UI.Core.Helper.ControlOptimizations.EnableDoubleBuffered(pnlSidebar);
            SmartRoutines.UI.Core.Helper.ControlOptimizations.EnableDoubleBuffered(pnlMainContent);

            // Start on Dashboard - DEFERRED safely
            SetActiveNavButton(btnNavDashboard);
            
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() => {
                    DisplayPage<Controls.UC_Dashboard>();
                }));
            }
            else
            {
                this.HandleCreated += (s, ev) => {
                    this.BeginInvoke(new Action(() => {
                        DisplayPage<Controls.UC_Dashboard>();
                    }));
                };
            }
        }

        private void BuildContentHeader()
        {
            // FIX: this is now a SINGLE panel added once — not rebuilt on every nav click.
            // The title label reference is stored in _lblContentTitle so UpdateContentTitle()
            // can update it when the user switches pages.

            // FIX: Increased height from 72 to 92 to prevent subtitle overflow on Dashboard cards
            var pnlContentHeader = new Guna.UI2.WinForms.Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 92,
                FillColor = SmartTheme.Background,
                BorderThickness = 0
            };

            var sepBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = SmartTheme.Border
            };

            // Right: action buttons
            var pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 16, 20, 0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            var btnCreateNew = new Guna.UI2.WinForms.Guna2GradientButton
            {
                Text = "+ Create New Routine",
                Font = SmartTheme.FontSmallBold,
                ForeColor = Color.White,
                Size = new Size(180, 38),
                BorderRadius = 8,
                FillColor = SmartTheme.Primary,
                FillColor2 = SmartTheme.Purple,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };
            btnCreateNew.Click += btnCreateNew_Click;
            
            var btnTheme = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "☼",
                Font = new Font("Segoe UI Symbol", 12f),
                ForeColor = SmartTheme.TextSecondary,
                FillColor = SmartTheme.Surface2,
                Size = new Size(38, 38),
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0)
            };
            btnTheme.HoverState.FillColor = SmartTheme.Surface3;

            var pnlBellContainer = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(38, 38),
                Margin = new Padding(0, 0, 8, 0),
                BackColor = Color.Transparent
            };

            var btnNotif = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "🔔",
                Font = new Font("Segoe UI Symbol", 11f),
                ForeColor = SmartTheme.TextSecondary,
                FillColor = SmartTheme.Surface2,
                Size = new Size(38, 38),
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Location = new Point(0, 0)
            };
            btnNotif.HoverState.FillColor = SmartTheme.Surface3;

            var notifDot = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(8, 8),
                FillColor = SmartTheme.Danger,
                BorderRadius = 4,
                Location = new Point(26, 4) // Top right corner inside the button
            };

            pnlBellContainer.Controls.Add(notifDot);
            pnlBellContainer.Controls.Add(btnNotif);
            notifDot.BringToFront();

            // Right-to-left flow adds items visually right-to-left
            pnlActions.Controls.Add(btnCreateNew);
            pnlActions.Controls.Add(btnTheme);
            pnlActions.Controls.Add(pnlBellContainer);

            // Left: breadcrumb title (stored as field)
            var pnlBreadcrumb = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(24, 16, 0, 0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            _lblContentTitle = new Label
            {
                Text = "Dashboard",
                Font = SmartTheme.FontHeader,
                ForeColor = SmartTheme.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0)
            };

            _lblContentSubtitle = new Label
            {
                Text = "Manage your automation routines",
                Font = SmartTheme.FontBody,
                ForeColor = SmartTheme.TextSecondary,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 0)
            };

            pnlBreadcrumb.Controls.Add(_lblContentTitle);
            pnlBreadcrumb.Controls.Add(_lblContentSubtitle);

            pnlContentHeader.Controls.Add(sepBottom);
            pnlContentHeader.Controls.Add(pnlActions);
            pnlContentHeader.Controls.Add(pnlBreadcrumb);
            pnlBreadcrumb.BringToFront();

            pnlMainContent.Controls.Add(pnlContentHeader);
            pnlContentHeader.BringToFront();
        }

        // ─── Drag logic ────────────────────────────────────────────────────
        private bool _isDragging = false;
        private Point _dragStartPoint;

        private void BindDragLogic(Control ctrl)
        {
            ctrl.DoubleClick += (s, e) =>
            {
                WindowState = WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            };

            ctrl.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    _isDragging = true;
                    _dragStartPoint = new Point(e.X, e.Y);
                }
            };

            ctrl.MouseMove += (s, e) =>
            {
                if (_isDragging && WindowState != FormWindowState.Maximized)
                {
                    Point p = PointToScreen(e.Location);
                    Location = new Point(p.X - _dragStartPoint.X - ctrl.Left,
                                         p.Y - _dragStartPoint.Y - ctrl.Top);
                }
            };

            ctrl.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    _isDragging = false;
            };
        }

        // ─── Icon helpers ──────────────────────────────────────────────────
        private Image GenerateIcon(string text, float emSize, Color color)
        {
            var bmp = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using var font = new Font("Segoe UI Symbol", emSize, FontStyle.Regular);
                using var brush = new SolidBrush(color);
                var fmt = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, brush, new RectangleF(0, -2, 32, 32), fmt);
            }
            return bmp;
        }

        private void StyleNavButton(Guna2Button btn, string iconFileName, string text)
        {
            btn.Tag = iconFileName;
            btn.Text = text;
            btn.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon(iconFileName, 22);
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.ImageOffset = new Point(4, 0);
            btn.TextOffset = new Point(8, 0);

            btn.FillColor = Color.Transparent;
            btn.ForeColor = SmartTheme.TextSecondary;
            btn.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
            btn.TextAlign = HorizontalAlignment.Left;
            btn.BorderRadius = 8;
            btn.Animated = true;
            btn.HoverState.FillColor = Color.FromArgb(32, 32, 35);
            btn.HoverState.ForeColor = SmartTheme.TextPrimary;
        }

        private static void StyleWindowButton(Guna2Button btn, string symbol, Color fg)
        {
            btn.Text = symbol;
            btn.FillColor = Color.Transparent;
            btn.ForeColor = fg;
            // Use a larger, symbol-friendly font so ✕ □ ─ render visibly
            btn.Font = new Font("Segoe UI", 13f, FontStyle.Regular);
            btn.BorderRadius = 4;
            btn.Animated = true;
            btn.HoverState.FillColor = SmartTheme.Surface2;
        }

        // ─── Sidebar animation ─────────────────────────────────────────────
        private void btnSidebarCollapse_Click(object sender, EventArgs e)
        {
            _sidebarCollapsed = !_sidebarCollapsed;
            AnimateSidebar(_sidebarCollapsed);
        }

        private void AnimateSidebar(bool collapse)
        {
            // Guard: prevent stacking multiple concurrent animation timers
            if (_sidebarAnimating) return;
            _sidebarAnimating = true;

            int targetWidth = collapse ? SidebarCollapsedWidth : SidebarExpandedWidth;
            string chevron = collapse ? ">" : "<";

            lblBrandName.Visible = !collapse;
            lblBrandSubtitle.Visible = !collapse;
            lblEngineStatus.Visible = !collapse;
            lblEngineSubtitle.Visible = !collapse;

            var timer = new System.Windows.Forms.Timer { Interval = 16 };
            timer.Tick += (s, e) =>
            {
                int current = pnlSidebar.Width;
                int diff    = targetWidth - current;
                int step    = (int)(diff * 0.28);
                if (step == 0 && diff != 0) step = Math.Sign(diff);
                int next    = current + step;

                if (Math.Abs(targetWidth - next) <= 1)
                {
                    this.SuspendLayout();
                    pnlSidebar.SuspendLayout();

                    pnlSidebar.Width = targetWidth;
                    btnSidebarCollapse.Text = chevron;
                    btnSidebarCollapse.Left = targetWidth - 14;

                    pnlSidebar.ResumeLayout(true);
                    this.ResumeLayout(false);

                    timer.Stop();
                    timer.Dispose();
                    _sidebarAnimating = false;
                    return;
                }

                pnlSidebar.Width = next;
                btnSidebarCollapse.Left = next - 14;
            };
            timer.Start();
        }

        // ─── Double buffer via reflection ──────────────────────────────────
        private static void EnableDoubleBuffering(Control control)
        {
            PropertyInfo? prop = typeof(Control).GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            prop?.SetValue(control, true, null);
        }

        // ─── Engine hover animation ───────────────────────────────────────
        private void InitializeEngineHover()
        {
            int currentAnimState = 0;

            pnlEngineStatusBase.Cursor = Cursors.Hand;
            lblEngineStatus.Cursor = Cursors.Hand;
            lblEngineSubtitle.Cursor = Cursors.Hand;

            // Timer only runs during hover animation, NOT continuously at startup.
            // This saves ~60 unnecessary UI-thread wakeups per second.
            _hoverTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _hoverTimer.Tick += (s, e) =>
            {
                Point mouseAt = pnlEngineStatusBase.PointToClient(Cursor.Position);
                bool isHovering = pnlEngineStatusBase.ClientRectangle.Contains(mouseAt) && !_sidebarCollapsed;

                if (isHovering && currentAnimState < 100) currentAnimState += 12;
                else if (!isHovering && currentAnimState > 0) currentAnimState -= 10;

                currentAnimState = Math.Clamp(currentAnimState, 0, 100);

                int baseRgb = 32;
                int brightOffset = (int)(currentAnimState * 0.15);
                pnlEngineStatusBase.FillColor = Color.FromArgb(
                    baseRgb + brightOffset,
                    baseRgb + brightOffset + 2,
                    baseRgb + brightOffset + 7);

                int shift = currentAnimState / 33;
                pnlEngineStatusBase.Padding = new Padding(shift);

                // Auto-stop when fully settled in un-hovered state — no more CPU burn
                if (!isHovering && currentAnimState == 0)
                    _hoverTimer.Stop();
            };

            // Start the timer only when the user actually hovers over the engine panel
            pnlEngineStatusBase.MouseEnter += (s, e) => _hoverTimer.Start();
            lblEngineStatus.MouseEnter     += (s, e) => _hoverTimer.Start();
            lblEngineSubtitle.MouseEnter   += (s, e) => _hoverTimer.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= SmartRoutines.UI.Core.Helper.ControlOptimizations.WS_EX_COMPOSITED;
                return cp;
            }
        }
    }
}