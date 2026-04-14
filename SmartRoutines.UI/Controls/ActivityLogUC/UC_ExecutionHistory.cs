using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_ExecutionHistory : SmartUserControl
    {

        public UC_ExecutionHistory()
        {
            InitializeComponent();
            // The updated Figma design displays this subheader prominently
            pnlHeader.Visible = true;
            pnlHeader.BackColor = Color.Transparent;
            lblTitle.Text = "Execution History";
            lblTitle.Font = SmartTheme.FontBodyBold;
            lblTitle.ForeColor = Color.White;
            pnlHeader.Padding = new Padding(24, 20, 0, 10);
            
            // ── Content ──
            this.BackColor = Color.Transparent;
            pnlContent.BackColor = Color.Transparent;
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(20);

            flowItems.BackColor = Color.Transparent;
            flowItems.Dock = DockStyle.Fill;
            flowItems.FlowDirection = FlowDirection.TopDown;
            flowItems.WrapContents = false;
            flowItems.AutoScroll = true;
            flowItems.Padding = new Padding(0);

            // Responsive auto size for wide log cards
            flowItems.SizeChanged += (s, e) =>
            {
                foreach (Control c in flowItems.Controls)
                    c.Width = flowItems.ClientSize.Width - flowItems.Padding.Horizontal - 20; // 20px padding
            };
        }

        public void LoadLogs(List<SmartRoutines.Core.Domain.Entities.ActivityLog> logs)
        {
            flowItems.SuspendLayout();
            flowItems.Controls.Clear();
            foreach (var log in logs)
            {
                var item = new UC_LogItem(log)
                {
                    Width = flowItems.ClientSize.Width - flowItems.Padding.Horizontal - 20,
                    Margin = new Padding(0) // Figma uses direct stacking without gaps
                };
                flowItems.Controls.Add(item);
            }
            flowItems.ResumeLayout(true);
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
