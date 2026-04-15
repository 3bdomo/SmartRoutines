using SmartRoutines.UI.Controls.AddRoutine.UC_Step3.ActionMainParts;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls.AddRoutine.UC_Step3
{
    partial class UC_ActionsMain
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
            this.pnlStepper = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this._step3 = new UC_Step3_Actions();
            this.SuspendLayout();
            
            // 
            // pnlStepper
            // 
            this.pnlStepper.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStepper.Height = 84;
            this.pnlStepper.Name = "pnlStepper";
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Controls.Add(this._step3);
            // 
            // pnlFooter
            // 
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height = 84;
            this.pnlFooter.Name = "pnlFooter";
            
            // 
            // UC_ActionsMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlStepper);
            this.Controls.Add(this.pnlFooter);
            this.Name = "UC_ActionsMain";
            this.Size = new Size(950, 680);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlStepper;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlFooter;
        private UC_Step3_Actions _step3;
    }
}
