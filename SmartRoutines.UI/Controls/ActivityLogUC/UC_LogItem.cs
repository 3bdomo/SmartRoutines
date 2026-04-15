using Microsoft.VisualBasic.Logging;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.UI.Core.Healper;
using SmartRoutines.UI.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_LogItem : SmartUserControl
    {
        public UC_LogItem()
        {
            InitializeComponent();
        }
        public UC_LogItem(ActivityLog log) : this()
        {


            RoutenNameLab.Text = log.RoutineName;
            lblDetails.Text = log.Message;
            lblTime.Text = log.CreatedAt.ToString("h:mm:ss tt");
            lblSummaryText.Text = log.Message; // جوا الـ pnlSummary

            // ── الخطوط ──
            RoutenNameLab.Font = SmartTheme.FontBodyBold;
            RoutenNameLab.ForeColor = SmartTheme.TextPrimary;

            lblDetails.Font = SmartTheme.FontCaption;
            lblDetails.ForeColor = SmartTheme.TextSecondary;

            lblTime.Font = SmartTheme.FontCaption;
            lblTime.ForeColor = SmartTheme.TextMuted;
            pnlSummary.BackColor = SmartTheme.Surface2;
            pnlSummary.BorderRadius = 10;
            lblSummaryText.ForeColor= SmartTheme.TextMuted;


            // ── Guna2CirclePictureBox ──

            picStatus.Size = new Size(50, 50);
            picStatus.SizeMode = PictureBoxSizeMode.Zoom;
            picStatus.Padding = new Padding(0);
            picStatus.BackColor = Color.Transparent;

            switch (log.Status)
            {
                case LogStatus.Success:
                    pnlIcon.FillColor = Color.FromArgb(76, 175, 80); // Figma Green
                    picStatus.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("status_ok.png", 18);
                    break;

                case LogStatus.Error:
                    pnlIcon.FillColor = SmartTheme.Danger;
                    picStatus.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("status_error.png", 18);
                    break;

                case LogStatus.Warning:
                    pnlIcon.FillColor = SmartTheme.Warning;
                    picStatus.Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("status_error.png", 18); 
                    break;

                default:
                    pnlIcon.FillColor = SmartTheme.TextMuted;
                    break;
            }
            // -----------------------------------


            // ── Summary Panel ──
            //pnlSummary.BackColor = SmartTheme.Surface;
            //pnlSummary.BorderRadius = 10;
            //lblSummaryText.Text = log.Message;
            //lblSummaryText.Font = SmartTheme.FontMono;
            //lblSummaryText.ForeColor = SmartTheme.TextSecondary;

            // ── Hover ──
            this.MouseEnter += (s, e) => this.BackColor = SmartTheme.Surface2;
            this.MouseLeave += (s, e) => this.BackColor = Color.Transparent;

            TooltipHelper.Set(this,
            $"<b>{log.RoutineName}</b><br>{log.Message}");

            // ── Draw Figma Separator Line ──
            this.Paint += (s, e) =>
            {
                using (var pen = new Pen(SmartTheme.Border, 1))
                {
                    e.Graphics.DrawLine(pen, 0, this.Height - 1, this.Width, this.Height - 1);
                }
            };
        }

        private void RoutenNameLab_Click(object sender, EventArgs e)
        {
            //SmartTheme.FontBodyBold;
        }

        private void lblDetails_Click(object sender, EventArgs e)
        {

        }
    }
}
