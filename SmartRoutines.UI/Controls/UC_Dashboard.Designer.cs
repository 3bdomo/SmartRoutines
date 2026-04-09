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
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTopHeader = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flpStatCards = new System.Windows.Forms.FlowLayoutPanel();
            this.statCard1 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.statCard2 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.statCard3 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.statCard4 = new SmartRoutines.UI.Controls.UC_StatCard();
            this.lblYourRoutines = new System.Windows.Forms.Label();
            this.flpRoutineCards = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlTopHeader.SuspendLayout();
            this.flpStatCards.SuspendLayout();
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
            // flpStatCards
            // 
            this.flpStatCards.Controls.Add(this.statCard1);
            this.flpStatCards.Controls.Add(this.statCard2);
            this.flpStatCards.Controls.Add(this.statCard3);
            this.flpStatCards.Controls.Add(this.statCard4);
            this.flpStatCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpStatCards.Location = new System.Drawing.Point(0, 80);
            this.flpStatCards.Name = "flpStatCards";
            this.flpStatCards.Padding = new System.Windows.Forms.Padding(26, 10, 26, 10);
            this.flpStatCards.Size = new System.Drawing.Size(1000, 140);
            this.flpStatCards.TabIndex = 1;
            this.flpStatCards.WrapContents = false;
            // 
            // statCard1
            // 
            this.statCard1.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.statCard1.CardColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(23)))), ((int)(((byte)(38)))));
            this.statCard1.Location = new System.Drawing.Point(26, 10);
            this.statCard1.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.statCard1.Name = "statCard1";
            this.statCard1.Size = new System.Drawing.Size(200, 110);
            this.statCard1.TabIndex = 0;
            this.statCard1.Title = "Total Routines";
            this.statCard1.Value = "3";
            // 
            // statCard2
            // 
            this.statCard2.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.statCard2.CardColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(22)))));
            this.statCard2.Location = new System.Drawing.Point(246, 10);
            this.statCard2.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.statCard2.Name = "statCard2";
            this.statCard2.Size = new System.Drawing.Size(200, 110);
            this.statCard2.TabIndex = 1;
            this.statCard2.Title = "Active Routines";
            this.statCard2.Value = "2";
            // 
            // statCard3
            // 
            this.statCard3.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.statCard3.CardColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(15)))), ((int)(((byte)(36)))));
            this.statCard3.Location = new System.Drawing.Point(466, 10);
            this.statCard3.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.statCard3.Name = "statCard3";
            this.statCard3.Size = new System.Drawing.Size(200, 110);
            this.statCard3.TabIndex = 2;
            this.statCard3.Title = "Running Now";
            this.statCard3.Value = "1";
            // 
            // statCard4
            // 
            this.statCard4.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.statCard4.CardColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(21)))), ((int)(((byte)(11)))));
            this.statCard4.Location = new System.Drawing.Point(686, 10);
            this.statCard4.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.statCard4.Name = "statCard4";
            this.statCard4.Size = new System.Drawing.Size(200, 110);
            this.statCard4.TabIndex = 3;
            this.statCard4.Title = "Total Actions";
            this.statCard4.Value = "5";
            // 
            // lblYourRoutines
            // 
            this.lblYourRoutines.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblYourRoutines.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblYourRoutines.ForeColor = System.Drawing.Color.White;
            this.lblYourRoutines.Location = new System.Drawing.Point(0, 220);
            this.lblYourRoutines.Name = "lblYourRoutines";
            this.lblYourRoutines.Padding = new System.Windows.Forms.Padding(26, 20, 0, 10);
            this.lblYourRoutines.Size = new System.Drawing.Size(1000, 60);
            this.lblYourRoutines.TabIndex = 2;
            this.lblYourRoutines.Text = "✨ Your Routines";
            this.lblYourRoutines.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flpRoutineCards
            // 
            this.flpRoutineCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpRoutineCards.Location = new System.Drawing.Point(0, 280);
            this.flpRoutineCards.Name = "flpRoutineCards";
            this.flpRoutineCards.Padding = new System.Windows.Forms.Padding(26, 0, 26, 26);
            this.flpRoutineCards.Size = new System.Drawing.Size(1000, 320);
            this.flpRoutineCards.TabIndex = 3;
            // 
            // UC_Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.flpRoutineCards);
            this.Controls.Add(this.lblYourRoutines);
            this.Controls.Add(this.flpStatCards);
            this.Controls.Add(this.pnlTopHeader);
            this.Name = "UC_Dashboard";
            this.Size = new System.Drawing.Size(1000, 600);
            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.flpStatCards.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopHeader;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flpStatCards;
        private System.Windows.Forms.Label lblYourRoutines;
        private System.Windows.Forms.FlowLayoutPanel flpRoutineCards;
        private SmartRoutines.UI.Controls.UC_StatCard statCard1;
        private SmartRoutines.UI.Controls.UC_StatCard statCard2;
        private SmartRoutines.UI.Controls.UC_StatCard statCard3;
        private SmartRoutines.UI.Controls.UC_StatCard statCard4;
    }
}
