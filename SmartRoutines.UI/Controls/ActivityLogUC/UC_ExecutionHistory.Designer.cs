namespace SmartRoutines.UI.Controls
{
    partial class UC_ExecutionHistory
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
            pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            flowItems = new FlowLayoutPanel();
            pnlContent = new Guna.UI2.WinForms.Guna2Panel();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.CustomizableEdges = customizableEdges1;
            _transition.SetDecoration(pnlHeader, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(2);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlHeader.Size = new Size(2295, 183);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            _transition.SetDecoration(lblTitle, Guna.UI2.AnimatorNS.DecorationType.None);
            lblTitle.Location = new Point(21, 18);
            lblTitle.Margin = new Padding(4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(236, 43);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "guna2HtmlLabel1";
            // 
            // flowItems
            // 
            flowItems.AutoScroll = true;
            _transition.SetDecoration(flowItems, Guna.UI2.AnimatorNS.DecorationType.None);
            flowItems.Dock = DockStyle.Fill;
            flowItems.FlowDirection = FlowDirection.TopDown;
            flowItems.Location = new Point(0, 0);
            flowItems.MinimumSize = new Size(2295, 286);
            flowItems.Name = "flowItems";
            flowItems.Size = new Size(2295, 991);
            flowItems.TabIndex = 0;
            flowItems.WrapContents = false;
            // 
            // pnlContent
            // 
            pnlContent.AutoScroll = true;
            pnlContent.Controls.Add(flowItems);
            pnlContent.CustomizableEdges = customizableEdges3;
            _transition.SetDecoration(pnlContent, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 183);
            pnlContent.Margin = new Padding(2);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlContent.Size = new Size(2295, 991);
            pnlContent.TabIndex = 1;
            // 
            // UC_ExecutionHistory
            // 
            AutoScaleDimensions = new SizeF(22F, 54F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            _transition.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
            Margin = new Padding(4);
            Name = "UC_ExecutionHistory";
            Size = new Size(2295, 1174);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlContent;
        private FlowLayoutPanel flowItems;
    }
}
