using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_Dashboard : SmartUserControl
    {
        public UC_Dashboard()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = SmartTheme.Background;
            lblTitle.Font = SmartTheme.FontHeader;
            lblTitle.ForeColor = SmartTheme.TextPrimary;
            lblSubtitle.Font = SmartTheme.FontBody;
            lblSubtitle.ForeColor = SmartTheme.TextSecondary;
            lblYourRoutines.Font = SmartTheme.FontHeader;
            lblYourRoutines.ForeColor = SmartTheme.TextPrimary;
        }
    }
}
