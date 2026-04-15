using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.UI.Core.Theme;

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
            lblTitle.Font = SmartTheme.FontBodyBold;
            lblTitle.ForeColor = Color.White;
            pnlHeader.Padding = new Padding(24, 20, 0, 10);
            
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
                item.Width = flowItems.Width - 25;
                flowItems.Controls.Add(item);
            }
            // ── ضبط الارتفاع حسب عدد الـ Items ──
            int visibleCount = Math.Min(logs.Count, MaxVisible);
            SetHeight(visibleCount * ItemHeight);
        }

        /// <summary>
        /// إضافة لوج جديد في الأعلى (للـ Real-time updates)
        /// </summary>
        public void AddLogToTop(ActivityLog log)
        {
            if (log == null) return;

            // إزالة "No logs yet" إذا كانت موجودة
            if (flowItems.Controls.Count > 0 && flowItems.Controls[0] is Guna.UI2.WinForms.Guna2Button)
            {
                flowItems.Controls.Clear();
            }

            var item = new UC_LogItem(log);
            item.Width = flowItems.Width - 25;
            flowItems.Controls.Add(item);
            // ضع العنصر في المقدمة
            flowItems.Controls.SetChildIndex(item, 0);

            // تحديث الارتفاع
            int visibleCount = Math.Min(flowItems.Controls.Count, MaxVisible);
            SetHeight(visibleCount * ItemHeight);
        }

        /// <summary>
        /// مسح جميع اللوجات والعودة للـ Empty State
        /// </summary>
        public void ClearAllLogs()
        {
            flowItems.Controls.Clear();
            ShowEmptyState();
            SetHeight(EmptyHeight);
        }

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
    }
}