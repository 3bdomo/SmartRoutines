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
        private const int ItemHeight = 286; 
        private const int MaxVisible = 3;   
        private const int EmptyHeight = 120;
        public UC_ExecutionHistory()
        {
            InitializeComponent();
            pnlHeader.BackColor = SmartTheme.Surface2;
            lblTitle.Text = "Execution History";
            lblTitle.Font = SmartTheme.FontSubheader;
            lblTitle.ForeColor = SmartTheme.TextPrimary;

            // ── Content ──
           pnlContent.BackColor = SmartTheme.Surface;
            flowItems.BackColor = SmartTheme.Surface;
            flowItems.FlowDirection = FlowDirection.TopDown;
            flowItems.WrapContents = false;
            flowItems.AutoScroll = true;
            flowItems.Padding = new Padding(0);
            SetHeight(EmptyHeight);
        }

        public void LoadLogs(List<ActivityLog> logs)
        {
            flowItems.Controls.Clear();
            if (!logs.Any())
            {
                ShowEmptyState();
                return;
            }
            foreach (var log in logs)
            {
                var item = new UC_LogItem(log);
                item.Width = flowItems.Width-25 ;
                flowItems.Controls.Add(item);
            }
            // ── ضبط الارتفاع حسب عدد الـ Items ──
            int visibleCount = Math.Min(logs.Count, MaxVisible);
            SetHeight(visibleCount * ItemHeight);
        }

        //private void ShowEmptyState()
        //{
        //    flowItems.Controls.Clear();

        //    // 1. إنشاء حاوية (Panel) بعرض الـ Control الكلي
        //    var pnlEmpty = new Panel
        //    {
        //        // العرض لازم يكون نفس عرض الـ flowItems عشان التوسيط يظبط
        //        Size = new Size(flowItems.Width - 10, EmptyHeight),
        //        BackColor = Color.Transparent // خليه شفاف عشان ياخد لون الخلفية اللي وراه
        //    };

        //    // 2. إنشاء النص
        //    var lblEmpty = new Guna.UI2.WinForms.Guna2HtmlLabel
        //    {
        //        Text ="no logs yet", // استخدام HTML للتوسيط الدقيق
        //        AutoSize = false, // نخليه false عشان نقدر نتحكم في الـ Dock
        //        Font = SmartTheme.FontBody,
        //        ForeColor = SmartTheme.TextMuted,
        //        Dock = DockStyle.Fill, // يملأ الـ pnlEmpty بالكامل
        //        TextAlignment = ContentAlignment.MiddleCenter // يوسطن النص داخلياً
        //    };

        //    pnlEmpty.Controls.Add(lblEmpty);
        //    flowItems.Controls.Add(pnlEmpty);

        //    // ضبط الارتفاع
        //    SetHeight(EmptyHeight);
        //}
        private void ShowEmptyState()
        {
            flowItems.Controls.Clear();

            // 1. إنشاء الزر كـ Container للنص
            var btnEmpty = new Guna.UI2.WinForms.Guna2Button
            {
                Text = " >> No logs yet",
                Font = SmartTheme.FontBody,
                ForeColor = SmartTheme.TextMuted,
                FillColor = Color.Transparent, // بدون لون خلفية
                Size = new Size(flowItems.ClientSize.Width - 10, 120), // ارتفاع مناسب

                // التوسيط هنا داخلي وتلقائي 100%
                TextAlign = HorizontalAlignment.Center,

                // عشان ميبانش إنه زرار تفاعلي
                HoverState = { FillColor = Color.Transparent },
                PressedColor = Color.Transparent,
                Animated = false,
                Cursor = Cursors.Default
            };

            // 2. إضافة أيقونة لو حابة (اختياري)
            // btnEmpty.Image = YourProperties.Resources.EmptyIcon;
            // btnEmpty.ImageSize = new Size(30, 30);
            // btnEmpty.ImageAlign = HorizontalAlignment.Center;
            // btnEmpty.TextImageRelation = TextImageRelation.ImageAboveText;

            flowItems.Controls.Add(btnEmpty);

            // ضبط الارتفاع الكلي
            SetHeight(120);
        }
        private void SetHeight(int contentHeight)
        {
            // ارتفاع الـ UC = Header + Content
            int totalHeight = pnlHeader.Height + contentHeight;
            this.Height = totalHeight;
            pnlContent.Height = contentHeight;
           
        }
        //public void ClearAllLogs()
        //{
        //    // مسح الكروت من الـ FlowLayoutPanel
        //    flowItems.Controls.Clear();

        //    // إظهار جملة "No logs yet" في النص (باستخدام الـ Guna2Button أو الـ Label اليدوي)
        //    ShowEmptyState();

        //    // تصغير الارتفاع عشان الـ Console يطلع لفوق
        //    SetHeight(EmptyHeight);
        //}
    }
}