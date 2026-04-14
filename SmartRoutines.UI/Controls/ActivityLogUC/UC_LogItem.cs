using Microsoft.VisualBasic.Logging;
using SmartRoutines.Core.Enums;
using SmartRoutines.Core.Models;
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


            // ── Guna2CirclePictureBox ──

            picStatus.Size = new Size(50, 50);
            picStatus.SizeMode = PictureBoxSizeMode.Zoom;
            picStatus.Padding = new Padding(8);

            switch (log.Status)
            {
                case LogStatus.Success:
                    //  picStatus.FillColor = SmartTheme.Success;
                    picStatus.BackColor = SmartTheme.Success;
                    picStatus.Image = Properties.Resources.check_circle;


                    break;

                case LogStatus.Error:
                    picStatus.BackColor = SmartTheme.Danger;
                    picStatus.Image = Properties.Resources.cross_circle;
                    break;

                //case LogStatus.Warning:
                //    picStatus.BackColor = SmartTheme.Warning;
                //    picStatus.Image = Properties.Resources.cross_circle;
                //    break;

                default:
                    picStatus.BackColor = SmartTheme.TextMuted;
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
            this.MouseEnter += (s, e) => this.BackColor = SmartTheme.Surface;
            this.MouseLeave += (s, e) => this.BackColor = SmartTheme.Surface2;

            TooltipHelper.Set(this,
            $"<b>{log.RoutineName}</b><br>{log.Message}");

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
