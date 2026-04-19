using System.Drawing;

namespace SmartRoutines.UI.Controls
{
    partial class UC_LogItem
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            RoutenNameLab = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblDetails = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTime = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlIcon = new Guna.UI2.WinForms.Guna2Panel();
            picStatus = new PictureBox();
            pnlSummary = new Guna.UI2.WinForms.Guna2Panel();
            lblSummaryText = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picStatus).BeginInit();
            pnlSummary.SuspendLayout();
            SuspendLayout();
            // 
            // RoutenNameLab
            // 
            RoutenNameLab.BackColor = Color.Transparent;
            _transition.SetDecoration(RoutenNameLab, Guna.UI2.AnimatorNS.DecorationType.None);
            RoutenNameLab.Location = new Point(300, 33);
            RoutenNameLab.Margin = new Padding(4);
            RoutenNameLab.Name = "RoutenNameLab";
            RoutenNameLab.Size = new Size(236, 43);
            RoutenNameLab.TabIndex = 1;
            RoutenNameLab.Text = "guna2HtmlLabel1";
            RoutenNameLab.Click += RoutenNameLab_Click;
            // 
            // lblDetails
            // 
            lblDetails.BackColor = Color.Transparent;
            _transition.SetDecoration(lblDetails, Guna.UI2.AnimatorNS.DecorationType.None);
            lblDetails.Location = new Point(300, 118);
            lblDetails.Margin = new Padding(4);
            lblDetails.Name = "lblDetails";
            lblDetails.Size = new Size(236, 43);
            lblDetails.TabIndex = 2;
            lblDetails.Text = "guna2HtmlLabel1";
            lblDetails.Click += lblDetails_Click;
            // 
            // lblTime
            // 
            lblTime.BackColor = Color.Transparent;
            _transition.SetDecoration(lblTime, Guna.UI2.AnimatorNS.DecorationType.None);
            lblTime.Location = new Point(2000, 40);
            lblTime.Margin = new Padding(4);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(236, 43);
            lblTime.TabIndex = 3;
            lblTime.Text = "guna2HtmlLabel1";
            lblTime.TextAlignment = ContentAlignment.MiddleRight;
            // 
            // pnlIcon
            // 
            // Increased the icon container for a slightly larger icon
            pnlIcon.BorderRadius = 26;
            pnlIcon.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pnlIcon.Controls.Add(picStatus);
            pnlIcon.CustomizableEdges = customizableEdges1;
            _transition.SetDecoration(pnlIcon, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlIcon.Location = new Point(14, 17);
            pnlIcon.Margin = new Padding(4);
            pnlIcon.Name = "pnlIcon";
            pnlIcon.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlIcon.Size = new Size(72, 72);
            pnlIcon.TabIndex = 4;
            // 
            // picStatus
            // 
            _transition.SetDecoration(picStatus, Guna.UI2.AnimatorNS.DecorationType.None);
            // Centered and enlarged status icon
            picStatus.Location = new Point(20, 20);
            picStatus.Margin = new Padding(4);
            picStatus.Name = "picStatus";
            picStatus.Size = new Size(32, 32);
            picStatus.TabIndex = 0;
            picStatus.TabStop = false;
            // 
            // pnlSummary
            // 
            pnlSummary.Controls.Add(lblSummaryText);
            pnlSummary.CustomizableEdges = customizableEdges3;
            _transition.SetDecoration(pnlSummary, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlSummary.Location = new Point(467, 195);
            pnlSummary.Margin = new Padding(4);
            pnlSummary.Name = "pnlSummary";
            pnlSummary.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlSummary.Size = new Size(776, 66);
            pnlSummary.TabIndex = 5;
            // 
            // lblSummaryText
            // 
            lblSummaryText.BackColor = Color.Transparent;
            _transition.SetDecoration(lblSummaryText, Guna.UI2.AnimatorNS.DecorationType.None);
            lblSummaryText.Location = new Point(14, 9);
            lblSummaryText.Margin = new Padding(4);
            lblSummaryText.Name = "lblSummaryText";
            lblSummaryText.Size = new Size(236, 43);
            lblSummaryText.TabIndex = 0;
            lblSummaryText.Text = "guna2HtmlLabel1";
            // 
            // UC_LogItem
            // 
            AutoScaleDimensions = new SizeF(22F, 54F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlSummary);
            Controls.Add(pnlIcon);
            Controls.Add(lblTime);
            Controls.Add(lblDetails);
            Controls.Add(RoutenNameLab);
            _transition.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
            Margin = new Padding(0);
            Name = "UC_LogItem";
            Size = new Size(2350, 286);
            pnlIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picStatus).EndInit();
            pnlSummary.ResumeLayout(false);
            pnlSummary.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Guna.UI2.WinForms.Guna2HtmlLabel RoutenNameLab;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDetails;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTime;
        private Guna.UI2.WinForms.Guna2Panel pnlIcon;
        private PictureBox picStatus;
        private Guna.UI2.WinForms.Guna2Panel pnlSummary;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSummaryText;
    }
}
