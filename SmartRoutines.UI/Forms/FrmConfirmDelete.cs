using System;
using System.Drawing;
using System.Windows.Forms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Forms
{
    public partial class FrmConfirmDelete : Form
    {
        public bool Confirmed { get; private set; } = false;

        public FrmConfirmDelete(string routineName)
        {
            InitializeComponent();
            lblTitle.Text = "Delete Routine";
            lblMessage.Text = $"Are you sure you want to permanently delete \"{routineName}\"? This action cannot be undone.";
            
            this.BackColor = SmartTheme.Background;
            pnlCard.FillColor = SmartTheme.Surface;
            
            btnCancel.FillColor = SmartTheme.Surface3;
            btnCancel.ForeColor = SmartTheme.TextSecondary;
            
            btnDelete.FillColor = SmartTheme.Danger;
            btnDelete.ForeColor = Color.White;

            ApplyTheme();
        }

        private void ApplyTheme()
        {
            lblTitle.Font = SmartTheme.FontSubheader;
            lblTitle.ForeColor = SmartTheme.TextPrimary;

            lblMessage.Font = SmartTheme.FontBody;
            lblMessage.ForeColor = SmartTheme.TextSecondary;

            btnCancel.Font = SmartTheme.FontBody;
            btnDelete.Font = SmartTheme.FontBodyBold;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Confirmed = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public static bool Show(string routineName, Form parent)
        {
            using (var overlay = new Form())
            {
                overlay.StartPosition = FormStartPosition.Manual;
                overlay.FormBorderStyle = FormBorderStyle.None;
                overlay.Opacity = 0.50d;
                overlay.BackColor = Color.Black;
                overlay.Size = parent.ClientSize;
                overlay.Location = parent.PointToScreen(Point.Empty);
                overlay.ShowInTaskbar = false;
                overlay.Show(parent);

                using (var frm = new FrmConfirmDelete(routineName))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    var result = frm.ShowDialog(overlay);
                    return result == DialogResult.OK;
                }
            }
        }
    }
}
