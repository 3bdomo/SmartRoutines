using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Forms
{
    public partial class FrmMain : Form
    {
        private Guna2Button? _activeNavButton;
        private bool _sidebarCollapsed = false;
        private const int SidebarExpandedWidth = 260;
        private const int SidebarCollapsedWidth = 76;

        public FrmMain()
        {
            InitializeComponent();

            // Override minimum bounds dynamically so the user can deeply test responsive web-like squishing
            this.MinimumSize = new Size(700, 500);

            // EXTREMELY CRITICAL: Expose 2px of the absolute root Form edge. 
            // If the child Panels sit perfectly on x=0 and y=0, they block Windows from sending 
            // the WM_NCHITTEST (Mouse Resize) messages to the Form entirely.
            this.Padding = new Padding(2);

            ApplyTheme();
        }

        // --- Low-Level OS Hit-Test Hook for Ultimate Borderless Resizability ---
        // This ensures the outer 6 pixels of the application always behave as a resizable frame,
        // even if inner UI components are physically rendering over them!
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;

            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == 1) // 1 = HTCLIENT
            {
                int resizerSize = 6;
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = this.PointToClient(screenPoint);

                if (clientPoint.Y <= resizerSize)
                {
                    if (clientPoint.X <= resizerSize) m.Result = (IntPtr)HTTOPLEFT;
                    else if (clientPoint.X >= this.ClientSize.Width - resizerSize) m.Result = (IntPtr)HTTOPRIGHT;
                    else m.Result = (IntPtr)HTTOP;
                }
                else if (clientPoint.Y >= this.ClientSize.Height - resizerSize)
                {
                    if (clientPoint.X <= resizerSize) m.Result = (IntPtr)HTBOTTOMLEFT;
                    else if (clientPoint.X >= this.ClientSize.Width - resizerSize) m.Result = (IntPtr)HTBOTTOMRIGHT;
                    else m.Result = (IntPtr)HTBOTTOM;
                }
                else if (clientPoint.X <= resizerSize) m.Result = (IntPtr)HTLEFT;
                else if (clientPoint.X >= this.ClientSize.Width - resizerSize) m.Result = (IntPtr)HTRIGHT;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Responsive auto-collapse based on pure window width
            if (this.Width < 1050 && !_sidebarCollapsed)
            {
                // Screen too tight! Auto shrink sidebar to icons.
                _sidebarCollapsed = true;
                AnimateSidebar(_sidebarCollapsed);
            }
            else if (this.Width >= 1050 && _sidebarCollapsed)
            {
                // Plenty of room! Auto push sidebar back to full glory.
                _sidebarCollapsed = false;
                AnimateSidebar(_sidebarCollapsed);
            }
        }

        public void DisplayPage(UserControl page)
        {
            // Remove old pages (keep pnlContentHeader)
            var toRemove = pnlMainContent.Controls
                .OfType<UserControl>()
                .ToList();
            foreach (var old in toRemove)
            {
                pnlMainContent.Controls.Remove(old);
                old.Dispose();
            }

            page.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(page);
            page.BringToFront();
        }

        private void SetActiveNavButton(Guna2Button btn)
        {
            if (_activeNavButton != null)
            {
                _activeNavButton.FillColor = Color.Transparent;
                _activeNavButton.ForeColor = SmartTheme.TextSecondary;
                _activeNavButton.CustomBorderThickness = new Padding(0);
                _activeNavButton.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
                if (_activeNavButton.Tag is string oldIcon) // Refresh image color back to gray
                    _activeNavButton.Image = GenerateIcon(oldIcon, 22f, SmartTheme.TextSecondary);
            }

            _activeNavButton = btn;
            _activeNavButton.FillColor = Color.FromArgb(30, 48, 80); // Exact rich dark blue from image
            _activeNavButton.ForeColor = SmartTheme.Primary;
            _activeNavButton.CustomBorderColor = SmartTheme.Primary;
            _activeNavButton.CustomBorderThickness = new Padding(4, 0, 0, 0);
            _activeNavButton.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            if (_activeNavButton.Tag is string newIcon) // Refresh image color to bright blue active state
                _activeNavButton.Image = GenerateIcon(newIcon, 22f, SmartTheme.Primary);
        }

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnNavDashboard);
            DisplayPage(new Controls.UC_Dashboard());
        }

        private void btnNavLogs_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnNavLogs);
            DisplayPage(new Controls.UC_LogsPage());
        }

        private void btnNavSettings_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnNavSettings);
            DisplayPage(new Controls.UC_Settings());
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        private void btnMinimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void ApplyTheme()
        {
            this.BackColor = SmartTheme.Background;
            pnlHeader.FillColor = SmartTheme.Background;
            lblAppTitle.Font = SmartTheme.FontSubheader;
            lblAppTitle.ForeColor = SmartTheme.TextPrimary;
            lblAppSubtitle.Font = SmartTheme.FontCaption;
            lblAppSubtitle.ForeColor = SmartTheme.TextMuted;

            BuildTitleBar();
            BuildContentHeader();

            // Sidebar Background
            pnlSidebar.FillColor = Color.FromArgb(24, 24, 27);

            // Brand Logo (Gradient Square + Circle)
            pnlLogoBase.FillColor = Color.FromArgb(139, 92, 246);    // Purple
            pnlLogoBase.FillColor2 = Color.FromArgb(59, 130, 246);   // Blue
            picLogoCircle.FillColor = Color.White;                    // White center

            lblBrandName.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblBrandName.ForeColor = Color.White;
            lblBrandSubtitle.Font = SmartTheme.FontCaption;
            lblBrandSubtitle.ForeColor = SmartTheme.TextMuted;

            // Collapse Button
            btnSidebarCollapse.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnSidebarCollapse.FillColor = SmartTheme.Primary;
            btnSidebarCollapse.ForeColor = Color.White;
            btnSidebarCollapse.Font = new Font("Segoe UI", 10f, FontStyle.Bold); // Standard font so text renders safely
            btnSidebarCollapse.Text = "<";
            btnSidebarCollapse.HoverState.FillColor = SmartTheme.PrimaryHover;
            btnSidebarCollapse.Size = new Size(24, 24);
            btnSidebarCollapse.TextOffset = new Point(1, -1);
            btnSidebarCollapse.Left = pnlSidebar.Width - 12;

            // Add 1px Separator bottom of branding area
            Panel sepBrand = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.FromArgb(42, 42, 42) };
            pnlSidebarBrand.Controls.Add(sepBrand);

            // Add 1px Separator top of footer area
            Panel sepFooter = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(42, 42, 42) };
            pnlSidebarFooter.Controls.Add(sepFooter);

            // Engine blinking
            var blinkTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            blinkTimer.Tick += (s, e) => pnlEngineDot.Visible = !pnlEngineDot.Visible;
            blinkTimer.Start();

            // Engine Footer Panel
            pnlEngineStatusBase.FillColor = Color.FromArgb(32, 32, 35);
            pnlEngineDot.FillColor = SmartTheme.Success;
            lblEngineStatus.Font = SmartTheme.FontSmallBold;
            lblEngineStatus.ForeColor = Color.White;
            lblEngineSubtitle.Font = SmartTheme.FontCaption;
            lblEngineSubtitle.ForeColor = SmartTheme.TextSecondary;

            // Adjust margins for beautiful spacing
            lblEngineStatus.Location = new Point(48, 14);
            lblEngineSubtitle.Location = new Point(48, 36);
            pnlEngineDot.Location = new Point(22, 28);

            InitializeEngineHover();

            // Sidebar Nav Items padding and styling
            StyleNavButton(btnNavDashboard, "⊞", "Dashboard");
            StyleNavButton(btnNavLogs, "⚡", "Activity Logs");
            StyleNavButton(btnNavSettings, "⚙", "Settings");

            pnlMainContent.FillColor = SmartTheme.Background;

            StyleWindowButton(btnClose, "✕", SmartTheme.Danger);
            StyleWindowButton(btnMaximize, "□", SmartTheme.TextSecondary);
            StyleWindowButton(btnMinimize, "─", SmartTheme.TextSecondary);

            SetActiveNavButton(btnNavDashboard);
            DisplayPage(new Controls.UC_Dashboard());
        }

        private void BuildTitleBar()
        {
            pnlHeader.Controls.Clear();
            pnlHeader.Height = 36; // Tiny Windows 11 style title bar
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.FillColor = SmartTheme.Background;
            pnlHeader.BorderThickness = 0;

            // 1. Right Section (Window Controls)
            var pnlRightControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            var btnCloseWin = new Button
            {
                Size = new Size(46, pnlHeader.Height),
                FlatStyle = FlatStyle.Flat,
                Text = "✕",
                Font = new Font("Segoe UI", 10f),
                ForeColor = SmartTheme.TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Default,
                Margin = new Padding(0)
            };
            btnCloseWin.FlatAppearance.BorderSize = 0;
            btnCloseWin.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
            btnCloseWin.FlatAppearance.MouseDownBackColor = Color.FromArgb(190, 15, 30);
            btnCloseWin.MouseEnter += (s, e) => btnCloseWin.ForeColor = Color.White;
            btnCloseWin.MouseLeave += (s, e) => btnCloseWin.ForeColor = SmartTheme.TextSecondary;
            btnCloseWin.Click += (s, e) => Application.Exit();

            var btnMaxWin = new Button
            {
                Size = new Size(46, pnlHeader.Height),
                FlatStyle = FlatStyle.Flat,
                Text = "□",
                Font = new Font("Segoe UI", 11f),
                ForeColor = SmartTheme.TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Default,
                Margin = new Padding(0)
            };
            btnMaxWin.FlatAppearance.BorderSize = 0;
            btnMaxWin.FlatAppearance.MouseOverBackColor = SmartTheme.Surface2;
            btnMaxWin.MouseEnter += (s, e) => btnMaxWin.ForeColor = SmartTheme.TextPrimary;
            btnMaxWin.MouseLeave += (s, e) => btnMaxWin.ForeColor = SmartTheme.TextSecondary;
            btnMaxWin.Click += (s, e) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;

            var btnMinWin = new Button
            {
                Size = new Size(46, pnlHeader.Height),
                FlatStyle = FlatStyle.Flat,
                Text = "─",
                Font = new Font("Segoe UI", 11f),
                ForeColor = SmartTheme.TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Default,
                Margin = new Padding(0)
            };
            btnMinWin.FlatAppearance.BorderSize = 0;
            btnMinWin.FlatAppearance.MouseOverBackColor = SmartTheme.Surface2;
            btnMinWin.MouseEnter += (s, e) => btnMinWin.ForeColor = SmartTheme.TextPrimary;
            btnMinWin.MouseLeave += (s, e) => btnMinWin.ForeColor = SmartTheme.TextSecondary;
            btnMinWin.Click += (s, e) => WindowState = FormWindowState.Minimized;

            pnlRightControls.Controls.Add(btnCloseWin);
            pnlRightControls.Controls.Add(btnMaxWin);
            pnlRightControls.Controls.Add(btnMinWin);

            // 2. Left Section (Tiny App Logo)
            var pnlLeftControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(24, 8, 0, 0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            var picTinyLogo = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(16, 16),
                BorderRadius = 4,
                FillColor = SmartTheme.Primary,
                Margin = new Padding(0, 2, 8, 0)
            };

            var lblTinyTitle = new Label
            {
                Text = "Smart Routines",
                Font = SmartTheme.FontSmallBold,
                ForeColor = SmartTheme.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 0)
            };

            pnlLeftControls.Controls.Add(picTinyLogo);
            pnlLeftControls.Controls.Add(lblTinyTitle);

            // Assembly & Physics
            pnlHeader.Controls.Add(pnlRightControls);
            pnlHeader.Controls.Add(pnlLeftControls);
            pnlLeftControls.BringToFront();

            BindDragLogic(pnlHeader);
            BindDragLogic(pnlLeftControls);
            BindDragLogic(lblTinyTitle);
            BindDragLogic(picTinyLogo);
        }

        private void BuildContentHeader()
        {
            var pnlContentHeader = new Guna.UI2.WinForms.Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                FillColor = SmartTheme.Background,
                BorderThickness = 0
            };

            // Separator between content header and content
            var sepBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = SmartTheme.Border
            };

            // 1. Right Section (Actions)
            var pnlActionsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 22, 24, 0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            var btnCreateNew = new Guna.UI2.WinForms.Guna2GradientButton
            {
                Text = "＋ Create New Routine",
                Font = SmartTheme.FontSmallBold,
                ForeColor = Color.White,
                Size = new Size(170, 40),
                BorderRadius = 8,
                FillColor = SmartTheme.Primary,
                FillColor2 = SmartTheme.Purple,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 16, 0)
            };
          //  btnCreateNew.Click += btnCreateNew_Click;

            var btnTheme = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "⚙",
                Font = new Font("Segoe UI Symbol", 12f),
                ForeColor = SmartTheme.TextSecondary,
                FillColor = SmartTheme.Surface2,
                Size = new Size(40, 40),
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };
            btnTheme.HoverState.FillColor = SmartTheme.Surface3;
            btnTheme.HoverState.ForeColor = SmartTheme.TextPrimary;

            var btnNotif = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "🔔",
                Font = new Font("Segoe UI Symbol", 11f),
                ForeColor = SmartTheme.TextSecondary,
                FillColor = SmartTheme.Surface2,
                Size = new Size(40, 40),
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Margin = new Padding(0)
            };
            btnNotif.HoverState.FillColor = SmartTheme.Surface3;
            btnNotif.HoverState.ForeColor = SmartTheme.TextPrimary;

            // Notification Dot
            var pnlDotBase = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(12, 12),
                Location = new Point(22, 6),
                FillColor = SmartTheme.Surface2,
                BorderRadius = 6,
                UseTransparentBackground = true
            };
            var pnlDot = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(8, 8),
                Location = new Point(2, 2),
                FillColor = SmartTheme.Danger,
                BorderRadius = 4,
                UseTransparentBackground = true
            };
            pnlDotBase.Controls.Add(pnlDot);
            btnNotif.Controls.Add(pnlDotBase);

            pnlActionsFlow.Controls.Add(btnCreateNew);
            pnlActionsFlow.Controls.Add(btnTheme);
            pnlActionsFlow.Controls.Add(btnNotif);

            // 2. Left Section (Breadcrumbs)
            var pnlBreadcrumbsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(24, 28, 0, 0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            var lblTitle = new Label
            {
                Text = "Dashboard",
                Font = SmartTheme.FontHeader,
                ForeColor = SmartTheme.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 0)
            };

            var lblSubtitle = new Label
            {
                Text = "Manage your automation routines",
                Font = SmartTheme.FontBody,
                ForeColor = SmartTheme.TextSecondary,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 0)
            };

            pnlBreadcrumbsFlow.Controls.Add(lblTitle);
            pnlBreadcrumbsFlow.Controls.Add(lblSubtitle);

            pnlContentHeader.Controls.Add(sepBottom);
            pnlContentHeader.Controls.Add(pnlActionsFlow);
            pnlContentHeader.Controls.Add(pnlBreadcrumbsFlow);
            pnlBreadcrumbsFlow.BringToFront();

            pnlMainContent.Controls.Add(pnlContentHeader);
            pnlContentHeader.BringToFront(); // Secure top dock within main content
        }

        private bool _isDragging = false;
        private Point _dragStartPoint;

        private void BindDragLogic(Control ctrl)
        {
            ctrl.DoubleClick += (s, e) =>
            {
                WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
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
                    Location = new Point(p.X - _dragStartPoint.X - ctrl.Left, p.Y - _dragStartPoint.Y - ctrl.Top);
                }
            };

            ctrl.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    _isDragging = false;
            };
        }



        private Image GenerateIcon(string text, float emSize, Color color)
        {
            var bmp = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var font = new Font("Segoe UI Symbol", emSize, FontStyle.Regular))
                using (var brush = new SolidBrush(color))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, brush, new RectangleF(0, -2, 32, 32), format);
                }
            }
            return bmp;
        }

        private void StyleNavButton(Guna2Button btn, string icon, string text)
        {
            btn.Tag = icon; // Store icon string to swap color on active state
            btn.Text = text;
            btn.Image = GenerateIcon(icon, 22f, SmartTheme.TextSecondary); // Render beautiful dynamically scaled large icon
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.ImageOffset = new Point(4, 0); // Spacing layout perfect
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
            btn.Font = SmartTheme.FontBody;
            btn.BorderRadius = 4;
            btn.Animated = true;
            btn.HoverState.FillColor = SmartTheme.Surface2;
        }

        private void btnSidebarCollapse_Click(object sender, EventArgs e)
        {
            _sidebarCollapsed = !_sidebarCollapsed;
            AnimateSidebar(_sidebarCollapsed);
        }

        private void AnimateSidebar(bool collapse)
        {
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
                int diff = targetWidth - current;
                int step = (int)(diff * 0.28);
                if (step == 0 && diff != 0) step = Math.Sign(diff);
                int next = current + step;

                if (Math.Abs(targetWidth - next) <= 1)
                {
                    pnlSidebar.Width = targetWidth;
                    btnSidebarCollapse.Text = chevron;
                    btnSidebarCollapse.Left = targetWidth - 14;
                    timer.Stop();
                    timer.Dispose();
                    return;
                }

                pnlSidebar.Width = next;
                btnSidebarCollapse.Left = next - 14;
            };
            timer.Start();
        }

        private void InitializeEngineHover()
        {
            var hoverTimer = new System.Windows.Forms.Timer { Interval = 16 };
            int currentAnimState = 0; // Anim state range 0 -> 100

            pnlEngineStatusBase.Cursor = Cursors.Hand;
            lblEngineStatus.Cursor = Cursors.Hand;
            lblEngineSubtitle.Cursor = Cursors.Hand;

            hoverTimer.Tick += (s, e) =>
            {
                // Flicker-free universal bounds checking
                Point mouseAt = pnlEngineStatusBase.PointToClient(Cursor.Position);
                bool isHovering = pnlEngineStatusBase.ClientRectangle.Contains(mouseAt) && !_sidebarCollapsed;

                if (isHovering && currentAnimState < 100) currentAnimState += 12;
                else if (!isHovering && currentAnimState > 0) currentAnimState -= 10;

                if (currentAnimState < 0) currentAnimState = 0;
                if (currentAnimState > 100) currentAnimState = 100;

                // Color lightup calculation
                int baseRgb = 32;
                int brightOffset = (int)(currentAnimState * 0.15); // max 15 brightness
                pnlEngineStatusBase.FillColor = Color.FromArgb(baseRgb + brightOffset, baseRgb + brightOffset + 2, baseRgb + brightOffset + 7);

                // Emulate physical scaling by compressing padding uniformly
                int shift = currentAnimState / 33; // max shift 3 pixels
                pnlEngineStatusBase.Padding = new Padding(shift);
            };
            hoverTimer.Start();
        }
    }
}