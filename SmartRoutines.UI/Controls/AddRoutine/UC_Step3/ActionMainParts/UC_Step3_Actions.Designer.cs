using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls.AddRoutine.UC_Step3.ActionMainParts;

partial class UC_Step3_Actions : SmartUserControl
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TableLayoutPanel _root;
    private System.Windows.Forms.Panel _header;
    private System.Windows.Forms.Label _lblTitle;
    private System.Windows.Forms.Label _lblSubtitle;
    private System.Windows.Forms.Panel _contentPanel;
    private System.Windows.Forms.FlowLayoutPanel _flowActions;
    private Guna.UI2.WinForms.Guna2Panel _actionPicker;
    private Guna.UI2.WinForms.Guna2TextBox _txtSearchAction;
    private System.Windows.Forms.FlowLayoutPanel _flowTemplateItems;
    private UC_EmptyState _emptyState;
    private System.Windows.Forms.Panel _footer;
    private Guna.UI2.WinForms.Guna2Button _btnAddAction;

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
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        _root = new System.Windows.Forms.TableLayoutPanel();
        _header = new System.Windows.Forms.Panel();
        _lblSubtitle = new System.Windows.Forms.Label();
        _lblTitle = new System.Windows.Forms.Label();
        _contentPanel = new System.Windows.Forms.Panel();
        _actionPicker = new Guna.UI2.WinForms.Guna2Panel();
        _flowTemplateItems = new System.Windows.Forms.FlowLayoutPanel();
        _txtSearchAction = new Guna.UI2.WinForms.Guna2TextBox();
        _flowActions = new System.Windows.Forms.FlowLayoutPanel();
        _emptyState = new UC_EmptyState();
        _footer = new System.Windows.Forms.Panel();
        _btnAddAction = new Guna.UI2.WinForms.Guna2Button();
        _root.SuspendLayout();
        _header.SuspendLayout();
        _contentPanel.SuspendLayout();
        _actionPicker.SuspendLayout();
        _footer.SuspendLayout();
        SuspendLayout();
        // 
        // _root
        // 
        _root.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _root.ColumnCount = 1;
        _root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _root.Controls.Add(_header, 0, 0);
        _root.Controls.Add(_contentPanel, 0, 1);
        _root.Controls.Add(_footer, 0, 2);
        _transition.SetDecoration(_root, Guna.UI2.AnimatorNS.DecorationType.None);
        _root.Dock = System.Windows.Forms.DockStyle.Fill;
        _root.Location = new System.Drawing.Point(0, 0);
        _root.Name = "_root";
        _root.Padding = new System.Windows.Forms.Padding(24);
        _root.RowCount = 3;
        _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
        _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
        _root.Size = new System.Drawing.Size(900, 520);
        _root.TabIndex = 0;
        // 
        // _header
        // 
        _header.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _header.Controls.Add(_lblSubtitle);
        _header.Controls.Add(_lblTitle);
        _transition.SetDecoration(_header, Guna.UI2.AnimatorNS.DecorationType.None);
        _header.Dock = System.Windows.Forms.DockStyle.Fill;
        _header.Location = new System.Drawing.Point(27, 27);
        _header.Name = "_header";
        _header.Size = new System.Drawing.Size(846, 66);
        _header.TabIndex = 0;
        // 
        // _lblSubtitle
        // 
        _transition.SetDecoration(_lblSubtitle, Guna.UI2.AnimatorNS.DecorationType.None);
        _lblSubtitle.Dock = System.Windows.Forms.DockStyle.Top;
        _lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
        _lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)160)), ((int)((byte)160)), ((int)((byte)160)));
        _lblSubtitle.Location = new System.Drawing.Point(0, 36);
        _lblSubtitle.Name = "_lblSubtitle";
        _lblSubtitle.Size = new System.Drawing.Size(846, 24);
        _lblSubtitle.TabIndex = 1;
        _lblSubtitle.Text = "Build your automation workflow step by step";
        // 
        // _lblTitle
        // 
        _transition.SetDecoration(_lblTitle, Guna.UI2.AnimatorNS.DecorationType.None);
        _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
        _lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
        _lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)255)), ((int)((byte)255)), ((int)((byte)255)));
        _lblTitle.Location = new System.Drawing.Point(0, 0);
        _lblTitle.Name = "_lblTitle";
        _lblTitle.Size = new System.Drawing.Size(846, 36);
        _lblTitle.TabIndex = 0;
        _lblTitle.Text = "Action Pipeline";
        // 
        // _contentPanel
        // 
        _contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _contentPanel.Controls.Add(_actionPicker);
        _contentPanel.Controls.Add(_flowActions);
        _contentPanel.Controls.Add(_emptyState);
        _transition.SetDecoration(_contentPanel, Guna.UI2.AnimatorNS.DecorationType.None);
        _contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        _contentPanel.Location = new System.Drawing.Point(27, 99);
        _contentPanel.Name = "_contentPanel";
        _contentPanel.Size = new System.Drawing.Size(846, 322);
        _contentPanel.TabIndex = 1;
        // 
        // _actionPicker
        // 
        _actionPicker.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)42)), ((int)((byte)42)), ((int)((byte)42)));
        _actionPicker.BorderRadius = 8;
        _actionPicker.BorderThickness = 1;
        _actionPicker.Controls.Add(_flowTemplateItems);
        _actionPicker.Controls.Add(_txtSearchAction);
        _transition.SetDecoration(_actionPicker, Guna.UI2.AnimatorNS.DecorationType.None);
        _actionPicker.CustomizableEdges = customizableEdges3;
        _actionPicker.Dock = System.Windows.Forms.DockStyle.Fill;
        _actionPicker.FillColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _actionPicker.Location = new System.Drawing.Point(0, 0);
        _actionPicker.Name = "_actionPicker";
        _actionPicker.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
        _actionPicker.ShadowDecoration.CustomizableEdges = customizableEdges4;
        _actionPicker.Size = new System.Drawing.Size(846, 322);
        _actionPicker.TabIndex = 2;
        _actionPicker.Visible = false;
        // 
        // _flowTemplateItems
        // 
        _flowTemplateItems.AutoScroll = true;
        _flowTemplateItems.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _transition.SetDecoration(_flowTemplateItems, Guna.UI2.AnimatorNS.DecorationType.None);
        _flowTemplateItems.Dock = System.Windows.Forms.DockStyle.Fill;
        _flowTemplateItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        _flowTemplateItems.Location = new System.Drawing.Point(0, 52);
        _flowTemplateItems.Name = "_flowTemplateItems";
        _flowTemplateItems.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _flowTemplateItems.Size = new System.Drawing.Size(846, 266);
        _flowTemplateItems.TabIndex = 1;
        _flowTemplateItems.WrapContents = false;
        // 
        // _txtSearchAction
        // 
        _txtSearchAction.BackColor = System.Drawing.Color.Transparent;
        _txtSearchAction.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)42)), ((int)((byte)42)), ((int)((byte)42)));
        _txtSearchAction.BorderRadius = 8;
        _transition.SetDecoration(_txtSearchAction, Guna.UI2.AnimatorNS.DecorationType.None);
        _txtSearchAction.Dock = System.Windows.Forms.DockStyle.Top;
        _txtSearchAction.FillColor = System.Drawing.Color.FromArgb(((int)((byte)30)), ((int)((byte)30)), ((int)((byte)30)));
        _txtSearchAction.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)0)), ((int)((byte)120)), ((int)((byte)212)));
        _txtSearchAction.Font = new System.Drawing.Font("Segoe UI", 9F);
        _txtSearchAction.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)255)), ((int)((byte)255)), ((int)((byte)255)));
        _txtSearchAction.Location = new System.Drawing.Point(0, 0);
        _txtSearchAction.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        _txtSearchAction.Name = "_txtSearchAction";
        _txtSearchAction.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)((byte)120)), ((int)((byte)120)), ((int)((byte)120)));
        _txtSearchAction.PlaceholderText = "Search actions...";
        _txtSearchAction.SelectedText = "";
        _txtSearchAction.Size = new System.Drawing.Size(846, 52);
        _txtSearchAction.TabIndex = 0;
        // 
        // _flowActions
        // 
        _flowActions.AutoScroll = true;
        _flowActions.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _transition.SetDecoration(_flowActions, Guna.UI2.AnimatorNS.DecorationType.None);
        _flowActions.Dock = System.Windows.Forms.DockStyle.Fill;
        _flowActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        _flowActions.Location = new System.Drawing.Point(0, 0);
        _flowActions.Name = "_flowActions";
        _flowActions.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
        _flowActions.Size = new System.Drawing.Size(846, 322);
        _flowActions.TabIndex = 0;
        _flowActions.WrapContents = false;
        // 
        // _emptyState
        // 
        _emptyState.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _transition.SetDecoration(_emptyState, Guna.UI2.AnimatorNS.DecorationType.None);
        _emptyState.Dock = System.Windows.Forms.DockStyle.Fill;
        _emptyState.Font = new System.Drawing.Font("Segoe UI", 9F);
        _emptyState.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)255)), ((int)((byte)255)), ((int)((byte)255)));
        _emptyState.Location = new System.Drawing.Point(0, 0);
        _emptyState.Name = "_emptyState";
        _emptyState.Size = new System.Drawing.Size(846, 322);
        _emptyState.Subtitle = "Click the + button below to build your automation";
        _emptyState.TabIndex = 1;
        _emptyState.Title = "No actions yet";
        // 
        // _footer
        // 
        _footer.BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        _footer.Controls.Add(_btnAddAction);
        _transition.SetDecoration(_footer, Guna.UI2.AnimatorNS.DecorationType.None);
        _footer.Dock = System.Windows.Forms.DockStyle.Fill;
        _footer.Location = new System.Drawing.Point(27, 427);
        _footer.Name = "_footer";
        _footer.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
        _footer.Size = new System.Drawing.Size(846, 66);
        _footer.TabIndex = 2;
        // 
        // _btnAddAction
        // 
        _btnAddAction.Animated = true;
        _btnAddAction.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)0)), ((int)((byte)120)), ((int)((byte)212)));
        _btnAddAction.BorderRadius = 8;
        _btnAddAction.BorderThickness = 1;
        _btnAddAction.CustomizableEdges = customizableEdges1;
        _transition.SetDecoration(_btnAddAction, Guna.UI2.AnimatorNS.DecorationType.None);
        _btnAddAction.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
        _btnAddAction.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
        _btnAddAction.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)((byte)169)), ((int)((byte)169)), ((int)((byte)169)));
        _btnAddAction.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)141)), ((int)((byte)141)), ((int)((byte)141)));
        _btnAddAction.Dock = System.Windows.Forms.DockStyle.Fill;
        _btnAddAction.FillColor = System.Drawing.Color.FromArgb(((int)((byte)20)), ((int)((byte)0)), ((int)((byte)120)), ((int)((byte)212)));
        _btnAddAction.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        _btnAddAction.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)0)), ((int)((byte)120)), ((int)((byte)212)));
        _btnAddAction.Location = new System.Drawing.Point(0, 8);
        _btnAddAction.Name = "_btnAddAction";
        _btnAddAction.ShadowDecoration.CustomizableEdges = customizableEdges2;
        _btnAddAction.Size = new System.Drawing.Size(846, 58);
        _btnAddAction.TabIndex = 0;
        _btnAddAction.Text = "+ Add Action";
        _btnAddAction.Click += _btnAddAction_Click;
        // 
        // UC_Step3_Actions
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(((int)((byte)18)), ((int)((byte)18)), ((int)((byte)18)));
        Controls.Add(_root);
        _transition.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
        Size = new System.Drawing.Size(900, 520);
        _root.ResumeLayout(false);
        _header.ResumeLayout(false);
        _contentPanel.ResumeLayout(false);
        _actionPicker.ResumeLayout(false);
        _footer.ResumeLayout(false);
        ResumeLayout(false);
    }
}

