using SmartRoutines.Core.Enums;
using SmartRoutines.Core.Models;
using SmartRoutines.UI.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_ExecutionHistory : SmartUserControl   
    {
      
        public UC_ExecutionHistory()
        {
            InitializeComponent();
            pnlHeader.BackColor = SmartTheme.Surface;
            lblTitle.Text = "Execution History";
            lblTitle.Font = SmartTheme.FontSubheader;
            lblTitle.ForeColor = SmartTheme.TextPrimary;

            // ── Content ──
            pnlContent.BackColor = SmartTheme.Background;
            flowItems.BackColor = SmartTheme.Background;
            flowItems.FlowDirection = FlowDirection.TopDown;
            flowItems.WrapContents = false;
            flowItems.AutoScroll = true;
            flowItems.Padding = new Padding(0);
        }
        
        //public void LoadLogs(List<ActivityLog> logs)
        //{
        //    flowItems.Controls.Clear();
        //    foreach (var log in logs)
        //    {
        //        var item = new UC_LogItem(log);
        //        // ✅ بدل flowItems.Width استخدم رقم ثابت
        //        item.Width = 860;
        //        item.Margin = new Padding(0, 0, 0, 4);
        //        flowItems.Controls.Add(item);
        //    }
        //}
        // ppendLine("Error: backup.bat not found", LogStatus.Error);
    }
    }
