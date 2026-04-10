using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls
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
            pipelineActions = new SmartRoutines.UI.Controls.Step3.UC_Step3_Actions();
            SuspendLayout();
            // 
            // pipelineActions
            // 
            pipelineActions.BackColor = SmartTheme.Background;//System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
            pipelineActions.Dock = System.Windows.Forms.DockStyle.Fill;
            pipelineActions.Font = new System.Drawing.Font("Segoe UI", 9F);
            pipelineActions.ForeColor =SmartTheme.Primary ;// System.Drawing.Color.FromArgb(((int)((byte)255)), ((int)((byte)255)), ((int)((byte)255)));
            pipelineActions.Location = new System.Drawing.Point(0, 0);
            pipelineActions.Name = "pipelineActions";
            pipelineActions.Size = new System.Drawing.Size(808, 426);
            pipelineActions.TabIndex = 0;
            // 
            // UC_AciotnsMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pipelineActions);
            Size = new System.Drawing.Size(808, 426);
            ResumeLayout(false);
        }

        #endregion

        private SmartRoutines.UI.Controls.Step3.UC_Step3_Actions pipelineActions;
    }
}
