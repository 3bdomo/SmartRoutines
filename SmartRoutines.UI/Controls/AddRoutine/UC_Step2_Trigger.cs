
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
	private Panel _wifiCfg = null!;
	private Panel _batteryCfg = null!;
	private Panel _idleCfg = null!;
	private Panel _shutdownCfg = null!;

	// Config controls
	private DateTimePicker _dtpTime = null!;
	private int _selectedDays = 127;
	private TextBox _txtAppName = null!;
	private TextBox _txtFilePath = null!;
	private TextBox _txtSsid = null!;
	private NumericUpDown _numBattery = null!;
	private NumericUpDown _numIdle = null!;

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

		// 2. Cards Grid (4x2 for 8 triggers)
		_cardsGrid = new TableLayoutPanel
		{
			ColumnCount = 4,
			RowCount = 2,
			Size = new Size(this.Width - 60, 200)
		};
		for(int i=0; i<4; i++) _cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
		for(int i=0; i<2; i++) _cardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

		var triggerData = new[] {
			(TriggerType.Time, "🕐", "Time", "Scheduled run"),
			(TriggerType.AppLaunched, "💻", "App Start", "App launches"),
			(TriggerType.Startup, "🚀", "Startup", "System Boot"),
			(TriggerType.Shutdown, "🛑", "Shutdown", "System Close"),
			(TriggerType.FileChanged, "📄", "File", "File modified"),
			(TriggerType.WiFi, "📶", "WiFi", "SSID connected"),
			(TriggerType.Battery, "🔋", "Battery", "Level threshold"),
			(TriggerType.Idle, "💤", "Idle", "System inactivity")
		};

		_allCards = new Panel[triggerData.Length];
		for (int i = 0; i < triggerData.Length; i++)
		{
			var card = CreateTriggerCard(triggerData[i].Item1, triggerData[i].Item2, triggerData[i].Item3, triggerData[i].Item4);
			_allCards[i] = card;
			_cardsGrid.Controls.Add(card, i % 4, i / 4);
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
			Margin = new Padding(3),
			Padding = new Padding(10),
			Tag = type
		};

		var lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 16), ForeColor = SmartTheme.Primary, AutoSize = true, Location = new Point(10, 5), Cursor = Cursors.Hand };
		var lblT = new Label { Text = title, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = SmartTheme.TextPrimary, AutoSize = true, Location = new Point(10, 35), Cursor = Cursors.Hand };
		var lblS = new Label { Text = subtitle, Font = new Font("Segoe UI", 7), ForeColor = SmartTheme.TextSecondary, AutoSize = true, Location = new Point(10, 50), Cursor = Cursors.Hand };

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
			c.Controls[0].ForeColor = isSel ? SmartTheme.Primary : SmartTheme.TextSecondary;
			c.BorderStyle = isSel ? BorderStyle.FixedSingle : BorderStyle.FixedSingle;
		}

		_configContainer.Controls.Clear();
		Panel panelToShow = type switch
		{
			TriggerType.Time => _timeCfg,
			TriggerType.AppLaunched => _appCfg,
			TriggerType.Startup => _startCfg,
			TriggerType.Shutdown => _shutdownCfg,
			TriggerType.FileChanged => _fileCfg,
			TriggerType.WiFi => _wifiCfg,
			TriggerType.Battery => _batteryCfg,
			TriggerType.Idle => _idleCfg,
			_ => _timeCfg
		};

		panelToShow.Size = _configContainer.ClientSize;
		panelToShow.Visible = true;
		_configContainer.Controls.Add(panelToShow);
	}

	private void CreateAllConfigPanels()
	{
		// 1. Time
		_timeCfg = CreateConfigPanel("Execute at Time");
		_dtpTime = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "hh:mm tt", ShowUpDown = true, Width = 150, Location = new Point(20, 35), BackColor = SmartTheme.Background, ForeColor = Color.White };
		_timeCfg.Controls.Add(_dtpTime);
		_timeCfg.Controls.Add(new Label { Text = "Days of Week", ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(20, 75), AutoSize = true });
		var flowD = new FlowLayoutPanel { Location = new Point(20, 100), Size = new Size(400, 50) };
		string[] days = { "S", "M", "T", "W", "T", "F", "S" };
		for (int i = 0; i < 7; i++)
		{
			int dayBit = 1 << i;
			var btn = new Button { Text = days[i], Size = new Size(35, 35), FlatStyle = FlatStyle.Flat, BackColor = (_selectedDays & dayBit) != 0 ? SmartTheme.Primary : SmartTheme.Surface3, ForeColor = Color.White, Cursor = Cursors.Hand };
			btn.FlatAppearance.BorderSize = 0;
			btn.Click += (s, e) => { _selectedDays ^= dayBit; btn.BackColor = (_selectedDays & dayBit) != 0 ? SmartTheme.Primary : SmartTheme.Surface3; };
			flowD.Controls.Add(btn);
		}
		_timeCfg.Controls.Add(flowD);

		// 2. App
		_appCfg = CreateConfigPanel("Application Executable Name (e.g. notepad.exe)");
		_txtAppName = new TextBox { Width = 300, Location = new Point(20, 45), BackColor = SmartTheme.Background, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
		_appCfg.Controls.Add(_txtAppName);

		// 3. Startup / Shutdown
		_startCfg = CreateConfigPanel("Trigger once Windows starts.");
		_shutdownCfg = CreateConfigPanel("Trigger when Windows begins shutting down.");

		// 4. File
		_fileCfg = CreateConfigPanel("Monitor File Path");
		_txtFilePath = new TextBox { Width = 300, Location = new Point(20, 45), BackColor = SmartTheme.Background, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
		_fileCfg.Controls.Add(_txtFilePath);

		// 5. WiFi
		_wifiCfg = CreateConfigPanel("Connect to WiFi SSID");
		_txtSsid = new TextBox { Width = 300, Location = new Point(20, 45), BackColor = SmartTheme.Background, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
		_wifiCfg.Controls.Add(_txtSsid);

		// 6. Battery
		_batteryCfg = CreateConfigPanel("Battery Level Drops Below (%)");
		_numBattery = new NumericUpDown { Value = 20, Width = 80, Location = new Point(20, 45), BackColor = SmartTheme.Background, ForeColor = Color.White };
		_batteryCfg.Controls.Add(_numBattery);

		// 7. Idle
		_idleCfg = CreateConfigPanel("System Idle for Minutes");
		_numIdle = new NumericUpDown { Value = 5, Width = 80, Location = new Point(20, 45), BackColor = SmartTheme.Background, ForeColor = Color.White };
		_idleCfg.Controls.Add(_numIdle);
	}

	private Panel CreateConfigPanel(string description)
	{
		var p = new Panel { BackColor = Color.Transparent, Padding = new Padding(20) };
		p.Controls.Add(new Label { Text = description, ForeColor = SmartTheme.TextPrimary, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true });
		return p;
	}

	public string GetTriggerConfigJson()
	{
		var config = new SmartRoutines.Core.Domain.Models.TriggerConfiguration();

		switch (_selectedTriggerType)
		{
			case TriggerType.Time:
				config.ScheduledTime = _dtpTime.Value;
				config.RepeatDays = (SmartRoutines.Core.Domain.Enums.DayOfWeek)_selectedDays;
				break;
			case TriggerType.AppLaunched:
				config.AppName = _txtAppName.Text;
				break;
			case TriggerType.FileChanged:
				config.FilePath = _txtFilePath.Text;
				break;
			case TriggerType.WiFi:
				config.SsidName = _txtSsid.Text;
				break;
			case TriggerType.Battery:
				config.BatteryThreshold = (int)_numBattery.Value;
				break;
			case TriggerType.Idle:
				config.IdleMinutes = (int)_numIdle.Value;
				break;
		}

		return System.Text.Json.JsonSerializer.Serialize(config);
	}

    public void SetData(TriggerType type, string configJson)
    {
        try
        {
            var config = System.Text.Json.JsonSerializer.Deserialize<SmartRoutines.Core.Domain.Models.TriggerConfiguration>(configJson);
            if (config == null) return;

            var card = _allCards.FirstOrDefault(c => (TriggerType)c.Tag! == type);
            if (card != null) SelectCard(card, type);

			if (config.ScheduledTime.HasValue) _dtpTime.Value = config.ScheduledTime.Value;
			if (config.RepeatDays.HasValue) 
			{
				_selectedDays = (int)config.RepeatDays.Value;
				// Update day buttons visibility/color
				var flowD = _timeCfg.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
				if (flowD != null) {
					for(int i=0; i<7; i++) {
						if(flowD.Controls[i] is Button btn) {
							int bit = 1 << i;
							btn.BackColor = (_selectedDays & bit) != 0 ? SmartTheme.Primary : SmartTheme.Surface3;
						}
					}
				}
			}
			if (config.AppName != null) _txtAppName.Text = config.AppName;
			if (config.FilePath != null) _txtFilePath.Text = config.FilePath;
			if (config.SsidName != null) _txtSsid.Text = config.SsidName;
			if (config.BatteryThreshold.HasValue) _numBattery.Value = config.BatteryThreshold.Value;
			if (config.IdleMinutes.HasValue) _numIdle.Value = config.IdleMinutes.Value;
        }
        catch { }
    }
}