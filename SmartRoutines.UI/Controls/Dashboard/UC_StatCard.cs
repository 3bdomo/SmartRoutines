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
            InitializeComponent(); // MUST keep designer components
            this.SuspendLayout();
            
            this.Size = new Size(220, 110);
            this.MinimumSize = new Size(180, 110);
            this.BackColor = Color.Transparent;
            this.Margin = new Padding(0, 0, 20, 20);

            // Configure the Designer instances rather than replacing them!
            pnlCard.BorderRadius = 8;
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
            }
        }

        [Category("Appearance")]
        public Color CardColor
        {
            get => _cardColor;
            set
            {
                _cardColor = value;
                pnlCard.FillColor = value;
                lblTitle.BackColor = value;
                lblValue.BackColor = value;
            }
        }

        private void ApplyTheme()
        {
            pnlCard.FillColor = _cardColor;
            lblTitle.BackColor = _cardColor;
            lblValue.BackColor = _cardColor;

            pnlCard.BorderColor = _accentColor;
            pnlCard.CustomBorderColor = _accentColor;

            lblTitle.Font = SmartTheme.FontSmallBold;
            lblTitle.ForeColor = SmartTheme.TextSecondary;
            
            lblValue.Font = new Font("Segoe UI", 28f, FontStyle.Bold);
            lblValue.ForeColor = SmartTheme.TextPrimary;
        }
    }
}
