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
            ApplyTheme();
        }

        public void DisplayPage(UserControl page)
        {
            if (pnlMainContent.Controls.Count > 0)
            {
                var oldPage = pnlMainContent.Controls[0];
                pnlMainContent.Controls.Remove(oldPage);
                oldPage.Dispose();
            }
            page.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(page);
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
