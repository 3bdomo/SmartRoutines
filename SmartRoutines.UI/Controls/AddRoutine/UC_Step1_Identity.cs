

using SmartRoutines.UI.Core.Theme;

namespace Project_2.UI.Steps;

public class UC_Step1_Identity : SmartUserControl
{
	private TextBox _txtRoutineName = null!;
	private Label _lblNameError = null!;
	private TextBox _txtDescription = null!;
	private FlowLayoutPanel _flowIcons = null!;
	private string _selectedIconKey = "sunrise";
	private Panel[] _iconCards = null!;

	private readonly Dictionary<string, (string Unicode, string Name)> _icons = new()
	{
		{ "sunrise", ("☀", "Sunrise") },
		{ "target", ("◎", "Target") },
		{ "moon", ("☾", "Moon") },
		{ "zap", ("⚡", "Zap") },
		{ "clock", ("🕐", "Clock") },
		{ "rocket", ("🚀", "Rocket") }
	};

	public string RoutineName => _txtRoutineName.Text == "e.g., Morning Setup" ? "" : _txtRoutineName.Text;
	public string Description => _txtDescription.Text == "Describe what this routine does..." ? "" : _txtDescription.Text;
	public string SelectedIconKey => _selectedIconKey;

	public UC_Step1_Identity()
	{
		InitializeComponents();
	}

	private void InitializeComponents()
	{
		this.BackColor = SmartTheme.Background;
		this.Padding = new Padding(30, 140, 30, 20);

		TableLayoutPanel mainLayout = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 1,
			RowCount = 7,
			BackColor = Color.Transparent
		};

		mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Label Name
		mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // 1: TextBox Name
		mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));  // 2: Error Label
		mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 3: Label Description
		mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // 4: TextBox Description
		mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 5: Label Choose Icon
		mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // 6: Icons Area

		// 1. Routine Name
		var lblName = CreateHeaderLabel("Routine Name *");
		_txtRoutineName = CreateStyledTextBox("e.g., Morning Setup");

		// 2. Error Label
		_lblNameError = new Label
		{
			Text = "Routine name is required",
			ForeColor = Color.FromArgb(231, 76, 60),
			Font = new Font("Segoe UI", 8),
			Visible = false,
			AutoSize = true
		};

		// 3. Description
		var lblDesc = CreateHeaderLabel("Description");
		lblDesc.Margin = new Padding(0, 10, 0, 5); 
		_txtDescription = CreateStyledTextBox("Describe what this routine does...", true);

		// 4. Icons
		var lblIcon = CreateHeaderLabel("Choose an Icon");
		lblIcon.Margin = new Padding(0, 15, 0, 10);

		_flowIcons = new FlowLayoutPanel
		{
			Dock = DockStyle.Fill,
			FlowDirection = FlowDirection.LeftToRight,
			WrapContents = false,
			BackColor = Color.Transparent
		};

		mainLayout.Controls.Add(lblName, 0, 0);
		mainLayout.Controls.Add(_txtRoutineName, 0, 1);
		mainLayout.Controls.Add(_lblNameError, 0, 2);
		mainLayout.Controls.Add(lblDesc, 0, 3);
		mainLayout.Controls.Add(_txtDescription, 0, 4);
		mainLayout.Controls.Add(lblIcon, 0, 5);
		mainLayout.Controls.Add(_flowIcons, 0, 6);

		this.Controls.Add(mainLayout);

		var iconsList = _icons.ToList();
		_iconCards = new Panel[iconsList.Count];
		for (int i = 0; i < iconsList.Count; i++)
		{
			var card = CreateIconCard(iconsList[i].Key, iconsList[i].Value.Unicode, iconsList[i].Value.Name);
			_iconCards[i] = card;
			_flowIcons.Controls.Add(card);
		}

		SelectIcon("sunrise");
	}

	private Label CreateHeaderLabel(string text) => new Label
	{
		Text = text,
		ForeColor = SmartTheme.TextPrimary,
		Font = new Font("Segoe UI", 9, FontStyle.Bold),
		AutoSize = true,
		Margin = new Padding(0, 0, 0, 5)
	};

	private TextBox CreateStyledTextBox(string placeholder, bool multiline = false)
	{
		var tb = new TextBox
		{
			Dock = DockStyle.Fill,
			BackColor = SmartTheme.Background,
			ForeColor = SmartTheme.TextSecondary,
			BorderStyle = BorderStyle.FixedSingle,
			Font = new Font("Segoe UI", 10),
			Text = placeholder,
			Multiline = multiline
		};

		tb.GotFocus += (s, e) => {
			if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = SmartTheme.TextPrimary; }
		};
		tb.LostFocus += (s, e) => {
			if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = SmartTheme.TextSecondary; }
		};
		return tb;
	}

	private Panel CreateIconCard(string key, string unicode, string name)
	{
		var card = new Panel
		{
			Size = new Size(95, 75),
			BackColor = SmartTheme.Background,
			BorderStyle = BorderStyle.FixedSingle,
			Margin = new Padding(0, 0, 8, 0),
			Cursor = Cursors.Hand,
			Tag = key
		};

		var lblImg = new Label
		{
			Text = unicode,
			Font = new Font("Segoe UI Symbol", 18),
			TextAlign = ContentAlignment.MiddleCenter,
			Dock = DockStyle.Fill,
			Tag = key
		};
		var lblName = new Label
		{
			Text = name,
			Font = new Font("Segoe UI", 7),
			TextAlign = ContentAlignment.BottomCenter,
			Dock = DockStyle.Bottom,
			Height = 22,
			Tag = key
		};

		card.Controls.Add(lblImg);
		card.Controls.Add(lblName);

		void OnClick(object? s, EventArgs e) => SelectIcon(key);
		card.Click += OnClick; lblImg.Click += OnClick; lblName.Click += OnClick;

		return card;
	}

	private void SelectIcon(string key)
	{
		_selectedIconKey = key;
		foreach (var card in _iconCards)
		{
			bool isSelected = (string)card.Tag! == key;
			card.BackColor = isSelected ? SmartTheme.Background : SmartTheme.Background;
			card.BorderStyle = isSelected ? BorderStyle.FixedSingle : BorderStyle.FixedSingle;
			foreach (Control c in card.Controls)
				c.ForeColor = isSelected ? SmartTheme.Primary : SmartTheme.TextSecondary;
		}
	}
}