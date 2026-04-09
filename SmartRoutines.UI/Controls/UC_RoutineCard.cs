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
        private bool _isActive = true;
        private bool _isRunning = false;

        public UC_RoutineCard()
        {
            InitializeComponent();
            ApplyTheme();
        }

        [Category("Routine Data")]
        public string RoutineName
        {
            get => lblName.Text;
            set => lblName.Text = value;
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
                _isRunning = value;
                UpdateStateStyle();
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

            lblActions.Font = SmartTheme.FontBody;
            lblActions.ForeColor = SmartTheme.TextSecondary;

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
            _isActive = toggleActive.Checked;
            UpdateStateStyle();
        }
    }
}
