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
            ApplyTheme();
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
                pnlCard.CustomBorderThickness = new Padding(0, 4, 0, 0); // Thicker top border for highlight
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
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = Color.Transparent;

            // Card Panel Styling
            pnlCard.FillColor = _cardColor;
            pnlCard.BorderColor = _accentColor;
            pnlCard.BorderRadius = SmartTheme.RadiusSmall;
            pnlCard.CustomBorderThickness = new Padding(0, 4, 0, 0); 
            pnlCard.CustomBorderColor = _accentColor;

            // Typography
            lblTitle.Font = SmartTheme.FontBody;
            lblTitle.ForeColor = SmartTheme.TextSecondary;
            
            lblValue.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
            lblValue.ForeColor = SmartTheme.TextPrimary;
        }
    }
}
