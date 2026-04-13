using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_RoutineCard : SmartUserControl
    {
        public event EventHandler StateChanged;

        private bool _isActive = true;
        private bool _isRunning = false;

        private Guna.UI2.WinForms.Guna2GradientPanel _pnlIcon;
        private Guna.UI2.WinForms.Guna2Panel _pnlScheduleRegion;

        public UC_RoutineCard()
        {
            InitializeComponent();
            this.MinimumSize = new Size(320, 275);
            
            // Upgrade Primitive Icon to Figma Gradient Block
            _pnlIcon = new Guna.UI2.WinForms.Guna2GradientPanel
            {
                Size = new Size(46, 46),
                Location = new Point(24, 24),
                BorderRadius = 12,
                FillColor = Color.FromArgb(99, 102, 241),
                FillColor2 = Color.FromArgb(56, 189, 248),
                GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
            };
            var iconLbl = new Label { Name="iconEmoji", Text = "✨", Font = new Font("Segoe UI Emoji", 14f), AutoSize = true, BackColor = Color.Transparent, ForeColor = Color.White };
            iconLbl.Location = new Point(10, 10);
            _pnlIcon.Controls.Add(iconLbl);

            pnlBase.Controls.Remove(pbIcon);
            pnlBase.Controls.Add(_pnlIcon);

            // Upgrade harsh gray scheduled block to modern rounded inset
            _pnlScheduleRegion = new Guna.UI2.WinForms.Guna2Panel
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

            // Reroute labels into the new rounded panel for perfect alpha transparency support
            pnlBase.Controls.Remove(lblSchedule);
            pnlBase.Controls.Remove(lblActions);
            lblSchedule.Location = new Point(10, 8);
            lblActions.Location = new Point(10, 30);
            _pnlScheduleRegion.Controls.Add(lblSchedule);
            _pnlScheduleRegion.Controls.Add(lblActions);

            ApplyTheme();
        }

        [Category("Routine Data")]
        public string RoutineName
        {
            get => lblName.Text;
            set
            {
                lblName.Text = value;
                UpdateDynamicIcon();
            }
        }

        private void UpdateDynamicIcon()
        {
            if (_pnlIcon == null || _pnlIcon.Controls.Count == 0) return;
            var lbl = _pnlIcon.Controls["iconEmoji"] as Label;
            if (lbl == null) return;

            string name = lblName.Text.ToLower();
            if (name.Contains("morning"))
            {
                lbl.Text = "☀️";
                _pnlIcon.FillColor = Color.FromArgb(0, 120, 212);
                _pnlIcon.FillColor2 = Color.FromArgb(100, 170, 255);
            }
            else if (name.Contains("focus"))
            {
                lbl.Text = "🎯";
                _pnlIcon.FillColor = Color.FromArgb(139, 92, 246);
                _pnlIcon.FillColor2 = Color.FromArgb(236, 72, 153);
            }
            else if (name.Contains("evening") || name.Contains("shut"))
            {
                lbl.Text = "🌙";
                _pnlIcon.FillColor = Color.FromArgb(30, 41, 59);
                _pnlIcon.FillColor2 = Color.FromArgb(15, 23, 42);
            }
            else
            {
                lbl.Text = "⚡";
                _pnlIcon.FillColor = SmartTheme.Primary;
                _pnlIcon.FillColor2 = SmartTheme.Purple;
            }
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
            set => lblSchedule.Text = "⏱️ " + value;
        }

        [Category("Routine Data")]
        public string ActionCountText
        {
            get => lblActions.Text;
            set => lblActions.Text = value;
        }
        
        [Category("Routine Data")]
        public string LastRunText
        {
            get => lblLastRun.Text;
            set => lblLastRun.Text = value;
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
                if (_isRunning != value)
                {
                    _isRunning = value;
                    UpdateStateStyle();
                    StateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = Color.Transparent;

            // Base Panel
            pnlBase.FillColor = SmartTheme.Surface;
            pnlBase.Radius = SmartTheme.RadiusLarge;
            pnlBase.ShadowColor = Color.Black;
            pnlBase.ShadowDepth = 40;

            // Typography
            lblName.Font = SmartTheme.FontSubheader;
            lblName.ForeColor = SmartTheme.TextPrimary;

            lblDescription.Font = SmartTheme.FontBody;
            lblDescription.ForeColor = SmartTheme.TextSecondary;
            
            lblSchedule.Font = SmartTheme.FontBody;
            lblSchedule.ForeColor = SmartTheme.TextSecondary;
            lblSchedule.BackColor = Color.Transparent; // Draw cleanly over new dark region

            lblActions.Font = SmartTheme.FontBody;
            lblActions.ForeColor = SmartTheme.TextSecondary;
            lblActions.BackColor = Color.Transparent; // Draw cleanly over new dark region

            lblLastRun.Font = SmartTheme.FontBody;
            lblLastRun.ForeColor = SmartTheme.Success; // Default

            lblActive.Font = SmartTheme.FontBody;
            lblActive.ForeColor = SmartTheme.TextSecondary;

            // Mini Buttons (Edit/Delete) - Interactive Hover states
            btnEdit.FillColor = Color.Transparent;
            btnEdit.ForeColor = SmartTheme.TextSecondary;
            btnEdit.HoverState.FillColor = SmartTheme.Surface2;
            btnEdit.HoverState.ForeColor = SmartTheme.TextPrimary;

            btnDelete.FillColor = Color.Transparent;
            btnDelete.ForeColor = SmartTheme.TextSecondary;
            btnDelete.HoverState.FillColor = SmartTheme.Danger;
            btnDelete.HoverState.ForeColor = SmartTheme.TextPrimary;

            // Card Hover Interactivity
            pnlBase.MouseEnter += PnlBase_MouseEnter;
            pnlBase.MouseLeave += PnlBase_MouseLeave;

            // Run Button Interaction
            btnRunNow.Click += BtnRunNow_Click;

            UpdateStateStyle();
        }

        private void PnlBase_MouseEnter(object sender, EventArgs e)
        {
            if (!_isRunning)
                pnlBase.ShadowDepth = 80; // Pop out slightly on hover
        }

        private void PnlBase_MouseLeave(object sender, EventArgs e)
        {
            if (!_isRunning)
                pnlBase.ShadowDepth = 40; // Return to normal
        }

        private void BtnRunNow_Click(object sender, EventArgs e)
        {
            // Toggle the running state to demonstrate interactivity visually!
            IsRunning = !IsRunning;
        }

        private void UpdateStateStyle()
        {
            // Toggle Switch Visuals
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

            // Running vs Normal State
            if (_isRunning)
            {
                // Guna2ShadowPanel doesn't have borders, so we use a glowing shadow effect!
                pnlBase.ShadowColor = SmartTheme.Danger; 
                pnlBase.ShadowDepth = 150; // Increased shadow depth for "glow"

                btnRunNow.FillColor = SmartTheme.Surface2; // Dark gray body
                btnRunNow.ForeColor = SmartTheme.Danger; // Red text
                btnRunNow.HoverState.FillColor = Color.FromArgb(50, 30, 30); // Very dark red hover
                btnRunNow.Text = "⏹ Stop";
            }
            else
            {
                pnlBase.ShadowColor = Color.Black; // Normal shadow
                pnlBase.ShadowDepth = 40;

                btnRunNow.FillColor = SmartTheme.Primary; // Blue primary
                btnRunNow.ForeColor = SmartTheme.TextPrimary;
                btnRunNow.HoverState.FillColor = SmartTheme.PrimaryHover; // Lighter blue hover
                btnRunNow.Text = "▶ Run Now";
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
    }
}
