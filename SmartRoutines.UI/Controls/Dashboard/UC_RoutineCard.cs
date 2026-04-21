using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;
using SmartRoutines.UI.Controls.AddRoutine.UC_Step3;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_RoutineCard : SmartUserControl
    {
        public event EventHandler StateChanged = null!;
        public event EventHandler DeleteRequested = null!;

        private bool _isActive = true;
        private bool _isRunning = false;
        private Guid _id;

        [Category("Routine Properties")]
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        private Guna2GradientPanel _pnlIcon = null!;
        private Guna2Panel _pnlScheduleRegion = null!;

        // Font cache — prevents GDI+ handle leaks from creating new Font objects on every resize tick
        private float _cachedScaleFactor = -1f;
        private Font? _cachedFontSubheader;
        private Font? _cachedFontBody;

        public UC_RoutineCard()
        {
            InitializeComponent();

            // Performance: High quality rendering styles
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            this.MinimumSize = new Size(260, 200);
            this.BackColor = Color.Transparent;

            BuildGradientIcon();
            BuildScheduleRegion();
            ApplyTheme();
        }

        // ─── Gradient icon block ───────────────────────────────────────────
        private void BuildGradientIcon()
        {
            _pnlIcon = new Guna2GradientPanel
            {
                Size = new Size(52, 52),
                Location = new Point(20, 20),
                BorderRadius = 14,
                FillColor = Color.FromArgb(99, 102, 241),
                FillColor2 = Color.FromArgb(56, 189, 248),
                GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
            };

            _pnlIcon.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            var iconPic = new PictureBox
            {
                Name = "iconPic",
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(26, 26),
                BackColor = Color.Transparent,
                Location = new Point(13, 13)
            };
            _pnlIcon.Controls.Add(iconPic);

            pnlBase.Controls.Remove(pbIcon);
            pnlBase.Controls.Add(_pnlIcon);
        }

        // ─── Rounded schedule region ───────────────────────────────────────
        private void BuildScheduleRegion()
        {
            _pnlScheduleRegion = new Guna2Panel
            {
                Size = pnlDivider.Size,
                Location = pnlDivider.Location,
                FillColor = Color.FromArgb(24, 24, 27),
                BorderRadius = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            pnlBase.Controls.Remove(pnlDivider);
            pnlBase.Controls.Add(_pnlScheduleRegion);
            _pnlScheduleRegion.SendToBack();

            pnlBase.Controls.Remove(lblSchedule);
            pnlBase.Controls.Remove(lblActions);
            lblSchedule.Location = new Point(10, 8);
            lblActions.Location = new Point(10, 30);
            _pnlScheduleRegion.Controls.Add(lblSchedule);
            _pnlScheduleRegion.Controls.Add(lblActions);
        }

        // ─── Properties ───────────────────────────────────────────────────
        [Category("Routine Data")]
        public string RoutineName
        {
            get => lblName.Text;
            set { lblName.Text = value; UpdateDynamicIcon(); }
        }

        [Category("Routine Data")]
        public string Description
        {
            get => lblDescription.Text;
            set => lblDescription.Text = value;
        }

        [Category("Routine Data")]
        public string ScheduleText
        {
            get => lblSchedule.Text;
            set => lblSchedule.Text = value;
        }

        [Category("Routine Data")]
        public string ActionCountText
        {
            get => lblActions.Text;
            set => lblActions.Text = value;
        }

        private PictureBox? _statusIconPic;
        [Category("Routine Data")]
        public string LastRunText
        {
            get => lblLastRun.Text;
            set
            {
                if (_statusIconPic == null)
                {
                    _statusIconPic = new PictureBox
                    {
                        Size = new Size(16, 16),
                        Location = new Point(lblLastRun.Left, lblLastRun.Top + 2),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BackColor = Color.Transparent
                    };
                    pnlBase.Controls.Add(_statusIconPic);
                    _statusIconPic.BringToFront();
                    lblLastRun.Left += 22;
                }

                if (value.StartsWith("✔ ") || value.ToLower().Contains("success"))
                {
                    lblLastRun.Text = value.StartsWith("✔ ") ? value.Substring(2) : value;
                    _statusIconPic.Visible = true;
                    _statusIconPic.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("success.png", 16);
                }
                else if (value.StartsWith("✖ ") || value.ToLower().Contains("error"))
                {
                    lblLastRun.Text = value.StartsWith("✖ ") ? value.Substring(2) : value;
                    _statusIconPic.Visible = true;
                    _statusIconPic.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("error.png", 16);
                }
                else
                {
                    lblLastRun.Text = value;
                    _statusIconPic.Visible = false;
                }
                lblLastRun.ForeColor = SmartTheme.TextSecondary;
            }
        }

        [Category("Routine State")]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                toggleActive.Checked = value;
                UpdateStateStyle();
            }
        }

        [Category("Routine State")]
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                UpdateStateStyle();
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // ─── Dynamic icon based on routine name ────────────────────────────
        private void UpdateDynamicIcon()
        {
            if (_pnlIcon == null || _pnlIcon.Controls.Count == 0) return;
            var pic = _pnlIcon.Controls["iconPic"] as PictureBox;
            if (pic == null) return;

            string name = lblName.Text.ToLower();
            if (name.Contains("morning"))
            {
                pic.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("activity.png", 24);
                _pnlIcon.FillColor = Color.FromArgb(0, 120, 212);
                _pnlIcon.FillColor2 = Color.FromArgb(100, 170, 255);
            }
            else if (name.Contains("focus"))
            {
                pic.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("dashboard.png", 24);
                _pnlIcon.FillColor = Color.FromArgb(139, 92, 246);
                _pnlIcon.FillColor2 = Color.FromArgb(236, 72, 153);
            }
            else
            {
                pic.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("dashboard.png", 24);
                _pnlIcon.FillColor = SmartTheme.Primary;
                _pnlIcon.FillColor2 = SmartTheme.Purple;
            }
        }

        // ─── Theme ────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            this.BackColor = Color.Transparent;

            pnlBase.FillColor = SmartTheme.Surface;
            pnlBase.BorderRadius = 16;

            // PERF FIX: Reduced shadow depth from 40 → 12 and padding from (0,0,10,10) → (0,0,4,4)
            // Heavy shadow was the main cause of slow window transitions and minimize animation
            pnlBase.ShadowDecoration.Enabled = true;
            pnlBase.ShadowDecoration.Color = Color.Black;
            pnlBase.ShadowDecoration.Depth = 12;
            pnlBase.ShadowDecoration.Shadow = new Padding(0, 0, 4, 4);

            lblName.Font = SmartTheme.FontSubheader;
            lblName.ForeColor = SmartTheme.TextPrimary;
            lblName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblName.Location = new Point(_pnlIcon.Right + 12, _pnlIcon.Top + 14);

            lblDescription.Font = SmartTheme.FontBody;
            lblDescription.ForeColor = SmartTheme.TextSecondary;
            lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDescription.Location = new Point(20, _pnlIcon.Bottom + 12);

            lblSchedule.Font = SmartTheme.FontBody;
            lblSchedule.ForeColor = SmartTheme.TextSecondary;
            lblSchedule.BackColor = Color.Transparent;

            lblActions.Font = SmartTheme.FontBody;
            lblActions.ForeColor = SmartTheme.TextSecondary;
            lblActions.BackColor = Color.Transparent;

            lblLastRun.Font = SmartTheme.FontBody;
            lblLastRun.ForeColor = SmartTheme.TextSecondary;

            lblActive.Font = SmartTheme.FontBody;
            lblActive.ForeColor = SmartTheme.TextSecondary;

            btnEdit.FillColor = Color.Transparent;
            btnEdit.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("edit.png", 18);
            btnEdit.Text = "";
            btnEdit.HoverState.FillColor = SmartTheme.Surface2;

            btnDelete.FillColor = Color.Transparent;
            btnDelete.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("trash_clean.png", 18);
            btnDelete.Text = "";
            btnDelete.HoverState.FillColor = SmartTheme.Surface2;
            btnDelete.Click += (s, e) => DeleteRequested?.Invoke(this, EventArgs.Empty);

            // PERF FIX: Removed hover shadow depth change (was triggering full repaint on every MouseEnter/Leave)
            // pnlBase.MouseEnter and MouseLeave shadow updates removed intentionally

            pnlBase.Paint += PnlBase_Paint;
            btnRunNow.Click += BtnRunNow_Click;
            btnEdit.Click += BtnEdit_Click;

            UpdateStateStyle();
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (this.FindForm() is SmartRoutines.UI.Forms.FrmMain main)
            {
                main.DisplayPage<UC_ActionsMain>(async wizard => 
                {
                    await wizard.LoadRoutineForEdit(this.Id);
                });
            }
        }

        private void PnlBase_Paint(object? sender, PaintEventArgs e)
        {
            // Manual drawing removed in favor of native Guna2 border properties
        }

        private void BtnRunNow_Click(object? sender, EventArgs e)
        {
            IsRunning = !IsRunning;
        }

        // ─── State style ──────────────────────────────────────────────────
        private void UpdateStateStyle()
        {
            if (_isActive)
            {
                toggleActive.CheckedState.FillColor = SmartTheme.Primary;
                lblActive.Text = "Active";
                lblActive.ForeColor = SmartTheme.TextPrimary;
            }
            else
            {
                toggleActive.UncheckedState.FillColor = SmartTheme.Surface2;
                lblActive.Text = "Inactive";
                lblActive.ForeColor = SmartTheme.TextSecondary;
            }

            if (_isRunning)
            {
                pnlBase.BorderThickness = 2;
                pnlBase.BorderColor = SmartTheme.Danger;

                // PERF FIX: Reduced shadow depth from 40 → 12
                pnlBase.ShadowDecoration.Depth = 12;
                pnlBase.ShadowDecoration.Color = Color.Black;
                pnlBase.Invalidate();

                btnRunNow.FillColor = SmartTheme.Danger;
                btnRunNow.ForeColor = Color.White;
                btnRunNow.HoverState.FillColor = Color.FromArgb(220, 38, 38);
                btnRunNow.Text = "Stop";
                btnRunNow.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("stop.png", 18);
                btnRunNow.ImageOffset = new Point(0, 0);
            }
            else
            {
                pnlBase.BorderThickness = 0;
                pnlBase.Invalidate();

                pnlBase.ShadowDecoration.Color = Color.Black;
                // PERF FIX: Reduced shadow depth from 40 → 12
                pnlBase.ShadowDecoration.Depth = 12;

                if (_isActive)
                {
                    btnRunNow.FillColor = SmartTheme.Primary;
                    btnRunNow.ForeColor = SmartTheme.TextPrimary;
                    btnRunNow.HoverState.FillColor = SmartTheme.PrimaryHover;
                    btnRunNow.Image = null;
                }
                else
                {
                    btnRunNow.FillColor = SmartTheme.Surface3;
                    btnRunNow.ForeColor = SmartTheme.TextSecondary;
                    btnRunNow.HoverState.FillColor = SmartTheme.Surface2;
                }
                btnRunNow.Text = "Run Now";
                btnRunNow.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("play.png", 18);
                btnRunNow.ImageOffset = new Point(0, 0);
                btnRunNow.BorderRadius = 20;
            }
        }

        private void toggleActive_CheckedChanged(object sender, EventArgs e)
        {
            if (_isActive != toggleActive.Checked)
            {
                _isActive = toggleActive.Checked;
                UpdateStateStyle();
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ScaleUI(float factor)
        {
            if (Math.Abs(factor - _cachedScaleFactor) < 0.01f) return;
            _cachedScaleFactor = factor;

            _cachedFontSubheader?.Dispose();
            _cachedFontBody?.Dispose();
            _cachedFontSubheader = new Font(SmartTheme.FontSubheader.FontFamily,
                SmartTheme.FontSubheader.Size * factor, SmartTheme.FontSubheader.Style);
            _cachedFontBody = new Font(SmartTheme.FontBody.FontFamily,
                SmartTheme.FontBody.Size * factor, SmartTheme.FontBody.Style);

            this.SuspendLayout();

            lblName.Font = _cachedFontSubheader;
            lblDescription.Font = _cachedFontBody;
            lblSchedule.Font = _cachedFontBody;
            lblActions.Font = _cachedFontBody;
            lblLastRun.Font = _cachedFontBody;
            btnRunNow.Font = _cachedFontBody;

            if (_pnlIcon != null)
            {
                int iconSize = (int)(46 * factor);
                _pnlIcon.Size = new Size(iconSize, iconSize);
                _pnlIcon.BorderRadius = (int)(14 * factor);
            }
            btnRunNow.BorderRadius = (int)(20 * factor);

            this.ResumeLayout(false);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _cachedFontSubheader?.Dispose();
            _cachedFontSubheader = null;
            _cachedFontBody?.Dispose();
            _cachedFontBody = null;
            base.OnHandleDestroyed(e);
        }
    }
}