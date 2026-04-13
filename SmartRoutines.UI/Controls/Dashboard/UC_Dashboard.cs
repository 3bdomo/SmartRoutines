using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
{
    public partial class UC_Dashboard : SmartUserControl
    {
        private FlowLayoutPanel newFlpStats;
        private FlowLayoutPanel newFlpRoutines;
        private TableLayoutPanel gridPagination;
        private Guna.UI2.WinForms.Guna2Button btnPrev;
        private Guna.UI2.WinForms.Guna2Button btnNext;
        private Label lblPageInfo;

        private System.Collections.Generic.List<UC_StatCard> rawStatCards;
        private System.Collections.Generic.List<UC_RoutineCard> routineList;

        private int _currentPage = 1;
        private const int _pageSize = 3;

        public UC_Dashboard()
        {
            InitializeComponent();
            ApplyTheme();
            this.Load += UC_Dashboard_Load;
            
            this.Dock = DockStyle.Fill;
            DoubleBuffered = true;
            SetDoubleBuffered(this);
        }

        public static void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, new object[] { true });
            foreach (Control child in control.Controls) SetDoubleBuffered(child);
        }

        public void AddNewRoutine()
        {
            if (newFlpRoutines == null) return;
            
            var r = new UC_RoutineCard();
            r.RoutineName = "Custom Routine";
            r.Description = "Live instantiated automation sequence";
            r.ScheduleText = "Manually triggered";
            r.ActionCountText = "0 actions configured";
            r.LastRunText = "Never run";
            r.IsActive = true;
            r.IsRunning = false;
            r.Anchor = AnchorStyles.None;
            r.Margin = new Padding(0, 0, 24, 24);
            
            routineList.Add(r);
            r.StateChanged += (s, ev) => RefreshStats(rawStatCards, routineList);
            
            RenderPage();
            RefreshStats(rawStatCards, routineList);
        }

        private void UC_Dashboard_Load(object sender, EventArgs e)
        {
            try 
            {
                this.SuspendLayout();

                this.Dock = DockStyle.Fill;
                this.AutoScroll = false; // Container handles it natively without WinForms scroll jumps

                // 1. rootLayout: A strictly controlled vertical TableLayoutPanel 
                TableLayoutPanel rootLayout = new TableLayoutPanel();
                rootLayout.Dock = DockStyle.Fill; 
                rootLayout.AutoSize = false;
                rootLayout.RowCount = 5; // Header area, Stats, Title, Routines, Pagination
                rootLayout.ColumnCount = 1;
                rootLayout.Margin = new Padding(0);
                rootLayout.Padding = new Padding(0);
                
                // STRICT IMPLEMENTATION REQUIREMENTS for Hierarchy
                rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row0: Header area (AutoSize)
                rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F)); // Row1: Stats row (ABSOLUTE 160px height) -> Immune to collapse
                rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row2: "Your Routines" title
                rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Row3: Routines area stretches to fill
                rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row4: Pagination
                rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                
                // 2. statsGrid: Guaranteed 4-column row replacing FlowLayoutPanel
                TableLayoutPanel statsGrid = new TableLayoutPanel();
                statsGrid.Dock = DockStyle.Fill;
                statsGrid.RowCount = 1;
                statsGrid.ColumnCount = 4;
                statsGrid.Margin = new Padding(0);
                statsGrid.Padding = new Padding(16, 10, 16, 0); // Side padding matching design
                
                // Ensure exactly 25% horizontal spread for each card
                for(int j=0; j<4; j++) {
                    statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                }

                // Inject 4 UC_StatCards strictly into the 4 columns
                rawStatCards = new System.Collections.Generic.List<UC_StatCard>();
                Color[] accents = { System.Drawing.Color.FromArgb(59, 130, 246), System.Drawing.Color.FromArgb(34, 197, 94), System.Drawing.Color.FromArgb(168, 85, 247), System.Drawing.Color.FromArgb(245, 158, 11) };
                Color[] bgs = { System.Drawing.Color.FromArgb(15, 23, 42), System.Drawing.Color.FromArgb(15, 28, 20), System.Drawing.Color.FromArgb(24, 18, 43), System.Drawing.Color.FromArgb(30, 20, 10) };
                string[] titles = { "Total Routines", "Active Routines", "Running Now", "Total Actions" };

                for (int i = 0; i < 4; i++)
                {
                    var st = new UC_StatCard();
                    st.CardColor = bgs[i];
                    st.AccentColor = accents[i];
                    st.Title = titles[i];
                    st.Value = "0";
                    st.Dock = DockStyle.Fill; // Perfectly utilizes the 25% column block!
                    st.Margin = new Padding(10, 10, 10, 10);
                    
                    rawStatCards.Add(st);
                    statsGrid.Controls.Add(st, i, 0);
                }

                // 3. Routines Container
                newFlpRoutines = new FlowLayoutPanel();
                newFlpRoutines.Dock = DockStyle.Fill;
                newFlpRoutines.WrapContents = true;
                newFlpRoutines.AutoScroll = false; 
                newFlpRoutines.AutoSize = false;
                newFlpRoutines.Padding = new Padding(26, 0, 26, 0);
                newFlpRoutines.Margin = new Padding(0);

                SetDoubleBuffered(rootLayout);
                SetDoubleBuffered(statsGrid);
                SetDoubleBuffered(newFlpRoutines);

                InitializePaginationControls();

                // 4. Generate Data
                routineList = new System.Collections.Generic.List<UC_RoutineCard>();

                var r1 = new UC_RoutineCard();
                r1.RoutineName = "Morning Setup";
                r1.Description = "Launches email, calendar, and sets volume";
                r1.ScheduleText = "Every Mon, Tue, Wed, Thu, Fri at 08:00";
                r1.ActionCountText = "2 actions configured";
                r1.LastRunText = "✔️ Last run: 11:34:59 PM";
                r1.IsActive = true;
                r1.IsRunning = false;
                routineList.Add(r1);

                var r2 = new UC_RoutineCard();
                r2.RoutineName = "Focus Mode";
                r2.Description = "Silences notifications and closes distracting apps";
                r2.ScheduleText = "When code.exe launches";
                r2.ActionCountText = "2 actions configured";
                r2.LastRunText = "✔️ Last run: 12:34:59 AM";
                r2.IsActive = true;
                r2.IsRunning = true; 
                routineList.Add(r2);

                var r3 = new UC_RoutineCard();
                r3.RoutineName = "Evening Shutdown";
                r3.Description = "Closes all work apps and backs up files";
                r3.ScheduleText = "Every Mon, Tue, Wed, Thu, Fri at 18:00";
                r3.ActionCountText = "1 action configured";
                r3.LastRunText = "❌ Last run: 12:34:59 AM";
                r3.IsActive = false;
                r3.IsRunning = false;
                routineList.Add(r3);

                var r4 = new UC_RoutineCard(); 
                r4.RoutineName = "System Scan";
                r4.Description = "Background maintenance operation";
                r4.ScheduleText = "Every Sunday";
                r4.ActionCountText = "3 actions configured";
                r4.LastRunText = "Never";
                r4.IsActive = false;
                r4.IsRunning = false;
                routineList.Add(r4);

                foreach(var c in routineList)
                {
                    c.Anchor = AnchorStyles.None;
                    c.Margin = new Padding(0, 0, 24, 24);
                    c.StateChanged += (senderBox, evArgs) => RefreshStats(rawStatCards, routineList);
                }

                // 5. Build Hierarchy
                flpStatCards.Visible = false;
                flpRoutineCards.Visible = false;
                if (pnlTopHeader != null) pnlTopHeader.Visible = false;

                lblYourRoutines.Dock = DockStyle.Fill;
                lblYourRoutines.Margin = new Padding(16, 20, 0, 10); // Standardize header padding

                // Ensure nothing overlaps by using explicit Row indexing
                rootLayout.Controls.Add(new Panel { Height = 0, Dock = DockStyle.Fill }, 0, 0); // Row 0
                rootLayout.Controls.Add(statsGrid, 0, 1); // Row 1: Absolute 160px height
                rootLayout.Controls.Add(lblYourRoutines, 0, 2); // Row 2
                rootLayout.Controls.Add(newFlpRoutines, 0, 3); // Row 3
                rootLayout.Controls.Add(gridPagination, 0, 4); // Row 4
                
                this.Controls.Add(rootLayout);
                rootLayout.BringToFront();

                newFlpRoutines.SizeChanged += (s, ev) => AdjustCardWidths(newFlpRoutines, 320, 24, 3);
                
                this.SizeChanged += (s, ev) => 
                {
                    AdjustCardWidths(newFlpRoutines, 320, 24, 3);
                };

                this.ResumeLayout(true);
                
                RenderPage(); 
                RefreshStats(rawStatCards, routineList);

                AdjustCardWidths(newFlpRoutines, 320, 24, 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CRITICAL ERROR in Dashboard Load: " + ex.Message);
            }
        }

        private void InitializePaginationControls()
        {
            gridPagination = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                RowCount = 1,
                ColumnCount = 3,
                Padding = new Padding(16, 8, 16, 24),
                BackColor = Color.Transparent
            };
            
            gridPagination.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            gridPagination.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            gridPagination.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            btnPrev = new Guna.UI2.WinForms.Guna2Button 
            { 
                Text = "Previous", 
                Size = new Size(100, 35), 
                BorderRadius = 6, 
                FillColor = SmartTheme.Surface2,
                ForeColor = SmartTheme.TextPrimary,
                Anchor = AnchorStyles.Right 
            };
            btnPrev.Click += (s,e) => { _currentPage--; RenderPage(); };

            lblPageInfo = new Label
            {
                Text = "Page 1 / 1",
                ForeColor = SmartTheme.TextSecondary,
                Font = SmartTheme.FontBody,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnNext = new Guna.UI2.WinForms.Guna2Button 
            { 
                Text = "Next", 
                Size = new Size(100, 35), 
                BorderRadius = 6, 
                FillColor = SmartTheme.Surface2,
                ForeColor = SmartTheme.TextPrimary,
                Anchor = AnchorStyles.Left 
            };
            btnNext.Click += (s,e) => { _currentPage++; RenderPage(); };

            gridPagination.Controls.Add(btnPrev, 0, 0);
            gridPagination.Controls.Add(lblPageInfo, 1, 0);
            gridPagination.Controls.Add(btnNext, 2, 0);
        }

        private void RenderPage()
        {
            if (routineList == null || newFlpRoutines == null) return;
            newFlpRoutines.SuspendLayout();
            newFlpRoutines.Controls.Clear();

            int totalItems = routineList.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)_pageSize);
            if (totalPages == 0) totalPages = 1;
            
            if (_currentPage < 1) _currentPage = 1;
            if (_currentPage > totalPages) _currentPage = totalPages;

            int startIndex = (_currentPage - 1) * _pageSize;
            
            var paginatedItems = routineList.Skip(startIndex).Take(_pageSize).ToList();
            
            foreach (var card in paginatedItems)
            {
                newFlpRoutines.Controls.Add(card);
            }

            lblPageInfo.Text = $"Page {_currentPage} / {totalPages}";
            btnPrev.Enabled = (_currentPage > 1);
            btnNext.Enabled = (_currentPage < totalPages);

            newFlpRoutines.ResumeLayout(true);
            
            AdjustCardWidths(newFlpRoutines, 320, 24, 3);
        }

        private void RefreshStats(System.Collections.Generic.List<UC_StatCard> stCards, System.Collections.Generic.List<UC_RoutineCard> rList)
        {
            try
            {
                if (stCards.Count >= 4)
                {
                    stCards[0].Value = rList.Count.ToString();
                    stCards[1].Value = rList.Count(x => x.IsActive).ToString();
                    stCards[2].Value = rList.Count(x => x.IsRunning).ToString();
                    stCards[3].Value = "5"; 
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Refresh error: " + ex.Message);
            }
        }

        private void AdjustCardWidths(FlowLayoutPanel flp, int minWidth, int marginSpace, int absoluteCols)
        {
            if (flp.Controls.Count == 0) return;
            flp.SuspendLayout();
            
            int outerPadding = flp.Padding.Left + flp.Padding.Right;
            int scrollbarMargin = 5; 
            
            int availableWidth = this.ClientSize.Width - outerPadding - scrollbarMargin; 
            if (availableWidth <= 0) availableWidth = flp.Width;

            int splitCols = absoluteCols; 
            int targetWidth = (availableWidth - (splitCols * marginSpace)) / splitCols;
            
            if (targetWidth < minWidth) 
            {
                targetWidth = minWidth;
                splitCols = availableWidth / (minWidth + marginSpace);
                if (splitCols <= 0) splitCols = 1;
            }

            foreach (Control card in flp.Controls)
            {
                if (card.Width != targetWidth) card.Width = targetWidth;
            }
            
            // NOTE: Height is strictly governed by the root TableLayoutPanel (100% Fill Row). 
            // Asserting artificial limits triggers an infinite layout looping lag cascade!
            
            flp.ResumeLayout(true);
        }

        private void ApplyTheme()
        {
            this.BackColor = SmartTheme.Background;
            lblYourRoutines.Font = SmartTheme.FontHeader;
            lblYourRoutines.ForeColor = SmartTheme.TextPrimary;
        }
    }
}
