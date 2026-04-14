using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_StatCard : SmartUserControl
    {
        private Color _accentColor = SmartTheme.Primary;
        private Color _cardColor = SmartTheme.Surface;

        public UC_StatCard()
        {
            InitializeComponent(); 
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.UserPaint | 
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            this.SuspendLayout();
            
            this.Size = new Size(220, 110);
            this.MinimumSize = new Size(180, 110);
            this.BackColor = Color.Transparent;
            this.Margin = new Padding(0, 0, 20, 20);

            // Configure the Designer instances rather than replacing them!
            pnlCard.BorderRadius = 20;
            pnlCard.CustomBorderThickness = new Padding(0, 4, 0, 0);
            pnlCard.UseTransparentBackground = false; // Disable transparency bug
            
            lblTitle.BackColor = Color.Transparent; // Draws exactly over the panel's custom color
            lblValue.BackColor = Color.Transparent;
            
            ApplyTheme();
            this.ResumeLayout(false);
        }

        [Category("Data")]
        public string Title
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        [Category("Data")]
        public string Value
        {
            get => lblValue.Text;
            set => lblValue.Text = value;
        }

        [Category("Appearance")]
        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                pnlCard.BorderColor = value;
                pnlCard.CustomBorderColor = value; 
                lblTitle.ForeColor = value;
            }
        }

        [Category("Appearance")]
        public int BorderRadius
        {
            get => pnlCard.BorderRadius;
            set => pnlCard.BorderRadius = value;
        }

        [Category("Appearance")]
        public Color CardColor
        {
            get => _cardColor;
            set
            {
                _cardColor = value;
                pnlCard.FillColor = value;
                // Labels must stay Transparent — setting them to the card color
                // creates visible rectangles on Guna2Panel's rounded corners.
                lblTitle.BackColor = Color.Transparent;
                lblValue.BackColor = Color.Transparent;
            }
        }

        [Category("Appearance")]
        public string CardIcon
        {
            set => pbIcon.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon(value, 32);
        }

        private void ApplyTheme()
        {
            pnlCard.FillColor = _cardColor;
            lblTitle.BackColor = Color.Transparent;
            lblValue.BackColor = Color.Transparent;

            pnlCard.BorderColor = _accentColor;
            pnlCard.CustomBorderColor = _accentColor;

            lblTitle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblTitle.ForeColor = _accentColor;
            lblTitle.Location = new Point(20, 18);
            
            lblValue.Font = new Font("Segoe UI", 30f, FontStyle.Bold);
            lblValue.ForeColor = Color.White;
            lblValue.Location = new Point(14, 46);

            pbIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pbIcon.BackColor = Color.Transparent;
            pbIcon.Size = new Size(32, 32);

            lblTitle.BringToFront();
            lblValue.BringToFront();
            pbIcon.BringToFront();
        }
    }
}
