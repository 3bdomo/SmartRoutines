namespace SmartRoutines.UI.Controls
{
    partial class UC_LogsPage
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            btnClearLogs = new Guna.UI2.WinForms.Guna2Button();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlDivider = new Guna.UI2.WinForms.Guna2Panel();
            ucConsole = new UC_LiveConsole();
            pnlLogsArea = new Guna.UI2.WinForms.Guna2Panel();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(btnClearLogs);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.CustomizableEdges = customizableEdges3;
            _transition.SetDecoration(pnlHeader, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlHeader.Size = new Size(2295, 148);
            pnlHeader.TabIndex = 0;
            // 
            // btnClearLogs
            // 
            btnClearLogs.BorderRadius = 10;
            btnClearLogs.CustomizableEdges = customizableEdges1;
            _transition.SetDecoration(btnClearLogs, Guna.UI2.AnimatorNS.DecorationType.None);
            btnClearLogs.DisabledState.BorderColor = Color.DarkGray;
            btnClearLogs.DisabledState.CustomBorderColor = Color.DarkGray;
            btnClearLogs.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnClearLogs.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnClearLogs.Font = new Font("Segoe MDL2 Assets", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnClearLogs.ForeColor = Color.White;
            btnClearLogs.Location = new Point(1902, 25);
            btnClearLogs.Margin = new Padding(4);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnClearLogs.Size = new Size(362, 104);
            btnClearLogs.TabIndex = 1;
            btnClearLogs.Text = "  Clear Logs";
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            _transition.SetDecoration(lblTitle, Guna.UI2.AnimatorNS.DecorationType.None);
            lblTitle.Location = new Point(26, 38);
            lblTitle.Margin = new Padding(4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(236, 43);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "guna2HtmlLabel1";
            // 
            // pnlDivider
            // 
            pnlDivider.CustomizableEdges = customizableEdges5;
            _transition.SetDecoration(pnlDivider, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlDivider.Dock = DockStyle.Top;
            pnlDivider.Location = new Point(0, 148);
            pnlDivider.Name = "pnlDivider";
            pnlDivider.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlDivider.Size = new Size(2295, 1);
            pnlDivider.TabIndex = 0;
            // 
            // ucConsole
            // 
            ucConsole.BackColor = Color.FromArgb(18, 18, 18);
            _transition.SetDecoration(ucConsole, Guna.UI2.AnimatorNS.DecorationType.None);
            ucConsole.Dock = DockStyle.Bottom;
            ucConsole.Font = new Font("Segoe UI", 9F);
            ucConsole.ForeColor = Color.FromArgb(255, 255, 255);
            ucConsole.Location = new Point(0, 756);
            ucConsole.Margin = new Padding(0);
            ucConsole.Name = "ucConsole";
            ucConsole.Size = new Size(2295, 776);
            ucConsole.TabIndex = 3;
            // 
            // pnlLogsArea
            // 
            pnlLogsArea.AutoScroll = true;
            pnlLogsArea.CustomizableEdges = customizableEdges7;
            _transition.SetDecoration(pnlLogsArea, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlLogsArea.Dock = DockStyle.Fill;
            pnlLogsArea.Location = new Point(0, 149);
            pnlLogsArea.Margin = new Padding(0);
            pnlLogsArea.Name = "pnlLogsArea";
            pnlLogsArea.ShadowDecoration.CustomizableEdges = customizableEdges8;
            pnlLogsArea.Size = new Size(2295, 607);
            pnlLogsArea.TabIndex = 4;
            // 
            // UC_LogsPage
            // 
            AutoScaleDimensions = new SizeF(22F, 54F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(pnlLogsArea);
            Controls.Add(ucConsole);
            Controls.Add(pnlDivider);
            Controls.Add(pnlHeader);
            _transition.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
            Margin = new Padding(4);
            Name = "UC_LogsPage";
            Size = new Size(2295, 1532);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2Button btnClearLogs;
        private Guna.UI2.WinForms.Guna2Panel pnlDivider;
        // private UC_LiveConsole uC_LiveConsole1;
        private UC_LiveConsole ucConsole;
        private Guna.UI2.WinForms.Guna2Panel pnlLogsArea;
        //private UC_LiveConsole uC_LiveConsole2;
    }
}
