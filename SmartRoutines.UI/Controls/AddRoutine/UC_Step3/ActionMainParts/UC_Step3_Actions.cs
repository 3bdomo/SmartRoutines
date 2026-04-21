using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls.AddRoutine.UC_Step3.ActionMainParts;

public partial class UC_Step3_Actions : SmartUserControl
{
    private const int RowGap = 12;

    private sealed class ActionTemplate
    {
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required ActionBlockInputMode Mode { get; init; }
        public required string Keywords { get; init; }
        public string IconText { get; init; } = "A";
        public string DefaultText { get; init; } = string.Empty;
        public string PlaceholderText { get; init; } = "e.g., value";
        public int SliderMin { get; init; } = 0;
        public int SliderMax { get; init; } = 100;
        public int SliderValue { get; init; } = 50;
    }

    private readonly List<(Panel Wrapper, Guna2VSeparator Connector, UC_ActionBlock Block)> _rows = [];
    private readonly List<ActionTemplate> _templates =
    [
        new ActionTemplate
        {
            Title = "Mute/Unmute Audio",
            Description = "Control system volume",
            Mode = ActionBlockInputMode.Slider,
            Keywords = "mute unmute audio volume sound",
            IconText = "AU",
            PlaceholderText = "Volume level",
            SliderMin = 0,
            SliderMax = 100,
            SliderValue = 30
        },
        new ActionTemplate
        {
            Title = "Launch Application",
            Description = "Start an application",
            Mode = ActionBlockInputMode.TextInput,
            Keywords = "launch start app application open exe",
            IconText = "AP",
            PlaceholderText = "e.g., notepad.exe"
        },
        new ActionTemplate
        {
            Title = "Run Command",
            Description = "Execute a shell command",
            Mode = ActionBlockInputMode.TextInput,
            Keywords = "run command cmd powershell shell script",
            IconText = "CM",
            PlaceholderText = "e.g., shutdown /s /t 0"
        },
        new ActionTemplate
        {
            Title = "Open URL",
            Description = "Open a website in browser",
            Mode = ActionBlockInputMode.TextInput,
            Keywords = "open url website link browser",
            IconText = "WB",
            PlaceholderText = "https://example.com"
        },

        new ActionTemplate
        {
            Title = "Close Application",
            Description = "Close an application",
            Mode = ActionBlockInputMode.TextInput,
            Keywords = "launch start app application open exe",
            IconText = "AP",
            PlaceholderText = "e.g., notepad.exe"
        }
    ];

    private readonly Dictionary<Guna2Panel, ActionTemplate> _templateCards = [];
    private ActionTemplate? _selectedTemplate;
    private bool _pickerVisible;

    public event EventHandler? AddActionRequested;
    public event EventHandler<UC_ActionBlock>? ActionRemoved;
    public event EventHandler<UC_ActionBlock>? ActionChanged;

    public UC_Step3_Actions()
    {
        InitializeComponent();

        _flowActions.Resize += (_, _) => RefreshRowLayout();
        _flowTemplateItems.Resize += (_, _) => RefreshTemplateCardsLayout();
        _txtSearchAction.TextChanged += (_, _) => RenderTemplateCards();
        _emptyState.AddActionClicked += (_, _) => AddActionRequested?.Invoke(this, EventArgs.Empty);

        RenderTemplateCards();
        UpdateState();
    }

    public IReadOnlyList<UC_ActionBlock> Actions
    {
        get
        {
            var actions = new List<UC_ActionBlock>(_rows.Count);
            foreach (var row in _rows)
            {
                actions.Add(row.Block);
            }
            return actions;
        }
    }

    public UC_ActionBlock AddActionBlock(ActionBlockInputMode mode, string title, string iconText = "A", string placeholderText = "e.g., value")
    {
        var block = new UC_ActionBlock
        {
            Mode = mode,
            Title = title,
            ActionIcon = iconText,
            PlaceholderText = placeholderText
        };

        var wrapper = new Panel
        {
            Height = block.Height + RowGap,
            Width = _flowActions.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4,
            BackColor = SmartTheme.Background,
            Margin = new Padding(0)
        };

        var connector = new Guna2VSeparator
        {
            FillColor = SmartTheme.Border,
            Location = new Point(30, 0),
            Size = new Size(2, wrapper.Height)
        };

        block.Location = new Point(38, 0);
        block.Size = new Size(wrapper.Width - 42, block.Height);
        block.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        wrapper.Controls.Add(connector);
        wrapper.Controls.Add(block);

        block.RemoveRequested += (_, _) => RemoveActionBlock(block);
        block.ValueChanged += (_, _) => ActionChanged?.Invoke(this, block);

        _rows.Add((wrapper, connector, block));
        _flowActions.Controls.Add(wrapper);

        RenumberSteps();
        UpdateConnectors();
        RefreshRowLayout();
        UpdateState();

        return block;
    }

    public void RemoveActionBlock(UC_ActionBlock block)
    {
        var index = _rows.FindIndex(x => x.Block == block);
        if (index < 0)
        {
            return;
        }

        var row = _rows[index];
        _flowActions.Controls.Remove(row.Wrapper);
        row.Wrapper.Dispose();
        _rows.RemoveAt(index);

        RenumberSteps();
        UpdateConnectors();
        RefreshRowLayout();
        UpdateState();

        ActionRemoved?.Invoke(this, block);
    }

    public void ClearActions()
    {
        foreach (var row in _rows)
        {
            _flowActions.Controls.Remove(row.Wrapper);
            row.Wrapper.Dispose();
        }

        _rows.Clear();
        RenumberSteps();
        UpdateState();
    }


    private void RenderTemplateCards()
    {
        _flowTemplateItems.SuspendLayout();
        _flowTemplateItems.Controls.Clear();
        _templateCards.Clear();

        var query = (_txtSearchAction.Text ?? string.Empty).Trim();
        var items = string.IsNullOrWhiteSpace(query)
            ? _templates
            : _templates.Where(x =>
                x.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Keywords.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var template in items)
        {
            var card = CreateTemplateCard(template);
            _templateCards[card] = template;
            _flowTemplateItems.Controls.Add(card);
        }

        _flowTemplateItems.ResumeLayout();
        RefreshTemplateCardsLayout();
        ApplySelectionStyles();
    }

    private Guna2Panel CreateTemplateCard(ActionTemplate template)
    {
        var card = new Guna2Panel
        {
            Height = 62,
            Margin = new Padding(0, 0, 0, 8),
            BorderRadius = 8,
            BorderThickness = 1,
            BorderColor = SmartTheme.Border,
            FillColor = SmartTheme.Background,
            Cursor = Cursors.Hand
        };

        var icon = new Guna2CircleButton
        {
            Size = new Size(28, 28),
            Location = new Point(14, 16),
            Text = template.IconText,
            Enabled = false,
            Font = SmartTheme.FontCaption,
            FillColor = Color.FromArgb(26, SmartTheme.Primary),
            ForeColor = SmartTheme.Primary,
            DisabledState = { FillColor = Color.FromArgb(26, SmartTheme.Primary), ForeColor = SmartTheme.Primary }
        };

        var title = new Label
        {
            AutoSize = false,
            Location = new Point(52, 10),
            Size = new Size(520, 22),
            ForeColor = SmartTheme.TextPrimary,
            Font = SmartTheme.FontBodyBold,
            Text = template.Title,
            Cursor = Cursors.Hand
        };

        var subtitle = new Label
        {
            AutoSize = false,
            Location = new Point(52, 32),
            Size = new Size(520, 20),
            ForeColor = SmartTheme.TextSecondary,
            Font = SmartTheme.FontCaption,
            Text = template.Description,
            Cursor = Cursors.Hand
        };

        card.Click += (_, _) => SelectTemplate(card);
        icon.Click += (_, _) => SelectTemplate(card);
        title.Click += (_, _) => SelectTemplate(card);
        subtitle.Click += (_, _) => SelectTemplate(card);

        card.Controls.Add(icon);
        card.Controls.Add(title);
        card.Controls.Add(subtitle);

        return card;
    }

    private void SelectTemplate(Guna2Panel selectedCard)
    {
        if (!_templateCards.TryGetValue(selectedCard, out var template))
        {
            return;
        }

        _selectedTemplate = template;
        ApplySelectionStyles();
        _btnAddAction.Text = "Add Action";
    }

    private void ApplySelectionStyles()
    {
        foreach (var cardEntry in _templateCards)
        {
            var isSelected = ReferenceEquals(cardEntry.Value, _selectedTemplate);
            cardEntry.Key.BorderColor = isSelected ? SmartTheme.Primary : SmartTheme.Border;
            cardEntry.Key.FillColor = isSelected ? Color.FromArgb(20, SmartTheme.Primary) : SmartTheme.Background;
        }
    }

    private void RefreshTemplateCardsLayout()
    {
        var width = Math.Max(320, _flowTemplateItems.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
        foreach (Control control in _flowTemplateItems.Controls)
        {
            control.Width = width;
        }
    }

    private void ShowPicker()
    {
        _pickerVisible = true;
        _selectedTemplate = null;
        _txtSearchAction.Clear();
        _btnAddAction.Text = "Cancel";

        RenderTemplateCards();
        UpdateState();
        _txtSearchAction.Focus();
    }

    private void HidePicker()
    {
        _pickerVisible = false;
        _selectedTemplate = null;
        _btnAddAction.Text = "+ Add Action";
        UpdateState();
    }

    private void AddSelectedTemplateAsBlock()
    {
        if (_selectedTemplate is null)
        {
            return;
        }

        var block = AddActionBlock(
            _selectedTemplate.Mode,
            _selectedTemplate.Title,
            _selectedTemplate.IconText,
            _selectedTemplate.PlaceholderText);

        if (_selectedTemplate.Mode == ActionBlockInputMode.TextInput)
        {
            block.InputText = _selectedTemplate.DefaultText;
        }
        else
        {
            block.SliderMin = _selectedTemplate.SliderMin;
            block.SliderMax = _selectedTemplate.SliderMax;
            block.SliderValue = _selectedTemplate.SliderValue;
        }

        ActionChanged?.Invoke(this, block);
        HidePicker();
    }

    private void RefreshRowLayout()
    {
        var targetWidth = Math.Max(300, _flowActions.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);

        foreach (var row in _rows)
        {
            row.Wrapper.Width = targetWidth;
            row.Connector.Location = new Point(30, 0);
            row.Connector.Height = row.Wrapper.Height;
            row.Block.Width = row.Wrapper.Width - 42;
        }
    }

    private void UpdateState()
    {
        if (_pickerVisible)
        {
            _actionPicker.Visible = true;
            _emptyState.Visible = false;
            _flowActions.Visible = false;
            return;
        }

        _actionPicker.Visible = false;
        var hasActions = _rows.Count > 0;
        _emptyState.Visible = !hasActions;
        _flowActions.Visible = hasActions;
    }

    private void UpdateConnectors()
    {
        for (var i = 0; i < _rows.Count; i++)
        {
            _rows[i].Connector.Visible = i < _rows.Count - 1;
        }
    }

    private void RenumberSteps()
    {
        for (var i = 0; i < _rows.Count; i++)
        {
            _rows[i].Block.StepNumber = i + 1;
        }
    }

    private void _btnAddAction_Click(object sender, EventArgs e)
    {
        if (!_pickerVisible)
        {
            AddActionRequested?.Invoke(this, EventArgs.Empty);
            ShowPicker();
            return;
        }

        if (_selectedTemplate is null)
        {
            HidePicker();
            return;
        }

        AddSelectedTemplateAsBlock();
    }

    public void SetData(IEnumerable<SmartRoutines.Core.DTOs.ActionDto> actions)
    {
        ClearActions();
        foreach (var adto in actions.OrderBy(a => a.ExecutionOrder))
        {
            ActionTemplate? template = null;
            switch (adto.Type)
            {
                case SmartRoutines.Core.Domain.Enums.ActionType.LaunchApp:
                    template = _templates.FirstOrDefault(t => t.Title == "Launch Application");
                    break;
                case SmartRoutines.Core.Domain.Enums.ActionType.OpenUrl:
                    template = _templates.FirstOrDefault(t => t.Title == "Open URL");
                    break;
                case SmartRoutines.Core.Domain.Enums.ActionType.SetVolume:
                    template = _templates.FirstOrDefault(t => t.Title == "Mute/Unmute Audio");
                    break;
                case SmartRoutines.Core.Domain.Enums.ActionType.RunCommand:
                    template = _templates.FirstOrDefault(t => t.Title == "Run Command");
                    break;
                case SmartRoutines.Core.Domain.Enums.ActionType.KillProcess:
                    template = _templates.FirstOrDefault(t => t.Title == "Close Application");
                    break;
            }

            if (template != null)
            {
                var block = AddActionBlock(template.Mode, template.Title, template.IconText, template.PlaceholderText);
                if (template.Mode == ActionBlockInputMode.TextInput)
                {
                    block.InputText = adto.Arguments;
                }
                else if (template.Mode == ActionBlockInputMode.Slider)
                {
                    if (int.TryParse(adto.Arguments, out int val))
                    {
                        block.SliderValue = val;
                    }
                }
            }
        }
    }
}
