namespace SmartRoutines.UI.Controls
{
    partial class UC_RoutineCard
    {
        private System.ComponentModel.IContainer components = null;

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
            this.pnlBase = new Guna.UI2.WinForms.Guna2Panel();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnEdit = new Guna.UI2.WinForms.Guna2Button();
            this.btnDelete = new Guna.UI2.WinForms.Guna2Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.pnlDivider = new System.Windows.Forms.Panel();
            this.lblSchedule = new System.Windows.Forms.Label();
            this.lblActions = new System.Windows.Forms.Label();
            this.lblLastRun = new System.Windows.Forms.Label();
            this.toggleActive = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.lblActive = new System.Windows.Forms.Label();
            this.btnRunNow = new Guna.UI2.WinForms.Guna2Button();
            this.pnlBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBase
            // 
            this.pnlBase.BackColor = System.Drawing.Color.Transparent;
            this.pnlBase.Controls.Add(this.btnRunNow);
            this.pnlBase.Controls.Add(this.lblActive);
            this.pnlBase.Controls.Add(this.toggleActive);
            this.pnlBase.Controls.Add(this.lblLastRun);
            this.pnlBase.Controls.Add(this.lblActions);
            this.pnlBase.Controls.Add(this.lblSchedule);
            this.pnlBase.Controls.Add(this.pnlDivider);
            this.pnlBase.Controls.Add(this.lblDescription);
            this.pnlBase.Controls.Add(this.btnDelete);
            this.pnlBase.Controls.Add(this.btnEdit);
            this.pnlBase.Controls.Add(this.lblName);
            this.pnlBase.Controls.Add(this.pbIcon);
            this.pnlBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBase.Location = new System.Drawing.Point(0, 0);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(340, 275);
            this.pnlBase.TabIndex = 0;
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.pbIcon.Location = new System.Drawing.Point(24, 24);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(42, 42);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true;
            this.lblName.Location = new System.Drawing.Point(82, 24);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(160, 22);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Routine Name";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(250, 20);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(32, 32);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "✏️";
            this.btnEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.btnEdit.Padding = new Padding(3,0,0,0);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(286, 20);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(32, 32);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "🗑";
            this.btnDelete.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.btnDelete.Padding = new Padding(3,0,0,0);
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoEllipsis = true;
            this.lblDescription.Location = new System.Drawing.Point(82, 48);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(234, 38);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Description details here spanning maximum of two lines.";
            // 
            // pnlDivider
            // 
            this.pnlDivider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDivider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.pnlDivider.Location = new System.Drawing.Point(24, 96);
            this.pnlDivider.Name = "pnlDivider";
            this.pnlDivider.Size = new System.Drawing.Size(292, 60);
            this.pnlDivider.TabIndex = 10;
            // 
            // lblSchedule
            // 
            this.lblSchedule.AutoEllipsis = true;
            this.lblSchedule.Location = new System.Drawing.Point(34, 104);
            this.lblSchedule.Name = "lblSchedule";
            this.lblSchedule.Size = new System.Drawing.Size(272, 20);
            this.lblSchedule.TabIndex = 5;
            this.lblSchedule.Text = "⏱️ Every Mon, Tue... at 08:00";
            this.lblSchedule.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // lblActions
            // 
            this.lblActions.AutoEllipsis = true;
            this.lblActions.Location = new System.Drawing.Point(34, 126);
            this.lblActions.Name = "lblActions";
            this.lblActions.Size = new System.Drawing.Size(272, 20);
            this.lblActions.TabIndex = 6;
            this.lblActions.Text = "2 actions configured";
            this.lblActions.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // lblLastRun
            // 
            this.lblLastRun.AutoEllipsis = true;
            this.lblLastRun.Location = new System.Drawing.Point(24, 172);
            this.lblLastRun.Name = "lblLastRun";
            this.lblLastRun.Size = new System.Drawing.Size(292, 20);
            this.lblLastRun.TabIndex = 6;
            this.lblLastRun.Text = "✔️ Last run: 8:06:24 PM";
            // 
            // toggleActive
            // 
            this.toggleActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toggleActive.Checked = true;
            this.toggleActive.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.toggleActive.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.toggleActive.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleActive.CheckedState.InnerColor = System.Drawing.Color.White;
            this.toggleActive.Location = new System.Drawing.Point(24, 218);
            this.toggleActive.Name = "toggleActive";
            this.toggleActive.Size = new System.Drawing.Size(40, 22);
            this.toggleActive.TabIndex = 7;
            this.toggleActive.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.toggleActive.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.toggleActive.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleActive.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.toggleActive.CheckedChanged += new System.EventHandler(this.toggleActive_CheckedChanged);
            // 
            // lblActive
            // 
            this.lblActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblActive.AutoSize = true;
            this.lblActive.Location = new System.Drawing.Point(74, 221);
            this.lblActive.Name = "lblActive";
            this.lblActive.Size = new System.Drawing.Size(40, 15);
            this.lblActive.TabIndex = 8;
            this.lblActive.Text = "Active";
            // 
            // btnRunNow
            // 
            this.btnRunNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunNow.Location = new System.Drawing.Point(196, 212);
            this.btnRunNow.Name = "btnRunNow";
            this.btnRunNow.Size = new System.Drawing.Size(120, 36);
            this.btnRunNow.TabIndex = 9;
            this.btnRunNow.Text = "▶ Run Now";
            this.btnRunNow.BorderRadius = 4;
            // 
            // UC_RoutineCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlBase);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 24, 24);
            this.Name = "UC_RoutineCard";
            this.Size = new System.Drawing.Size(340, 275);
            this.pnlBase.ResumeLayout(false);
            this.pnlBase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlBase;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblName;
        private Guna.UI2.WinForms.Guna2Button btnEdit;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Panel pnlDivider;
        private System.Windows.Forms.Label lblSchedule;
        private System.Windows.Forms.Label lblActions;
        private System.Windows.Forms.Label lblLastRun;
        private Guna.UI2.WinForms.Guna2ToggleSwitch toggleActive;
        private System.Windows.Forms.Label lblActive;
        private Guna.UI2.WinForms.Guna2Button btnRunNow;
    }
}
