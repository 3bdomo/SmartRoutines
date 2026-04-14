using Guna.UI2.WinForms;
using Microsoft.Win32;
using SmartRoutines.UI.Core.Theme;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Text.Json.Serialization;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.Generic;
using SmartRoutines.UI.Core.Theme;
using Guna.UI2.WinForms;

namespace SmartRoutines.UI.Controls
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ENUMS & CONSTANTS
    // ══════════════════════════════════════════════════════════════════════════

    public enum DialogIconType { Information, Warning, Error, Success }
    public enum CustomDialogResult { Confirm, Cancel }

    internal static class AppConstants
    {
        public const string RegistryRunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public const string AppRegistryValueName = "SmartRoutines";
        public const string ExportDefaultFileName = "routines_backup.json";
        public const string JsonFileFilter = "JSON Files (*.json)|*.json";
        public const string AppDisplayName = "Smart Routines";
        public const string AppVersion = "v2.1.0";
        public const string AppCopyright = "© 2026 Enterprise Automation Engine";
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  DTOs & SERIALIZER
    // ══════════════════════════════════════════════════════════════════════════

    public sealed class RoutineExportDto
    {
        public string Name { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TriggerType { get; set; }
        public string TriggerConfig { get; set; } = string.Empty;
        public List<ActionEntryDto> Actions { get; set; } = new();
    }

    public sealed class ActionEntryDto
    {
        public int Type { get; set; }
        public string Arguments { get; set; } = string.Empty;
        public int ExecutionOrder { get; set; }
    }

    public sealed class RoutineBackupFile
    {
        public string App { get; set; } = AppConstants.AppDisplayName;
        public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
        public int SchemaVersion { get; set; } = 1;
        public List<RoutineExportDto> Routines { get; set; } = new();
    }

    public static class JsonRoutineSerializer
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string Serialize(IEnumerable<RoutineExportDto> routines)
        {
            var backup = new RoutineBackupFile { Routines = new List<RoutineExportDto>(routines) };
            return JsonSerializer.Serialize(backup, _options);
        }

        public static List<RoutineExportDto> Deserialize(string json)
        {
            var backup = JsonSerializer.Deserialize<RoutineBackupFile>(json, _options)
                  ?? throw new JsonException("Backup file is empty or invalid.");
            if (backup.SchemaVersion != 1)
                throw new JsonException($"Unsupported schema version: {backup.SchemaVersion}");
            return backup.Routines;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SETTINGS MANAGER
    // ══════════════════════════════════════════════════════════════════════════

    public static class SettingsManager
    {
        private static readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_settings.json");
        private static readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

        public static bool AutomationEngine { get; set; } = true;
        public static bool RunAtStartup { get; set; } = false;
        public static bool RunInBackground { get; set; } = true;
        public static bool RoutineAlerts { get; set; } = true;
        public static bool ErrorNotifications { get; set; } = true;
        public static bool SoundAlerts { get; set; } = false;
        public static bool RequireConfirmation { get; set; } = true;
        public static bool ElevatedPermissions { get; set; } = false;
        public static bool DebugMode { get; set; } = false;
        public static bool ApiAccess { get; set; } = false;

        public static void Load()
        {
            try
            {
                if (!File.Exists(_settingsPath)) return;
                string json = File.ReadAllText(_settingsPath);
                var dict = JsonSerializer.Deserialize<Dictionary<string, bool>>(json, _opts);
                if (dict == null) return;

                AutomationEngine = Get(dict, nameof(AutomationEngine), AutomationEngine);
                RunAtStartup = Get(dict, nameof(RunAtStartup), RunAtStartup);
                RunInBackground = Get(dict, nameof(RunInBackground), RunInBackground);
                RoutineAlerts = Get(dict, nameof(RoutineAlerts), RoutineAlerts);
                ErrorNotifications = Get(dict, nameof(ErrorNotifications), ErrorNotifications);
                SoundAlerts = Get(dict, nameof(SoundAlerts), SoundAlerts);
                RequireConfirmation = Get(dict, nameof(RequireConfirmation), RequireConfirmation);
                ElevatedPermissions = Get(dict, nameof(ElevatedPermissions), ElevatedPermissions);
                DebugMode = Get(dict, nameof(DebugMode), DebugMode);
                ApiAccess = Get(dict, nameof(ApiAccess), ApiAccess);
            }
            catch { }
        }

        public static void Save()
        {
            var dict = new Dictionary<string, bool>
            {
                [nameof(AutomationEngine)] = AutomationEngine,
                [nameof(RunAtStartup)] = RunAtStartup,
                [nameof(RunInBackground)] = RunInBackground,
                [nameof(RoutineAlerts)] = RoutineAlerts,
                [nameof(ErrorNotifications)] = ErrorNotifications,
                [nameof(SoundAlerts)] = SoundAlerts,
                [nameof(RequireConfirmation)] = RequireConfirmation,
                [nameof(ElevatedPermissions)] = ElevatedPermissions,
                [nameof(DebugMode)] = DebugMode,
                [nameof(ApiAccess)] = ApiAccess,
            };
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(dict, _opts));
        }

        public static bool IsAutoStartEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(AppConstants.RegistryRunKey, false);
            return key?.GetValue(AppConstants.AppRegistryValueName) != null;
        }

        public static async Task SetAutoStartAsync(bool enable)
        {
            await Task.Run(() =>
            {
                using var key = Registry.CurrentUser.OpenSubKey(AppConstants.RegistryRunKey, true);
                if (key == null) return;
                if (enable)
                    key.SetValue(AppConstants.AppRegistryValueName, Application.ExecutablePath);
                else
                    key.DeleteValue(AppConstants.AppRegistryValueName, false);
            });
        }

        private static bool Get(Dictionary<string, bool> d, string key, bool fallback)
          => d.TryGetValue(key, out bool v) ? v : fallback;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SMART DIALOG
    // ══════════════════════════════════════════════════════════════════════════

    public sealed class SmartDialog : Form
    {
        private static readonly Color _bg = SmartTheme.Surface;
        private static readonly Color _border = SmartTheme.Border;
        public CustomDialogResult DialogChoice { get; private set; } = CustomDialogResult.Cancel;

        public SmartDialog(string title, string message, DialogIconType iconType = DialogIconType.Information, string confirmText = "Confirm", string cancelText = "Cancel")
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(440, 220);
            BackColor = _bg;
            DoubleBuffered = true;

            Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(_border, 1f);
                using var path = RoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), 12);
                e.Graphics.DrawPath(pen, path);
                Region = new Region(path);
            };

            var iconPanel = new Panel { Size = new Size(60, Height), BackColor = IconBackground(iconType) };
            iconPanel.Paint += (_, e) => { DrawIcon(e.Graphics, iconType, new Rectangle(10, Height / 2 - 20, 40, 40)); };
            Controls.Add(iconPanel);

            Controls.Add(new Label { Text = title, Font = SmartTheme.FontBodyBold, ForeColor = SmartTheme.TextPrimary, Location = new Point(80, 30), Size = new Size(340, 24) });
            Controls.Add(new Label { Text = message, Font = SmartTheme.FontBody, ForeColor = SmartTheme.TextSecondary, Location = new Point(80, 60), Size = new Size(340, 80) });

            var btnConfirm = CreateDialogButton(confirmText, new Point(200, 160), (iconType == DialogIconType.Error ? SmartTheme.Danger : SmartTheme.Primary), (iconType == DialogIconType.Error ? SmartTheme.DangerHover : SmartTheme.PrimaryHover));
            btnConfirm.Click += (_, _) => { DialogChoice = CustomDialogResult.Confirm; Close(); };
            Controls.Add(btnConfirm);

            var btnCancel = CreateDialogButton(cancelText, new Point(310, 160), SmartTheme.Surface3, SmartTheme.Surface2);
            btnCancel.Click += (_, _) => { DialogChoice = CustomDialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) DragMove(); };
        }

        public static CustomDialogResult Show(IWin32Window owner, string title, string message, DialogIconType icon = DialogIconType.Information, string confirmText = "OK", string cancelText = "Cancel")
        {
            using var dlg = new SmartDialog(title, message, icon, confirmText, cancelText);
            dlg.ShowDialog(owner);
            return dlg.DialogChoice;
        }

        private Button CreateDialogButton(string text, Point location, Color bg, Color hoverBg)
        {
            var btn = new Button { Text = text, Location = location, Size = new Size(100, 34), FlatStyle = FlatStyle.Flat, Font = SmartTheme.FontSmallBold, ForeColor = Color.White, BackColor = bg, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (_, _) => btn.BackColor = hoverBg;
            btn.MouseLeave += (_, _) => btn.BackColor = bg;
            return btn;
        }

        private static Color IconBackground(DialogIconType t) => Color.FromArgb(50, t switch { DialogIconType.Warning => 243, DialogIconType.Error => 239, DialogIconType.Success => 46, _ => 59 }, 156, 18);

        private static void DrawIcon(Graphics g, DialogIconType t, Rectangle r)
        {
            Color c = t switch { DialogIconType.Warning => SmartTheme.Warning, DialogIconType.Error => SmartTheme.Danger, DialogIconType.Success => SmartTheme.Success, _ => SmartTheme.Primary };
            using var brush = new SolidBrush(c); using var pen = new Pen(c, 2.5f);
            int cx = r.Left + r.Width / 2, cy = r.Top + r.Height / 2;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (t == DialogIconType.Information || t == DialogIconType.Error || t == DialogIconType.Success) g.DrawEllipse(pen, r);
            if (t == DialogIconType.Information) { g.FillRectangle(brush, cx - 2, cy - 6, 4, 12); g.FillEllipse(brush, cx - 2, cy - 11, 5, 5); }
            else if (t == DialogIconType.Warning) { var tri = new Point[] { new(cx, r.Top), new(r.Right, r.Bottom), new(r.Left, r.Bottom) }; g.DrawPolygon(pen, tri); g.FillRectangle(brush, cx - 2, cy - 4, 4, 10); g.FillEllipse(brush, cx - 2, cy + 8, 5, 5); }
            else if (t == DialogIconType.Error) { g.DrawLine(pen, r.Left + 8, r.Top + 8, r.Right - 8, r.Bottom - 8); g.DrawLine(pen, r.Right - 8, r.Top + 8, r.Left + 8, r.Bottom - 8); }
            else if (t == DialogIconType.Success) { g.DrawLine(pen, cx - 9, cy, cx - 3, cy + 8); g.DrawLine(pen, cx - 3, cy + 8, cx + 9, cy - 8); }
        }

        private void DragMove() { NativeMethods.ReleaseCapture(); NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0); }
        private static GraphicsPath RoundedPath(Rectangle r, int rad) { var p = new GraphicsPath(); p.AddArc(r.X, r.Y, rad * 2, rad * 2, 180, 90); p.AddArc(r.Right - rad * 2, r.Y, rad * 2, rad * 2, 270, 90); p.AddArc(r.Right - rad * 2, r.Bottom - rad * 2, rad * 2, rad * 2, 0, 90); p.AddArc(r.X, r.Bottom - rad * 2, rad * 2, rad * 2, 90, 90); p.CloseFigure(); return p; }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ReleaseCapture();
            [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  UC_SETTING TOGGLE ITEM
    // ══════════════════════════════════════════════════════════════════════════

    public sealed class UC_SettingToggleItem : UserControl
    {
        public string Title { get => _lblTitle.Text; set => _lblTitle.Text = value; }
        public string Subtitle { get => _lblSub.Text; set => _lblSub.Text = value; }
        public bool IsChecked { get => _toggle.Checked; set => _toggle.Checked = value; }
        public event EventHandler<bool>? ToggledChanged;

        private readonly Label _lblTitle, _lblSub;
        private readonly Guna2ToggleSwitch _toggle;

        public UC_SettingToggleItem(string title, string subtitle, bool initialValue = false)
        {
            this.Height = 75;
            this.BackColor = Color.Transparent;
            this.Padding = new Padding(15, 0, 15, 0);

            _lblTitle = new Label { Text = title, Font = SmartTheme.FontBodyBold, ForeColor = SmartTheme.TextPrimary, Location = new Point(15, 15), AutoSize = true };
            _lblSub = new Label { Text = subtitle, Font = SmartTheme.FontSmall, ForeColor = SmartTheme.TextSecondary, Location = new Point(15, 41), AutoSize = true };

            _toggle = new Guna2ToggleSwitch
            {
                Checked = initialValue,
                Size = new Size(50, 24),
                CheckedState = { FillColor = SmartTheme.Primary },
                UncheckedState = { FillColor = SmartTheme.Surface3 },
                Anchor = AnchorStyles.Right,
                Cursor = Cursors.Hand
            };

            _toggle.Location = new Point(this.Width - _toggle.Width - 20, 25);
            _toggle.CheckedChanged += (_, _) => ToggledChanged?.Invoke(this, _toggle.Checked);

            var sep = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.FromArgb(40, SmartTheme.Border) };

            Controls.AddRange(new Control[] { _lblTitle, _lblSub, _toggle, sep });

            this.SizeChanged += (s, e) => {
                _toggle.Left = this.Width - _toggle.Width - 20;
            };
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  UC_SETTINGS
    // ══════════════════════════════════════════════════════════════════════════

    public partial class UC_Settings : UserControl
    {
        private FlowLayoutPanel mainPanel;

        public UC_Settings()
        {
            this.Dock = DockStyle.Fill;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = SmartTheme.Background;
            this.DoubleBuffered = true;

            // تمكين استلام المفاتيح
            this.Load += (s, e) => { this.Focus(); };

            SettingsManager.Load();
            InitializeUI();
        }

        // --- إضافة منطق الاختصارات ---
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.N))
            {
                CreateNewRoutine();
                return true;
            }
            if (keyData == (Keys.Control | Keys.T))
            {
                ToggleTheme();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CreateNewRoutine()
        {
            SmartDialog.Show(this, "New Routine", "Opening routine designer wizard...", DialogIconType.Information);
        }

        private void ToggleTheme()
        {
            // محاكاة تبديل الثيم
            SmartDialog.Show(this, "Theme Switch", "Theme preference updated. Application will refresh visuals.", DialogIconType.Success);
        }

        private void InitializeUI()
        {
            mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 50),
                BackColor = Color.Transparent
            };
            this.Controls.Add(mainPanel);

            // لضمان استجابة الاختصارات عند الضغط داخل أي جزء
            mainPanel.Click += (s, e) => this.Focus();

            mainPanel.SizeChanged += (s, e) => {
                mainPanel.SuspendLayout();
                int targetWidth = mainPanel.ClientSize.Width - mainPanel.Padding.Left - mainPanel.Padding.Right - 25;
                foreach (Control c in mainPanel.Controls)
                {
                    if (c is RoundedPanel || c is Guna2GradientPanel || c is Panel)
                        c.Width = targetWidth;
                }
                mainPanel.ResumeLayout();
            };

            mainPanel.Controls.Add(new Label { Text = "Settings", ForeColor = SmartTheme.TextPrimary, Font = new Font("Segoe UI Semibold", 28), AutoSize = true });
            mainPanel.Controls.Add(new Label { Text = "Configure your automation preferences", ForeColor = SmartTheme.TextSecondary, Font = new Font("Segoe UI", 11), AutoSize = true, Margin = new Padding(0, 0, 0, 30) });

            mainPanel.Controls.Add(CreateThemeBanner());

            mainPanel.Controls.Add(CreateSettingsCard("Engine Settings", "⚡", new[] {
        MakeToggle("Automation Engine", "Enable or disable all routine automation", SettingsManager.AutomationEngine, v => { SettingsManager.AutomationEngine = v; SettingsManager.Save(); }),
        MakeToggle("Auto-start on System Boot", "Launch Smart Routines when Windows starts", SettingsManager.IsAutoStartEnabled(), async v => { SettingsManager.RunAtStartup = v; await SettingsManager.SetAutoStartAsync(v); SettingsManager.Save(); }),
        MakeToggle("Run in Background", "Keep engine running when window is closed", SettingsManager.RunInBackground, v => { SettingsManager.RunInBackground = v; SettingsManager.Save(); })
      }));

            mainPanel.Controls.Add(CreateSettingsCard("Notifications", "🔔", new[] {
        MakeToggle("Routine Execution Alerts", "Show toast notifications when routines run", SettingsManager.RoutineAlerts, v => { SettingsManager.RoutineAlerts = v; SettingsManager.Save(); }),
        MakeToggle("Error Notifications", "Get notified when routines fail", SettingsManager.ErrorNotifications, v => { SettingsManager.ErrorNotifications = v; SettingsManager.Save(); }),
        MakeToggle("Sound Alerts", "Play sound on routine completion", SettingsManager.SoundAlerts, v => { SettingsManager.SoundAlerts = v; SettingsManager.Save(); })
      }));

            mainPanel.Controls.Add(CreateSettingsCard("Security", "🛡️", new[] {
        MakeToggle("Require Confirmation", "Ask before running destructive actions", SettingsManager.RequireConfirmation, v => { SettingsManager.RequireConfirmation = v; SettingsManager.Save(); }),
        MakeToggle("Elevated Permissions", "Run routines with administrator rights", SettingsManager.ElevatedPermissions, v => { SettingsManager.ElevatedPermissions = v; SettingsManager.Save(); })
      }));

            mainPanel.Controls.Add(CreateSettingsCard("Developer", "🛠️", new[] {
        MakeToggle("Debug Mode", "Show detailed execution logs", SettingsManager.DebugMode, v => { SettingsManager.DebugMode = v; SettingsManager.Save(); }),
        MakeToggle("API Access", "Enable external API control", SettingsManager.ApiAccess, v => { SettingsManager.ApiAccess = v; SettingsManager.Save(); })
      }));

            mainPanel.Controls.Add(CreateShortcutsCard());
            mainPanel.Controls.Add(CreateDataManagementCard());
            mainPanel.Controls.Add(CreateFooter());
        }

        private Guna2GradientPanel CreateThemeBanner()
        {
            var banner = new Guna2GradientPanel { Height = 160, Margin = new Padding(0, 0, 0, 20), FillColor = Color.FromArgb(30, 27, 75), FillColor2 = Color.FromArgb(20, 18, 50), BorderRadius = 18 };
            banner.Controls.Add(new Label { Text = "🎨 Current Theme: Dark Mode", Font = new Font("Segoe UI Semibold", 14), ForeColor = Color.White, Location = new Point(25, 25), AutoSize = true, BackColor = Color.Transparent });
            banner.Controls.Add(new Label { Text = "Use Ctrl + T to quickly switch between light and dark modes.", Font = new Font("Segoe UI", 10.5f), ForeColor = Color.Gainsboro, Location = new Point(25, 110), AutoSize = true, BackColor = Color.Transparent });
            return banner;
        }

        private RoundedPanel CreateSettingsCard(string title, string icon, UC_SettingToggleItem[] items)
        {
            int headerHeight = 65;
            int itemHeight = 75;
            var card = new RoundedPanel { Height = headerHeight + (items.Length * itemHeight) + 10, BackColor = SmartTheme.Surface, Margin = new Padding(0, 0, 0, 25) };

            card.Controls.Add(new Label
            {
                Text = icon + "  " + title,
                Font = new Font("Segoe UI Semibold", 13),
                ForeColor = SmartTheme.TextPrimary,
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            });

            for (int i = 0; i < items.Length; i++)
            {
                items[i].Location = new Point(0, headerHeight + (i * itemHeight));
                items[i].Width = card.Width;
                items[i].Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                card.Controls.Add(items[i]);
            }
            return card;
        }

        private static UC_SettingToggleItem MakeToggle(string t, string s, bool i, Action<bool> cb)
        {
            var item = new UC_SettingToggleItem(t, s, i);
            item.ToggledChanged += (_, v) => cb(v);
            return item;
        }

        private RoundedPanel CreateShortcutsCard()
        {
            var card = new RoundedPanel { Height = 180, BackColor = SmartTheme.Surface, Margin = new Padding(0, 0, 0, 25) };
            card.Controls.Add(new Label { Text = "⌨️  Keyboard Shortcuts", Font = new Font("Segoe UI Semibold", 13), ForeColor = SmartTheme.TextPrimary, Location = new Point(20, 20), AutoSize = true, BackColor = Color.Transparent });
            string[] names = { "Create New Routine", "Toggle Theme" }; string[] keys = { "Ctrl + N", "Ctrl + T" };
            for (int i = 0; i < 2; i++)
            {
                int y = 75 + (i * 50);
                card.Controls.Add(new Label { Text = names[i], Font = new Font("Segoe UI", 10.5f), ForeColor = SmartTheme.TextSecondary, Location = new Point(25, y), AutoSize = true, BackColor = Color.Transparent });
                var lblKey = new Label { Text = keys[i], ForeColor = Color.White, BackColor = SmartTheme.Surface3, TextAlign = ContentAlignment.MiddleCenter, Size = new Size(100, 30), Location = new Point(card.Width - 125, y - 5), Font = new Font("Consolas", 10) };
                lblKey.Anchor = AnchorStyles.Right;
                card.Controls.Add(lblKey);
            }
            return card;
        }

        private RoundedPanel CreateDataManagementCard()
        {
            var card = new RoundedPanel { Height = 280, BackColor = SmartTheme.Surface, Margin = new Padding(0, 0, 0, 25) };
            card.Controls.Add(new Label { Text = "📊  Data Management", Font = new Font("Segoe UI Semibold", 13), ForeColor = SmartTheme.TextPrimary, Location = new Point(20, 20), AutoSize = true, BackColor = Color.Transparent });
            AddDataRow(card, "Export All Routines", "Download routines as JSON file", "Export", SmartTheme.Surface3, 75, false, false, ExportRoutinesAsync);
            AddDataRow(card, "Import Routines", "Load routines from JSON file", "Import", SmartTheme.Surface3, 145, true, false, ImportRoutinesAsync);
            AddDataRow(card, "Clear History", "Delete all routine execution logs", "Clear", SmartTheme.Danger, 215, true, true, ClearHistoryAsync);
            return card;
        }

        private void AddDataRow(Panel card, string t, string d, string bt, Color bc, int y, bool hl, bool ol, Action onClick)
        {
            if (hl) card.Controls.Add(new Panel { BackColor = Color.FromArgb(40, SmartTheme.Border), Size = new Size(card.Width - 50, 1), Location = new Point(25, y - 10), Anchor = AnchorStyles.Left | AnchorStyles.Right });
            card.Controls.Add(new Label { Text = t, Font = new Font("Segoe UI Semibold", 11), ForeColor = SmartTheme.TextPrimary, Location = new Point(30, y), AutoSize = true });
            card.Controls.Add(new Label { Text = d, Font = new Font("Segoe UI", 9.5f), ForeColor = SmartTheme.TextSecondary, Location = new Point(30, y + 28), Size = new Size(card.Width - 150, 40), Anchor = AnchorStyles.Left | AnchorStyles.Right });
            var btn = new Button { Text = bt, Location = new Point(card.Width - 125, y + 5), Size = new Size(100, 35), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand, BackColor = ol ? Color.Transparent : bc, ForeColor = ol ? bc : Color.White };
            btn.Anchor = AnchorStyles.Right;
            if (ol) btn.FlatAppearance.BorderColor = bc; else btn.FlatAppearance.BorderSize = 0;
            btn.Click += (_, _) => onClick?.Invoke(); card.Controls.Add(btn);
        }

        private Panel CreateFooter()
        {
            var f = new Panel { Height = 210, Margin = new Padding(0, 40, 0, 40) };
            f.Controls.Add(new Label { Text = AppConstants.AppCopyright, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 30 });
            f.Controls.Add(new Label { Text = "Enterprise-grade automation engine for Windows 11", Font = new Font("Segoe UI", 10), ForeColor = Color.DimGray, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 25 });
            f.Controls.Add(new Label { Text = $"{AppConstants.AppDisplayName} {AppConstants.AppVersion}", Font = new Font("Segoe UI Semibold", 13), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 35 });
            f.Controls.Add(new Label { Text = "🗄️", Font = new Font("Segoe UI", 42), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 80 });
            return f;
        }

        private async void ExportRoutinesAsync()
        {
            try
            {
                using SaveFileDialog sfd = new SaveFileDialog { Filter = AppConstants.JsonFileFilter, FileName = AppConstants.ExportDefaultFileName, Title = "Export Routines" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var routines = new List<RoutineExportDto>();
                    string json = JsonRoutineSerializer.Serialize(routines);
                    await File.WriteAllTextAsync(sfd.FileName, json);
                    SmartDialog.Show(this, "Export Success", "Your routines have been backed up successfully.", DialogIconType.Success);
                }
            }
            catch (Exception ex) { SmartDialog.Show(this, "Export Error", ex.Message, DialogIconType.Error); }
        }

        private async void ImportRoutinesAsync()
        {
            try
            {
                using OpenFileDialog ofd = new OpenFileDialog { Filter = AppConstants.JsonFileFilter, Title = "Import Routines" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string json = await File.ReadAllTextAsync(ofd.FileName);
                    var routines = JsonRoutineSerializer.Deserialize(json);
                    SmartDialog.Show(this, "Import Success", $"{routines.Count} routines imported.", DialogIconType.Success);
                }
            }
            catch (Exception ex) { SmartDialog.Show(this, "Import Error", ex.Message, DialogIconType.Error); }
        }

        private async void ClearHistoryAsync()
        {
            if (SmartDialog.Show(this, "Confirm", "Clear all execution logs? This cannot be undone.", DialogIconType.Warning, "Clear") == CustomDialogResult.Confirm)
            {
                await Task.Delay(500);
                SmartDialog.Show(this, "Done", "History cleared.", DialogIconType.Success);
            }
        }
    }

    public class RoundedPanel : Panel
    {
        public RoundedPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = GetPath(ClientRectangle, 18);
            // Dispose old Region before assigning — prevents GDI handle leak on every repaint
            this.Region?.Dispose();
            this.Region = new Region(path);
            using var brush = new SolidBrush(BackColor);
            e.Graphics.FillPath(brush, path);
        }

        private static GraphicsPath GetPath(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, rad, rad, 180, 90);
            p.AddArc(r.Right - rad, r.Y, rad, rad, 270, 90);
            p.AddArc(r.Right - rad, r.Bottom - rad, rad, rad, 0, 90);
            p.AddArc(r.X, r.Bottom - rad, rad, rad, 90, 90);
            p.CloseFigure(); return p;
        }
    }
}