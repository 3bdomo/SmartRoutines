namespace SmartRoutines.UI.Controls
{
    partial class UC_Dashboard
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.pnlTopHeader = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            // Replace FlowLayoutPanel with TableLayoutPanel for deterministic 4-column stat grid
            this.tlpStatCards = new System.Windows.Forms.TableLayoutPanel();
            this.statCard1 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.statCard2 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.statCard3 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.statCard4 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.lblYourRoutines = new System.Windows.Forms.Label();
            this.flpRoutineCards = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlTopHeader.SuspendLayout();
            this.tlpStatCards.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopHeader
            // 
            this.pnlTopHeader.Controls.Add(this.lblSubtitle);
            this.pnlTopHeader.Controls.Add(this.lblTitle);
            this.pnlTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlTopHeader.Name = "pnlTopHeader";
            this.pnlTopHeader.Size = new System.Drawing.Size(1000, 80);
            this.pnlTopHeader.TabIndex = 0;
            this.pnlTopHeader.Visible = false; // Header now lives in FrmMain's BuildContentHeader
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSubtitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubtitle.Location = new System.Drawing.Point(30, 45);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(217, 19);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Manage your automation routines";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(26, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(138, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard";
            // 
            // tlpStatCards — 4 equal columns, fixed height stat grid
            // 
            this.tlpStatCards.ColumnCount = 4;
            this.tlpStatCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStatCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStatCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStatCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStatCards.Controls.Add(this.statCard1, 0, 0);
            this.tlpStatCards.Controls.Add(this.statCard2, 1, 0);
            this.tlpStatCards.Controls.Add(this.statCard3, 2, 0);
            this.tlpStatCards.Controls.Add(this.statCard4, 3, 0);
            this.tlpStatCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpStatCards.Location = new System.Drawing.Point(0, 0);
            this.tlpStatCards.Name = "tlpStatCards";
            this.tlpStatCards.Padding = new System.Windows.Forms.Padding(20, 14, 20, 14);
            this.tlpStatCards.RowCount = 1;
            this.tlpStatCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpStatCards.Size = new System.Drawing.Size(1000, 148);
            this.tlpStatCards.TabIndex = 1;
            this.tlpStatCards.BackColor = System.Drawing.Color.Transparent;
            // 
            // statCard1
            // 
            this.statCard1.AccentColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.statCard1.CardColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.statCard1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statCard1.Margin = new System.Windows.Forms.Padding(6);
            this.statCard1.Name = "statCard1";
            this.statCard1.TabIndex = 0;
            this.statCard1.Title = "Total Routines";
            this.statCard1.Value = "0";
            // 
            // statCard2
            // 
            this.statCard2.AccentColor = System.Drawing.Color.FromArgb(34, 197, 94);
            this.statCard2.CardColor = System.Drawing.Color.FromArgb(15, 28, 20);
            this.statCard2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statCard2.Margin = new System.Windows.Forms.Padding(6);
            this.statCard2.Name = "statCard2";
            this.statCard2.TabIndex = 1;
            this.statCard2.Title = "Active Routines";
            this.statCard2.Value = "0";
            // 
            // statCard3
            // 
            this.statCard3.AccentColor = System.Drawing.Color.FromArgb(139, 92, 246);
            this.statCard3.CardColor = System.Drawing.Color.FromArgb(24, 18, 43);
            this.statCard3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statCard3.Margin = new System.Windows.Forms.Padding(6);
            this.statCard3.Name = "statCard3";
            this.statCard3.TabIndex = 2;
            this.statCard3.Title = "Running Now";
            this.statCard3.Value = "0";
            // 
            // statCard4
            // 
            this.statCard4.AccentColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.statCard4.CardColor = System.Drawing.Color.FromArgb(30, 20, 10);
            this.statCard4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statCard4.Margin = new System.Windows.Forms.Padding(6);
            this.statCard4.Name = "statCard4";
            this.statCard4.TabIndex = 3;
            this.statCard4.Title = "Total Actions";
            this.statCard4.Value = "0";
            // 
            // lblYourRoutines
            // 
            this.lblYourRoutines.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblYourRoutines.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblYourRoutines.ForeColor = System.Drawing.Color.White;
            this.lblYourRoutines.Location = new System.Drawing.Point(0, 148);
            this.lblYourRoutines.Name = "lblYourRoutines";
            this.lblYourRoutines.Padding = new System.Windows.Forms.Padding(26, 10, 0, 8);
            this.lblYourRoutines.Size = new System.Drawing.Size(1000, 52);
            this.lblYourRoutines.TabIndex = 2;
            this.lblYourRoutines.Text = "✨ Your Routines";
            this.lblYourRoutines.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flpRoutineCards — Dock.Fill, always last to fill remaining space
            // 
            this.flpRoutineCards.AutoScroll = true;
            this.flpRoutineCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpRoutineCards.Location = new System.Drawing.Point(0, 200);
            this.flpRoutineCards.Name = "flpRoutineCards";
            this.flpRoutineCards.Padding = new System.Windows.Forms.Padding(20, 4, 20, 20);
            this.flpRoutineCards.Size = new System.Drawing.Size(1000, 400);
            this.flpRoutineCards.TabIndex = 3;
            this.flpRoutineCards.WrapContents = true;
            // 
            // UC_Dashboard
            // Z-Order rule: Controls added LAST dock on visual TOP.
            // Add in reverse: flpRoutineCards (Fill) → lblYourRoutines (Top) → tlpStatCards (Top)
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 20);
            this.Controls.Add(this.flpRoutineCards);     // Fill — added first → renders in remaining space
            this.Controls.Add(this.lblYourRoutines);     // Top  — added second → docks above Fill
            this.Controls.Add(this.tlpStatCards);        // Top  — added third  → docks above label
            this.Name = "UC_Dashboard";
            this.Size = new System.Drawing.Size(1000, 600);
            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.tlpStatCards.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTopHeader;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tlpStatCards;
        private System.Windows.Forms.Label lblYourRoutines;
        private System.Windows.Forms.FlowLayoutPanel flpRoutineCards;
        private SmartRoutines.UI.Controls.UC_StatCard statCard1;
        private SmartRoutines.UI.Controls.UC_StatCard statCard2;
        private SmartRoutines.UI.Controls.UC_StatCard statCard3;
        private SmartRoutines.UI.Controls.UC_StatCard statCard4;
    }
}
