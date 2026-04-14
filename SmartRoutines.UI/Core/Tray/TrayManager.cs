using System.Drawing.Drawing2D;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Core.Tray
{
    /// <summary>
    /// Manages the System Tray (NotifyIcon) lifecycle for SmartRoutines.
    /// 
    /// This class is entirely self-contained:
    ///   • Owns the NotifyIcon + its programmatically-generated icon
    ///   • Builds a fully owner-drawn dark-mode popup panel (not ContextMenuStrip)
    ///     to match the Fluent Design reference mockup exactly
    ///   • Exposes events so FrmMain can react without coupling
    ///   • Guarantees deterministic disposal — no ghost tray icons
    /// </summary>
    public sealed class TrayManager : IDisposable
    {
        // ─── Events for FrmMain integration ───────────────────────────────
        /// <summary>Raised when the user clicks "Open App" or double-clicks the tray icon.</summary>
        public event Action? RestoreRequested;

        /// <summary>Raised when the user clicks "Exit Completely".</summary>
        public event Action? ExitRequested;

        /// <summary>Raised when the user clicks "Disable All".</summary>
        public event Action? DisableAllRequested;

        /// <summary>Raised when the user clicks a routine in the Quick Run sub-menu.</summary>
        public event Action<string>? QuickRunRequested;

        // ─── Internal state ───────────────────────────────────────────────
        private readonly NotifyIcon _notifyIcon;
        private readonly Form _ownerForm;
        private TrayPopupForm? _popup;
        private bool _disposed;

        // ─── Palette (sourced from SmartTheme for single-source-of-truth) ─
        private static readonly Color BgDark       = Color.FromArgb(24, 24, 30);
        private static readonly Color BgItem       = Color.FromArgb(30, 30, 36);
        private static readonly Color BgHover      = Color.FromArgb(42, 42, 50);
        private static readonly Color BorderColor  = Color.FromArgb(50, 50, 58);
        private static readonly Color TextWhite    = SmartTheme.TextPrimary;
        private static readonly Color TextMuted    = SmartTheme.TextMuted;
        private static readonly Color AccentBlue   = SmartTheme.Primary;
        private static readonly Color AccentPurple = SmartTheme.Purple;
        private static readonly Color AccentGreen  = SmartTheme.Success;
        private static readonly Color DangerRed    = SmartTheme.Danger;

        // ─── Constructor ──────────────────────────────────────────────────
        public TrayManager(Form owner)
        {
            _ownerForm = owner ?? throw new ArgumentNullException(nameof(owner));

            _notifyIcon = new NotifyIcon
            {
                Icon = GenerateTrayIcon(),
                Text = "Smart Routines — Engine Active",
                Visible = true
            };

            _notifyIcon.MouseClick += OnTrayMouseClick;
            _notifyIcon.DoubleClick += (_, _) => RestoreRequested?.Invoke();

            // Safety net: ensure disposal even if the caller forgets
            Application.ApplicationExit += OnApplicationExit;
        }

        // ─── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Shows a brief balloon notification in the system tray.
        /// </summary>
        public void ShowBalloon(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (_disposed) return;
            _notifyIcon.ShowBalloonTip(3000, title, text, icon);
        }

        /// <summary>
        /// Updates the tray icon tooltip text (e.g. to reflect engine state).
        /// </summary>
        public void SetTooltip(string text)
        {
            if (_disposed) return;
            // NotifyIcon.Text max length is 127 characters
            _notifyIcon.Text = text.Length > 127 ? text[..127] : text;
        }

        // ─── Tray Click Handling ──────────────────────────────────────────

        private void OnTrayMouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                ShowPopup();
            }
        }

        private void ShowPopup()
        {
            // If one is already open, close it first
            if (_popup != null && !_popup.IsDisposed)
            {
                _popup.Close();
                _popup.Dispose();
                _popup = null;
                return; // Toggle behavior: click to open, click to close
            }

            _popup = new TrayPopupForm(this);
            _popup.Show();
        }

        // ─── Icon Generation (brand gradient ⚡ rendered at 16×16 ICO) ──

        private static Icon GenerateTrayIcon()
        {
            const int sz = 32;
            using var bmp = new Bitmap(sz, sz, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Gradient circle background (purple → blue)
            using var bgBrush = new LinearGradientBrush(
                new Rectangle(0, 0, sz, sz),
                SmartTheme.Purple, SmartTheme.Primary,
                LinearGradientMode.ForwardDiagonal);
            g.FillEllipse(bgBrush, 1, 1, sz - 2, sz - 2);

            // Lightning bolt ⚡ in white
            using var font = new Font("Segoe UI Symbol", 16f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);
            var fmt = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString("⚡", font, brush, new RectangleF(0, -1, sz, sz), fmt);

            return Icon.FromHandle(bmp.GetHicon());
        }

        // ─── Disposal ─────────────────────────────────────────────────────

        private void OnApplicationExit(object? sender, EventArgs e) => Dispose();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Application.ApplicationExit -= OnApplicationExit;

            _popup?.Close();
            _popup?.Dispose();
            _popup = null;

            _notifyIcon.Visible = false;
            _notifyIcon.Icon?.Dispose();
            _notifyIcon.Dispose();

            GC.SuppressFinalize(this);
        }

        ~TrayManager() => Dispose();

        // ═══════════════════════════════════════════════════════════════════
        //  INNER CLASS: The owner-drawn tray popup panel
        //  This is a borderless Form that pops up near the tray area,
        //  styled exactly like the Fluent dark-mode reference mockup.
        // ═══════════════════════════════════════════════════════════════════

        private sealed class TrayPopupForm : Form
        {
            private readonly TrayManager _manager;
            private readonly List<PopupItem> _items = new();
            private int _hoveredIndex = -1;
            private readonly List<QuickRunEntry> _quickRunEntries = new();

            // Layout constants
            private const int PopupWidth = 280;
            private const int ItemHeight = 38;
            private const int HeaderHeight = 48;
            private const int SeparatorHeight = 1;
            private const int FooterHeight = 36;
            private const int Rounding = 12;
            private const int IconSize = 18;
            private const int LeftPad = 16;

            public TrayPopupForm(TrayManager manager)
            {
                _manager = manager;

                // Borderless, tool-window (no taskbar entry), topmost
                FormBorderStyle = FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                ShowInTaskbar = false;
                TopMost = true;
                BackColor = BgDark;
                DoubleBuffered = true;
                AutoScaleMode = AutoScaleMode.Dpi;

                BuildItemList();
                CalculateSizeAndPosition();

                // Close on deactivation (click-away)
                Deactivate += (_, _) =>
                {
                    Close();
                    Dispose();
                };
            }

            private void BuildItemList()
            {
                _items.Clear();
                _quickRunEntries.Clear();

                // --- Header (non-clickable) ---
                _items.Add(new PopupItem(PopupItemType.Header, "Smart Routines"));

                // --- Separator ---
                _items.Add(new PopupItem(PopupItemType.Separator));

                // --- Quick Run parent ---
                _items.Add(new PopupItem(PopupItemType.QuickRunHeader, "QUICK RUN") { IsSectionLabel = true });

                // Populate dynamic routine entries
                // These would come from a service — for now use representative entries
                // that match the reference mockup. The public API allows injection.
                AddQuickRunEntry("Morning Setup");
                AddQuickRunEntry("Focus Mode");

                // --- Separator ---
                _items.Add(new PopupItem(PopupItemType.Separator));

                // --- Disable All ---
                _items.Add(new PopupItem(PopupItemType.Action, "Disable All")
                {
                    IconGlyph = "⊘",
                    GlyphColor = DangerRed,
                    OnClick = () => _manager.DisableAllRequested?.Invoke()
                });

                // --- Open App ---
                _items.Add(new PopupItem(PopupItemType.Action, "Open App")
                {
                    IconGlyph = "⚙",
                    GlyphColor = TextMuted,
                    OnClick = () =>
                    {
                        _manager.RestoreRequested?.Invoke();
                        Close();
                        Dispose();
                    }
                });

                // --- Exit Completely ---
                _items.Add(new PopupItem(PopupItemType.Action, "Exit Completely")
                {
                    IconGlyph = "⏻",
                    GlyphColor = DangerRed,
                    OnClick = () =>
                    {
                        Close();
                        Dispose();
                        _manager.ExitRequested?.Invoke();
                    }
                });

                // --- Separator ---
                _items.Add(new PopupItem(PopupItemType.Separator));

                // --- Footer (Engine Active status) ---
                _items.Add(new PopupItem(PopupItemType.Footer, "Engine Active"));
            }

            private void AddQuickRunEntry(string routineName)
            {
                int idx = _quickRunEntries.Count;
                _quickRunEntries.Add(new QuickRunEntry(routineName));
                _items.Add(new PopupItem(PopupItemType.QuickRunChild, routineName)
                {
                    IconGlyph = "▷",
                    GlyphColor = AccentBlue,
                    OnClick = () =>
                    {
                        _manager.QuickRunRequested?.Invoke(routineName);
                        Close();
                        Dispose();
                    }
                });
            }

            /// <summary>
            /// Allows external code to dynamically inject routine names into the Quick Run list.
            /// Call this BEFORE the popup is shown (i.e. override BuildItemList or expose a builder).
            /// </summary>
            public void InjectRoutines(IEnumerable<string> routineNames)
            {
                // Remove existing QuickRunChild items
                _items.RemoveAll(i => i.Type == PopupItemType.QuickRunChild);
                _quickRunEntries.Clear();

                // Find the index after the QuickRunHeader
                int insertIdx = _items.FindIndex(i => i.Type == PopupItemType.QuickRunHeader) + 1;

                foreach (var name in routineNames)
                {
                    var entry = new QuickRunEntry(name);
                    _quickRunEntries.Add(entry);

                    var item = new PopupItem(PopupItemType.QuickRunChild, name)
                    {
                        IconGlyph = "▷",
                        GlyphColor = AccentBlue,
                        OnClick = () =>
                        {
                            _manager.QuickRunRequested?.Invoke(name);
                            Close();
                            Dispose();
                        }
                    };
                    _items.Insert(insertIdx++, item);
                }

                CalculateSizeAndPosition();
                Invalidate();
            }

            private void CalculateSizeAndPosition()
            {
                int totalHeight = 0;
                foreach (var item in _items)
                {
                    totalHeight += item.Type switch
                    {
                        PopupItemType.Header => HeaderHeight,
                        PopupItemType.Separator => SeparatorHeight + 10, // 5px padding above/below
                        PopupItemType.Footer => FooterHeight,
                        PopupItemType.QuickRunHeader => 28,
                        _ => ItemHeight
                    };
                }
                totalHeight += 16; // top + bottom padding

                Size = new Size(PopupWidth, totalHeight);

                // Position above the system tray (bottom-right of screen)
                var workArea = Screen.PrimaryScreen!.WorkingArea;
                Location = new Point(
                    workArea.Right - PopupWidth - 12,
                    workArea.Bottom - totalHeight - 8
                );
            }

            // ─── Painting ─────────────────────────────────────────────────

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Rounded background
                using var bgPath = RoundedRect(ClientRectangle, Rounding);
                using var bgBrush = new SolidBrush(BgDark);
                g.FillPath(bgBrush, bgPath);

                // 1px border
                using var borderPen = new Pen(BorderColor, 1f);
                using var borderPath = RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), Rounding);
                g.DrawPath(borderPen, borderPath);

                // Draw items
                int y = 8;
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    int itemH = item.Type switch
                    {
                        PopupItemType.Header => HeaderHeight,
                        PopupItemType.Separator => SeparatorHeight + 10,
                        PopupItemType.Footer => FooterHeight,
                        PopupItemType.QuickRunHeader => 28,
                        _ => ItemHeight
                    };

                    var itemRect = new Rectangle(0, y, Width, itemH);
                    item.Bounds = itemRect;

                    switch (item.Type)
                    {
                        case PopupItemType.Header:
                            DrawHeader(g, itemRect, item);
                            break;
                        case PopupItemType.Separator:
                            DrawSeparator(g, itemRect);
                            break;
                        case PopupItemType.QuickRunHeader:
                            DrawSectionLabel(g, itemRect, item);
                            break;
                        case PopupItemType.QuickRunChild:
                        case PopupItemType.Action:
                            DrawMenuItem(g, itemRect, item, i == _hoveredIndex);
                            break;
                        case PopupItemType.Footer:
                            DrawFooter(g, itemRect, item);
                            break;
                    }

                    y += itemH;
                }
            }

            private void DrawHeader(Graphics g, Rectangle r, PopupItem item)
            {
                // App icon (gradient circle with lightning bolt)
                var iconRect = new Rectangle(LeftPad, r.Y + (r.Height - 26) / 2, 26, 26);
                using var gradBrush = new LinearGradientBrush(iconRect, AccentPurple, AccentBlue, LinearGradientMode.ForwardDiagonal);
                g.FillEllipse(gradBrush, iconRect);

                // ⚡ glyph inside the circle
                using var boltFont = new Font("Segoe UI Symbol", 10f, FontStyle.Bold);
                using var whiteBrush = new SolidBrush(Color.White);
                var boltFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("⚡", boltFont, whiteBrush, iconRect, boltFmt);

                // Title text
                using var titleFont = new Font("Segoe UI", 12f, FontStyle.Bold);
                var textRect = new Rectangle(iconRect.Right + 10, r.Y, r.Width - iconRect.Right - 50, r.Height);
                g.DrawString(item.Text, titleFont, whiteBrush, textRect, new StringFormat { LineAlignment = StringAlignment.Center });

                // Close X button
                var closeRect = new Rectangle(r.Width - 38, r.Y + (r.Height - 22) / 2, 22, 22);
                _closeButtonBounds = closeRect;
                bool closeHover = closeRect.Contains(PointToClient(Cursor.Position));
                using var closeBg = new SolidBrush(closeHover ? Color.FromArgb(60, 60, 68) : Color.Transparent);
                using var closePath = RoundedRect(closeRect, 4);
                g.FillPath(closeBg, closePath);

                using var closeFont = new Font("Segoe UI", 9f);
                using var closeBrush = new SolidBrush(closeHover ? TextWhite : TextMuted);
                g.DrawString("✕", closeFont, closeBrush, closeRect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            private Rectangle _closeButtonBounds;

            private static void DrawSeparator(Graphics g, Rectangle r)
            {
                int lineY = r.Y + r.Height / 2;
                using var pen = new Pen(BorderColor, 1f);
                g.DrawLine(pen, LeftPad, lineY, r.Width - LeftPad, lineY);
            }

            private static void DrawSectionLabel(Graphics g, Rectangle r, PopupItem item)
            {
                using var font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var brush = new SolidBrush(TextMuted);
                g.DrawString(item.Text, font, brush, LeftPad, r.Y + (r.Height - 14) / 2 + 2);
            }

            private void DrawMenuItem(Graphics g, Rectangle r, PopupItem item, bool hovered)
            {
                var innerRect = new Rectangle(8, r.Y + 2, r.Width - 16, r.Height - 4);

                if (hovered)
                {
                    using var hoverBrush = new SolidBrush(BgHover);
                    using var hoverPath = RoundedRect(innerRect, 6);
                    g.FillPath(hoverBrush, hoverPath);
                }

                // Icon glyph
                if (item.IconGlyph != null)
                {
                    using var glyphFont = new Font("Segoe UI Symbol", 10f);
                    using var glyphBrush = new SolidBrush(item.GlyphColor);
                    var glyphRect = new Rectangle(LeftPad + 4, r.Y, 24, r.Height);
                    g.DrawString(item.IconGlyph, glyphFont, glyphBrush, glyphRect,
                        new StringFormat { LineAlignment = StringAlignment.Center });
                }

                // Text
                int textX = LeftPad + (item.IconGlyph != null ? 32 : 0);
                using var textFont = new Font("Segoe UI", 10f);
                using var textBrush = new SolidBrush(hovered ? TextWhite : Color.FromArgb(220, 220, 225));
                var textRect = new Rectangle(textX, r.Y, r.Width - textX - 16, r.Height);
                g.DrawString(item.Text, textFont, textBrush, textRect,
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }

            private static void DrawFooter(Graphics g, Rectangle r, PopupItem item)
            {
                // Green dot
                int dotSize = 8;
                int dotY = r.Y + (r.Height - dotSize) / 2;
                using var dotBrush = new SolidBrush(AccentGreen);
                g.FillEllipse(dotBrush, LeftPad + 2, dotY, dotSize, dotSize);

                // "Engine Active" text
                using var font = new Font("Segoe UI", 9f);
                using var brush = new SolidBrush(TextMuted);
                g.DrawString(item.Text, font, brush, LeftPad + 16, r.Y + (r.Height - 14) / 2);
            }

            // ─── Mouse interaction ────────────────────────────────────────

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                int newHover = -1;
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].IsClickable && _items[i].Bounds.Contains(e.Location))
                    {
                        newHover = i;
                        break;
                    }
                }

                if (newHover != _hoveredIndex)
                {
                    _hoveredIndex = newHover;
                    Cursor = newHover >= 0 ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }

                // Also check close button hover for repaint
                if (_closeButtonBounds.Contains(e.Location))
                    Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (_hoveredIndex != -1)
                {
                    _hoveredIndex = -1;
                    Cursor = Cursors.Default;
                    Invalidate();
                }
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);

                // Close button
                if (_closeButtonBounds.Contains(e.Location))
                {
                    Close();
                    Dispose();
                    return;
                }

                // Menu items
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].IsClickable && _items[i].Bounds.Contains(e.Location))
                    {
                        _items[i].OnClick?.Invoke();
                        return;
                    }
                }
            }

            // ─── Rounded-rect helper ──────────────────────────────────────

            private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
            {
                int d = radius * 2;
                var path = new GraphicsPath();
                path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
                path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
                path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                return path;
            }

            // ─── Drop-shadow via CreateParams ─────────────────────────────

            protected override CreateParams CreateParams
            {
                get
                {
                    const int CS_DROPSHADOW = 0x00020000;
                    var cp = base.CreateParams;
                    cp.ClassStyle |= CS_DROPSHADOW;
                    return cp;
                }
            }

            // Prevent ALT+F4 from killing the popup — just close it
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
                {
                    e.Handled = true;
                    Close();
                    Dispose();
                    return;
                }
                base.OnKeyDown(e);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Data models for popup items
        // ═══════════════════════════════════════════════════════════════════

        private enum PopupItemType
        {
            Header,
            Separator,
            QuickRunHeader,
            QuickRunChild,
            Action,
            Footer
        }

        private sealed class PopupItem
        {
            public PopupItemType Type { get; }
            public string Text { get; }
            public string? IconGlyph { get; init; }
            public Color GlyphColor { get; init; } = TextMuted;
            public Action? OnClick { get; init; }
            public bool IsSectionLabel { get; init; }
            public Rectangle Bounds { get; set; }

            public bool IsClickable =>
                Type == PopupItemType.Action || Type == PopupItemType.QuickRunChild;

            public PopupItem(PopupItemType type, string text = "")
            {
                Type = type;
                Text = text;
            }
        }

        private sealed record QuickRunEntry(string RoutineName);
    }
}
