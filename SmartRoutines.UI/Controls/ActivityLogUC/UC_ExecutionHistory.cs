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
        private const int ItemHeight = 110; 
        private const int MaxVisible = 3;   
        private const int EmptyHeight = 120;
        public UC_ExecutionHistory()
        {
            InitializeComponent();
            pnlHeader.BackColor = SmartTheme.Background;
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
                item.Width = flowItems.Width - 5;
                flowItems.Controls.Add(item);
            }

            // ── ضبط الارتفاع حسب عدد الـ Items ──
            int visibleCount = Math.Min(logs.Count, MaxVisible);
            SetHeight(visibleCount * ItemHeight);
        }

        private void ShowEmptyState()
        {
            // ── Empty State ──
            var pnlEmpty = new Panel
            {
                Size = new Size(flowItems.Width, EmptyHeight),
                BackColor = SmartTheme.Background
            };

            var lblEmpty = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "No logs yet",
                Font = SmartTheme.FontBody,
                ForeColor = SmartTheme.TextMuted,
                Dock = DockStyle.Fill,
                TextAlignment = ContentAlignment.MiddleCenter
            };

            pnlEmpty.Controls.Add(lblEmpty);
            flowItems.Controls.Add(pnlEmpty);

            // ── ارتفاع Empty State ──
            SetHeight(EmptyHeight);
        }

        private void SetHeight(int contentHeight)
        {
            // ارتفاع الـ UC = Header + Content
            int totalHeight = pnlHeader.Height + contentHeight;
            this.Height = totalHeight;
            this.MinimumSize = new Size(0, totalHeight);
            this.MaximumSize = new Size(9999, totalHeight);
            pnlContent.Height = contentHeight;
        }
    }
    }
