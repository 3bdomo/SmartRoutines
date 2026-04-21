namespace SmartRoutines.UI.Controls
{
    partial class UC_LiveConsole
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            richConsole = new RichTextBox();
            pnlConsoleHeader = new Guna.UI2.WinForms.Guna2Panel();
            pnlGreen = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            pnlYellow = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            pnlRed = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            lblConsoleTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtCommand = new Guna.UI2.WinForms.Guna2TextBox();
            pnlConsoleHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlGreen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlYellow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlRed).BeginInit();
            SuspendLayout();
            // 
            // richConsole
            // 
            _transition.SetDecoration(richConsole, Guna.UI2.AnimatorNS.DecorationType.None);
            richConsole.Dock = DockStyle.Fill;
            richConsole.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richConsole.Location = new Point(0, 140);
            richConsole.Margin = new Padding(4);
            richConsole.Name = "richConsole";
            richConsole.Size = new Size(2295, 442);
            richConsole.TabIndex = 0;
            richConsole.Text = "";
            richConsole.TextChanged += richConsole_TextChanged;
            // 
            // pnlConsoleHeader
            // 
            pnlConsoleHeader.Controls.Add(pnlGreen);
            pnlConsoleHeader.Controls.Add(pnlYellow);
            pnlConsoleHeader.Controls.Add(pnlRed);
            pnlConsoleHeader.Controls.Add(lblConsoleTitle);
            pnlConsoleHeader.CustomizableEdges = customizableEdges4;
            _transition.SetDecoration(pnlConsoleHeader, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlConsoleHeader.Dock = DockStyle.Top;
            pnlConsoleHeader.Location = new Point(0, 0);
            pnlConsoleHeader.Margin = new Padding(4);
            pnlConsoleHeader.Name = "pnlConsoleHeader";
            pnlConsoleHeader.ShadowDecoration.CustomizableEdges = customizableEdges5;
            pnlConsoleHeader.Size = new Size(2295, 140);
            pnlConsoleHeader.TabIndex = 1;
            // 
            // pnlGreen
            // 
            _transition.SetDecoration(pnlGreen, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlGreen.ImageRotate = 0F;
            pnlGreen.Location = new Point(2143, 42);
            pnlGreen.Margin = new Padding(4);
            pnlGreen.Name = "pnlGreen";
            pnlGreen.ShadowDecoration.CustomizableEdges = customizableEdges1;
            pnlGreen.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            pnlGreen.Size = new Size(50, 50);
            pnlGreen.TabIndex = 8;
            pnlGreen.TabStop = false;
            // 
            // pnlYellow
            // 
            _transition.SetDecoration(pnlYellow, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlYellow.ImageRotate = 0F;
            pnlYellow.Location = new Point(2214, 42);
            pnlYellow.Margin = new Padding(4);
            pnlYellow.Name = "pnlYellow";
            pnlYellow.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlYellow.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            pnlYellow.Size = new Size(50, 50);
            pnlYellow.TabIndex = 7;
            pnlYellow.TabStop = false;
            // 
            // pnlRed
            // 
            _transition.SetDecoration(pnlRed, Guna.UI2.AnimatorNS.DecorationType.None);
            pnlRed.ImageRotate = 0F;
            pnlRed.Location = new Point(2067, 42);
            pnlRed.Margin = new Padding(4);
            pnlRed.Name = "pnlRed";
            pnlRed.ShadowDecoration.CustomizableEdges = customizableEdges3;
            pnlRed.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            pnlRed.Size = new Size(50, 50);
            pnlRed.TabIndex = 6;
            pnlRed.TabStop = false;
            pnlRed.Click += pnlRed_Click;
            // 
            // lblConsoleTitle
            // 
            lblConsoleTitle.BackColor = Color.Transparent;
            _transition.SetDecoration(lblConsoleTitle, Guna.UI2.AnimatorNS.DecorationType.None);
            lblConsoleTitle.Location = new Point(25, 42);
            lblConsoleTitle.Margin = new Padding(4);
            lblConsoleTitle.Name = "lblConsoleTitle";
            lblConsoleTitle.Size = new Size(236, 43);
            lblConsoleTitle.TabIndex = 2;
            lblConsoleTitle.Text = "guna2HtmlLabel1";
            // 
            // txtCommand
            // 
            txtCommand.CustomizableEdges = customizableEdges6;
            _transition.SetDecoration(txtCommand, Guna.UI2.AnimatorNS.DecorationType.None);
            txtCommand.DefaultText = "";
            txtCommand.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtCommand.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtCommand.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtCommand.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtCommand.Dock = DockStyle.Bottom;
            txtCommand.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtCommand.Font = new Font("Segoe UI", 9F);
            txtCommand.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtCommand.Location = new Point(0, 582);
            txtCommand.Margin = new Padding(9, 11, 9, 11);
            txtCommand.Name = "txtCommand";
            txtCommand.PlaceholderText = "";
            txtCommand.SelectedText = "";
            txtCommand.ShadowDecoration.CustomizableEdges = customizableEdges7;
            txtCommand.Size = new Size(2295, 50);
            txtCommand.TabIndex = 6;
            // 
            // UC_LiveConsole
            // 
            AutoScaleDimensions = new SizeF(22F, 54F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(richConsole);
            Controls.Add(pnlConsoleHeader);
            Controls.Add(txtCommand);
            _transition.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
            Margin = new Padding(0);
            Name = "UC_LiveConsole";
            Size = new Size(2295, 632);
            Load += UC_LiveConsole_Load;
            pnlConsoleHeader.ResumeLayout(false);
            pnlConsoleHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pnlGreen).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlYellow).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlRed).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richConsole;
        private Guna.UI2.WinForms.Guna2Panel pnlConsoleHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblConsoleTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtCommand;
        private Guna.UI2.WinForms.Guna2CirclePictureBox pnlGreen;
        private Guna.UI2.WinForms.Guna2CirclePictureBox pnlYellow;
        private Guna.UI2.WinForms.Guna2CirclePictureBox pnlRed;
    }
}
