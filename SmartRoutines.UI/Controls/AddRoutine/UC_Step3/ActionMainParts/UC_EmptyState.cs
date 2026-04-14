using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls.AddRoutine.UC_Step3.ActionMainParts;

public partial class UC_EmptyState : SmartUserControl
{
    public event EventHandler? AddActionClicked;

    public UC_EmptyState()
    {
        InitializeComponent();
       // _btnAddAction.Click += (_, _) => AddActionClicked?.Invoke(this, EventArgs.Empty);
    }

    public string Title
    {
        get => _lblTitle.Text;
        set => _lblTitle.Text = value;
    }

    public string Subtitle
    {
        get => _lblSubtitle.Text;
        set => _lblSubtitle.Text = value;
    }
}

