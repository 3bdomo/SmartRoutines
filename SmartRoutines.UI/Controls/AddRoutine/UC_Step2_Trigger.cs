
using System.Drawing;
using System.Windows.Forms;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls.AddRoutine;

public class UC_Step2_Trigger : SmartUserControl
{
	// --- Properties ---
	private TriggerType _selectedTriggerType = TriggerType.Time;
	public TriggerType SelectedTriggerType => _selectedTriggerType;

	// --- Private Fields ---
	private Panel[] _allCards = null!;
	private TableLayoutPanel _cardsGrid = null!;
	private Panel _configContainer = null!;
	private Label _lblHeader = null!;

	// Config panels
	private Panel _timeCfg = null!;
	private Panel _appCfg = null!;
	private Panel _startCfg = null!;
	private Panel _fileCfg = null!;

	// Config controls
	private DateTimePicker _dtpTime = null!;
	private int _selectedDays = 127;
	private TextBox _txtAppName = null!;
	private TextBox _txtFilePath = null!;

	public UC_Step2_Trigger()
	{
		InitializeComponents();
		this.Resize += (s, e) => UpdateLayoutPositions();
	}

	private void InitializeComponents()
	{
		this.BackColor = SmartTheme.Background;
		this.Padding = new Padding(30);

		_configContainer = new Panel
		{
			Size = new Size(this.Width - 60, 180),
			BackColor = SmartTheme.Background,
			BorderStyle = BorderStyle.FixedSingle
		};

		// 2. Cards Grid
		_cardsGrid = new TableLayoutPanel
		{
			ColumnCount = 2,
			RowCount = 2,
			Size = new Size(this.Width - 60, 200)
		};
		_cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
		_cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
		_cardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
		_cardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

		var triggerData = new[] {
			(TriggerType.Time, "🕐", "Time-based", "Run at specific times"),
			(TriggerType.AppLaunched, "💻", "App Launched", "When an app starts"),
			(TriggerType.Startup, "🚀", "System Start", "On Windows startup"),
			(TriggerType.FileChanged, "📄", "File Changed", "When files are modified")
		};

		_allCards = new Panel[4];
		for (int i = 0; i < triggerData.Length; i++)
		{
			var card = CreateTriggerCard(triggerData[i].Item1, triggerData[i].Item2, triggerData[i].Item3, triggerData[i].Item4);
			_allCards[i] = card;
			_cardsGrid.Controls.Add(card, i % 2, i / 2);
		}

		_lblHeader = new Label
		{
			Text = "Choose Trigger Type",
			ForeColor = SmartTheme.TextPrimary,
			Font = new Font("Segoe UI", 10, FontStyle.Bold),
			AutoSize = true
		};

		this.Controls.Add(_configContainer);
		this.Controls.Add(_cardsGrid);
		this.Controls.Add(_lblHeader);

		CreateAllConfigPanels();
		SelectCard(_allCards[0], TriggerType.Time);

		UpdateLayoutPositions();
	}

	private void UpdateLayoutPositions()
	{
		int margin = 30;
		int spacing = 15;
		int availableWidth = Math.Max(this.Width - (margin * 2), 100);

		_configContainer.Width = availableWidth;
		_configContainer.Height = 180; 
		_configContainer.Location = new Point(margin, this.Height - _configContainer.Height - margin);

		_cardsGrid.Width = availableWidth;
		_cardsGrid.Location = new Point(margin, _configContainer.Top - _cardsGrid.Height - spacing);

		_lblHeader.Location = new Point(margin, _cardsGrid.Top - _lblHeader.Height - 5);

		foreach (Control ctrl in _configContainer.Controls)
		{
			ctrl.Size = _configContainer.ClientSize;
		}
	}

	private Panel CreateTriggerCard(TriggerType type, string icon, string title, string subtitle)
	{
		var card = new Panel
		{
			Dock = DockStyle.Fill,
			BackColor = SmartTheme.Background,
			BorderStyle = BorderStyle.FixedSingle,
			Cursor = Cursors.Hand,
			Margin = new Padding(5),
			Padding = new Padding(15),
			Tag = type
		};

		var lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 18), ForeColor = SmartTheme.Primary, AutoSize = true, Location = new Point(15, 10), Cursor = Cursors.Hand };
		var lblT = new Label { Text = title, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = SmartTheme.TextPrimary, AutoSize = true, Location = new Point(15, 42), Cursor = Cursors.Hand };
		var lblS = new Label { Text = subtitle, Font = new Font("Segoe UI", 8), ForeColor = SmartTheme.TextSecondary, AutoSize = true, Location = new Point(15, 62), Cursor = Cursors.Hand };

		card.Controls.AddRange(new Control[] { lblIcon, lblT, lblS });

		void OnClick(object? s, EventArgs e) => SelectCard(card, type);
		card.Click += OnClick; lblIcon.Click += OnClick; lblT.Click += OnClick; lblS.Click += OnClick;

		return card;
	}

	private void SelectCard(Panel selectedCard, TriggerType type)
	{
		_selectedTriggerType = type;
		foreach (var c in _allCards)
		{
			bool isSel = (c == selectedCard);
			c.BackColor = isSel ? SmartTheme.Background : SmartTheme.Background;
			c.BorderStyle = isSel ? BorderStyle.FixedSingle : BorderStyle.FixedSingle;
			// c.Controls[0] is lblIcon
			c.Controls[0].ForeColor = isSel ? SmartTheme.Primary : SmartTheme.TextSecondary;
		}

		_configContainer.Controls.Clear();
		Panel panelToShow = type switch
		{
			TriggerType.Time => _timeCfg,
			TriggerType.AppLaunched => _appCfg,
			TriggerType.Startup => _startCfg,
			TriggerType.FileChanged => _fileCfg,
			_ => _timeCfg
		};

		panelToShow.Size = _configContainer.ClientSize;
		panelToShow.Visible = true;
		_configContainer.Controls.Add(panelToShow);
	}

	private void CreateAllConfigPanels()
	{
		// 1. Time-Based Config
		_timeCfg = new Panel { BackColor = Color.Transparent, Padding = new Padding(20) };
		_timeCfg.Controls.Add(new Label { Text = "Execute at Time", ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(20, 10), AutoSize = true });
		_dtpTime = new DateTimePicker
		{
			Format = DateTimePickerFormat.Custom,
			CustomFormat = "hh:mm tt",
			ShowUpDown = true,
			Width = 150,
			Location = new Point(20, 35),
			BackColor = SmartTheme.Background,
			ForeColor = Color.White
		};
		_timeCfg.Controls.Add(_dtpTime);

		_timeCfg.Controls.Add(new Label { Text = "Days of Week", ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(20, 75), AutoSize = true });
		var flowD = new FlowLayoutPanel { Location = new Point(20, 100), Size = new Size(400, 50) };
		string[] days = { "M", "T", "W", "T", "F", "S", "S" };
		for (int i = 0; i < 7; i++)
		{
			int dIdx = i;
			var btn = new Button { Text = days[i], Size = new Size(35, 35), FlatStyle = FlatStyle.Flat, BackColor = SmartTheme.Primary, ForeColor = Color.White, Cursor = Cursors.Hand };
			btn.FlatAppearance.BorderSize = 0;
			btn.Click += (s, e) => {
				int bit = 1 << dIdx; _selectedDays ^= bit;
				btn.BackColor = (_selectedDays & bit) != 0 ? SmartTheme.Primary : SmartTheme.Surface3;
			};
			flowD.Controls.Add(btn);
		}
		_timeCfg.Controls.Add(flowD);

		_appCfg = new Panel { BackColor = Color.Transparent, Padding = new Padding(20) };
		_appCfg.Controls.Add(new Label { Text = "Application Executable Name (e.g. notepad.exe)", ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true });
		_txtAppName = new TextBox { Width = 300, Location = new Point(20, 50), BackColor = SmartTheme.Background, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
		_appCfg.Controls.Add(_txtAppName);

		// 3. System Start Config
		_startCfg = new Panel { BackColor = Color.Transparent };
		_startCfg.Controls.Add(new Label { Text = "Automation will trigger once Windows starts.", ForeColor = Color.Gray, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

		// 4. File Changed Config
		_fileCfg = new Panel { BackColor = Color.Transparent, Padding = new Padding(20) };
		_fileCfg.Controls.Add(new Label { Text = "Monitor File Path", ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true });
		_txtFilePath = new TextBox { Width = 300, Location = new Point(20, 50), BackColor = SmartTheme.Background, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
		_fileCfg.Controls.Add(_txtFilePath);
	}

	public string GetTriggerConfigJson()
	{
		var config = new SmartRoutines.Core.Domain.Models.TriggerConfiguration 
		{ 
			ScheduledTime = _dtpTime.Value,
			RepeatDays = (SmartRoutines.Core.Domain.Enums.DayOfWeek)_selectedDays,
			SsidName = _selectedTriggerType == TriggerType.AppLaunched ? _txtAppName.Text : _txtFilePath.Text
		};

		return System.Text.Json.JsonSerializer.Serialize(config);
	}

    public void SetData(TriggerType type, string configJson)
    {
        try
        {
            var config = System.Text.Json.JsonSerializer.Deserialize<SmartRoutines.Core.Domain.Models.TriggerConfiguration>(configJson);
            if (config == null) return;

            // Find and select card
            var card = _allCards.FirstOrDefault(c => (TriggerType)c.Tag! == type);
            if (card != null)
            {
                SelectCard(card, type);
            }

            // Populate values
            _dtpTime.Value = config.ScheduledTime;
            _selectedDays = (int)config.RepeatDays;

            // Update days buttons
            var flowD = _timeCfg.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
            if (flowD != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (flowD.Controls[i] is Button btn)
                    {
                        int bit = 1 << i;
                        btn.BackColor = (_selectedDays & bit) != 0 ? SmartTheme.Primary : SmartTheme.Surface3;
                    }
                }
            }

            if (type == TriggerType.AppLaunched)
            {
                _txtAppName.Text = config.SsidName;
            }
            else if (type == TriggerType.FileChanged)
            {
                _txtFilePath.Text = config.SsidName;
            }
        }
        catch { /* ignore invalid json */ }
    }
}