using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Controls.Step3;

public enum ActionBlockInputMode
{
    TextInput,
    Slider
}

public class UC_ActionBlock : SmartUserControl
{
    private readonly Guna2Panel _card;
    private readonly Label _lblGrip;
    private readonly Guna2Button _badgeStep;
    private readonly Label _lblIcon;
    private readonly Label _lblTitle;
    private readonly Label _lblValue;
    private readonly Guna2Button _btnRemove;
    private readonly Guna2TextBox _txtValue;
    private readonly Guna2TrackBar _slider;
    private readonly Guna2Button _btnMute;
    private readonly Guna2Button _btnUnmute;

    private ActionBlockInputMode _mode;

    public event EventHandler? RemoveRequested;
    public event EventHandler? ValueChanged;

    private bool _isMuted;
    private int _lastNonZeroVolume = 30;

    public UC_ActionBlock()
    {
        Height = 132;
        MinimumSize = new Size(520, 132);
        BackColor = Color.Transparent;
        Margin = new Padding(0, 0, 0, 12);

        _card = new Guna2Panel
        {
            Dock = DockStyle.Fill,
            FillColor = SmartTheme.Surface,
            BorderColor = SmartTheme.Border,
            BorderThickness = 1,
            BorderRadius = SmartTheme.RadiusSmall,
            Padding = new Padding(12)
        };

        _lblGrip = new Label
        {
            Text = ":\n:",
            ForeColor = SmartTheme.TextMuted,
            Font = SmartTheme.FontBodyBold,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(14, 30),
            Location = new Point(8, 14)
        };

        _badgeStep = new Guna2Button
        {
            Text = "1",
            Size = new Size(44, 44),
            Location = new Point(24, 10),
            BorderThickness = 1,
            BorderRadius = 8,
            BorderColor = SmartTheme.Primary,
            FillColor = Color.FromArgb(28, SmartTheme.Primary),
            ForeColor = SmartTheme.Primary,
            Font = SmartTheme.FontBodyBold,
            Enabled = false,
            DisabledState =
            {
                BorderColor = SmartTheme.Primary,
                FillColor = Color.FromArgb(28, SmartTheme.Primary),
                ForeColor = SmartTheme.Primary
            }
        };

        _lblIcon = new Label
        {
            Text = "AP",
            ForeColor = SmartTheme.Primary,
            Font = SmartTheme.FontCaption,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(28, 20),
            Location = new Point(78, 14)
        };

        _lblTitle = new Label
        {
            Text = "Action",
            ForeColor = SmartTheme.TextPrimary,
            Font = SmartTheme.FontBodyBold,
            Location = new Point(108, 13),
            Size = new Size(260, 22)
        };

        _btnRemove = new Guna2Button
        {
            Text = "x",
            Size = new Size(32, 28),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Cursor = Cursors.Hand,
            BorderRadius = 8,
            FillColor = Color.FromArgb(100, SmartTheme.Danger),
            ForeColor = SmartTheme.Danger,
            Font = SmartTheme.FontCaption
        };
        _btnRemove.HoverState.FillColor = Color.FromArgb(150, SmartTheme.Danger);
        _btnRemove.Click += (_, _) => RemoveRequested?.Invoke(this, EventArgs.Empty);

        _txtValue = new Guna2TextBox
        {
            PlaceholderText = "e.g., value",
            Location = new Point(108, 48),
            Size = new Size(300, 34),
            FillColor = SmartTheme.Surface3,
            ForeColor = SmartTheme.TextPrimary,
            BorderColor = SmartTheme.Border,
            FocusedState = { BorderColor = SmartTheme.BorderFocus },
            BorderRadius = 8,
            PlaceholderForeColor = SmartTheme.TextMuted,
            Font = SmartTheme.FontBody
        };
        _txtValue.TextChanged += (_, _) =>
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        };

        _slider = new Guna2TrackBar
        {
            Location = new Point(102, 78),
            Size = new Size(248, 24),
            Minimum = 0,
            Maximum = 100,
            FillColor = Color.FromArgb(52, 60, 70),
            ThumbColor = SmartTheme.Primary,
            Value = _lastNonZeroVolume,
            Cursor = Cursors.Hand
        };

        _lblValue = new Label
        {
            Text = "Volume Level: 30%",
            Font = SmartTheme.FontBody,
            ForeColor = SmartTheme.TextSecondary,
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(108, 48),
            Size = new Size(170, 22)
        };

        _btnMute = new Guna2Button
        {
            Text = "Mute",
            Size = new Size(84, 28),
            Cursor = Cursors.Hand,
            BorderRadius = 14,
            FillColor = Color.FromArgb(130, 42, 42),
            ForeColor = SmartTheme.TextPrimary,
            Font = SmartTheme.FontCaption
        };

        _btnUnmute = new Guna2Button
        {
            Text = "Unmute",
            Size = new Size(84, 28),
            Cursor = Cursors.Hand,
            BorderRadius = 14,
            FillColor = Color.FromArgb(32, 110, 70),
            ForeColor = SmartTheme.TextPrimary,
            Font = SmartTheme.FontCaption
        };

        _slider.ValueChanged += (_, _) =>
        {
            if (_slider.Value > 0)
            {
                _lastNonZeroVolume = _slider.Value;
                _isMuted = false;
            }
            else
            {
                _isMuted = true;
            }

            _lblValue.Text = $"Volume Level: {_slider.Value}%";
            UpdateMuteVisualState();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        };

        _btnMute.Click += (_, _) =>
        {
            if (_slider.Value > 0)
            {
                _lastNonZeroVolume = _slider.Value;
            }

            _slider.Value = 0;
            _isMuted = true;
            UpdateMuteVisualState();
        };

        _btnUnmute.Click += (_, _) =>
        {
            var restore = _lastNonZeroVolume <= 0 ? 30 : _lastNonZeroVolume;
            _slider.Value = Math.Max(_slider.Minimum, Math.Min(_slider.Maximum, restore));
            _isMuted = false;
            UpdateMuteVisualState();
        };

        _card.Controls.Add(_lblGrip);
        _card.Controls.Add(_badgeStep);
        _card.Controls.Add(_lblIcon);
        _card.Controls.Add(_lblTitle);
        _card.Controls.Add(_btnRemove);
        _card.Controls.Add(_txtValue);
        _card.Controls.Add(_slider);
        _card.Controls.Add(_lblValue);
        _card.Controls.Add(_btnMute);
        _card.Controls.Add(_btnUnmute);

        Controls.Add(_card);
        Resize += (_, _) => RefreshLayout();

        Mode = ActionBlockInputMode.TextInput;
        UpdateMuteVisualState();
        RefreshLayout();
    }

    [Category("Action Block")]
    public string Title
    {
        get => _lblTitle.Text;
        set => _lblTitle.Text = value;
    }

    [Category("Action Block")]
    public int StepNumber
    {
        get => int.TryParse(_badgeStep.Text, out var step) ? step : 1;
        set => _badgeStep.Text = Math.Max(1, value).ToString();
    }

    [Category("Action Block")]
    public string ActionIcon
    {
        get => _lblIcon.Text;
        set => _lblIcon.Text = string.IsNullOrWhiteSpace(value) ? "AP" : value;
    }

    [Category("Action Block")]
    public ActionBlockInputMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            _txtValue.Visible = value == ActionBlockInputMode.TextInput;
            _slider.Visible = value == ActionBlockInputMode.Slider;
            _lblValue.Visible = value == ActionBlockInputMode.Slider;
            _btnMute.Visible = value == ActionBlockInputMode.Slider;
            _btnUnmute.Visible = value == ActionBlockInputMode.Slider;
            _lblValue.Text = $"Volume Level: {_slider.Value}%";
        }
    }

    [Category("Action Block")]
    public string InputText
    {
        get => _txtValue.Text;
        set => _txtValue.Text = value;
    }

    [Category("Action Block")]
    public string PlaceholderText
    {
        get => _txtValue.PlaceholderText;
        set => _txtValue.PlaceholderText = value;
    }

    [Category("Action Block")]
    public int SliderValue
    {
        get => _slider.Value;
        set
        {
            _slider.Value = Math.Max(_slider.Minimum, Math.Min(_slider.Maximum, value));
            if (_slider.Value > 0)
            {
                _lastNonZeroVolume = _slider.Value;
            }
            _isMuted = _slider.Value == 0;
            _lblValue.Text = $"Volume Level: {_slider.Value}%";
            UpdateMuteVisualState();
        }
    }

    [Category("Action Block")]
    public int SliderMin
    {
        get => _slider.Minimum;
        set => _slider.Minimum = value;
    }

    [Category("Action Block")]
    public int SliderMax
    {
        get => _slider.Maximum;
        set => _slider.Maximum = Math.Max(value, _slider.Minimum + 1);
    }

    private void RefreshLayout()
    {
        var rightEdge = Math.Max(500, Width) - 14;
        _btnRemove.Location = new Point(rightEdge - _btnRemove.Width, 12);

        var inputX = _lblTitle.Left;
        var inputWidth = Math.Max(180, _btnRemove.Left - 12 - inputX);
        _txtValue.Location = new Point(inputX, 48);
        _txtValue.Size = new Size(inputWidth, 34);

        _lblValue.Location = new Point(inputX, 48);
        _lblValue.Size = new Size(180, 22);

        _btnUnmute.Location = new Point(inputX + inputWidth - _btnUnmute.Width, 45);
        _btnMute.Location = new Point(_btnUnmute.Left - 8 - _btnMute.Width, 45);

        _slider.Location = new Point(inputX - 2, 78);
        _slider.Size = new Size(Math.Max(160, inputWidth), 24);
    }

    private void UpdateMuteVisualState()
    {
        _btnMute.FillColor = _isMuted ? Color.FromArgb(176, 52, 52) : Color.FromArgb(130, 42, 42);
        _btnUnmute.FillColor = _isMuted ? Color.FromArgb(32, 110, 70) : Color.FromArgb(48, 148, 90);
    }
}

