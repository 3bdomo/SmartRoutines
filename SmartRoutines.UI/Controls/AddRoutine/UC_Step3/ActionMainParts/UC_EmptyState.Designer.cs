namespace SmartRoutines.UI.Controls.AddRoutine.UC_Step3.ActionMainParts;

partial class UC_EmptyState
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TableLayoutPanel _layoutBody;
    private System.Windows.Forms.Label _lblIcon;
    private System.Windows.Forms.Label _lblTitle;
    private System.Windows.Forms.Label _lblSubtitle;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _layoutBody = new System.Windows.Forms.TableLayoutPanel();
        _lblIcon = new System.Windows.Forms.Label();
        _lblTitle = new System.Windows.Forms.Label();
        _lblSubtitle = new System.Windows.Forms.Label();
        _layoutBody.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutBody
        // 
        _layoutBody.ColumnCount = 3;
        _layoutBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
        _layoutBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
        _layoutBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
        _layoutBody.Controls.Add(_lblIcon, 1, 1);
        _layoutBody.Controls.Add(_lblTitle, 1, 2);
        _layoutBody.Controls.Add(_lblSubtitle, 1, 3);
        _transition.SetDecoration(_layoutBody, Guna.UI2.AnimatorNS.DecorationType.None);
        _layoutBody.Dock = System.Windows.Forms.DockStyle.Fill;
        _layoutBody.Location = new System.Drawing.Point(0, 0);
        _layoutBody.Name = "_layoutBody";
        _layoutBody.RowCount = 7;
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
        _layoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64F));
        _layoutBody.Size = new System.Drawing.Size(820, 440);
        _layoutBody.TabIndex = 0;
        // 
        // _lblIcon
        // 
        _transition.SetDecoration(_lblIcon, Guna.UI2.AnimatorNS.DecorationType.None);
        _lblIcon.Dock = System.Windows.Forms.DockStyle.Top;
        _lblIcon.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
        _lblIcon.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)45)), ((int)((byte)45)), ((int)((byte)45)));
        _lblIcon.Location = new System.Drawing.Point(167, 41);
        _lblIcon.Name = "_lblIcon";
        _lblIcon.Size = new System.Drawing.Size(486, 90);
        _lblIcon.TabIndex = 0;
        _lblIcon.Text = "O";
        _lblIcon.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
        // 
        // _lblTitle
        // 
        _transition.SetDecoration(_lblTitle, Guna.UI2.AnimatorNS.DecorationType.None);
        _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
        _lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        _lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)255)), ((int)((byte)255)), ((int)((byte)255)));
        _lblTitle.Location = new System.Drawing.Point(167, 131);
        _lblTitle.Name = "_lblTitle";
        _lblTitle.Size = new System.Drawing.Size(486, 36);
        _lblTitle.TabIndex = 1;
        _lblTitle.Text = "No actions yet";
        _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // _lblSubtitle
        // 
        _transition.SetDecoration(_lblSubtitle, Guna.UI2.AnimatorNS.DecorationType.None);
        _lblSubtitle.Dock = System.Windows.Forms.DockStyle.Top;
        _lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
        _lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)160)), ((int)((byte)160)), ((int)((byte)160)));
        _lblSubtitle.Location = new System.Drawing.Point(167, 167);
        _lblSubtitle.Name = "_lblSubtitle";
        _lblSubtitle.Size = new System.Drawing.Size(486, 28);
        _lblSubtitle.TabIndex = 2;
        _lblSubtitle.Text = "Click the + button below to build your automation";
        _lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // UC_EmptyState
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        Controls.Add(_layoutBody);
        _transition.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
        Size = new System.Drawing.Size(820, 440);
        _layoutBody.ResumeLayout(false);
        ResumeLayout(false);
    }
}

