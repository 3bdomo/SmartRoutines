namespace SmartRoutines.UI.Core.Theme
{
    /// <summary>
    /// Centralized design-system tokens for SmartRoutines dark-mode UI.
    /// All colors, fonts, dimensions, spacing, and radii are defined here
    /// so that every Form and UserControl draws from a single source of truth.
    /// 
    /// Naming convention follows the allColors.html design-token document.
    /// </summary>
    public static class SmartTheme
    {
        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  PRIMARY / BRAND COLORS                                        ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Primary Blue – buttons, active states, links (#3B82F6).</summary>
        public static readonly Color Primary = ColorTranslator.FromHtml("#3B82F6");

        /// <summary>Primary Blue darkened – button hover state.</summary>
        public static readonly Color PrimaryHover = ColorTranslator.FromHtml("#2563EB");

        /// <summary>Primary Blue 10 % opacity – subtle highlights, selected card tint.</summary>
        public static readonly Color Primary10 = Color.FromArgb(26, 59, 130, 246);

        /// <summary>Primary Blue 30 % opacity – focus rings, step-progress lines.</summary>
        public static readonly Color Primary30 = Color.FromArgb(77, 59, 130, 246);

        /// <summary>Primary Blue 40 % opacity – slider track fill.</summary>
        public static readonly Color Primary40 = Color.FromArgb(102, 59, 130, 246);

        /// <summary>Primary Blue 50 % opacity – mid-emphasis accents.</summary>
        public static readonly Color Primary50 = Color.FromArgb(128, 59, 130, 246);

        /// <summary>Primary Blue 60 % opacity – day-of-week chip fill.</summary>
        public static readonly Color Primary60 = Color.FromArgb(153, 59, 130, 246);

        /// <summary>Primary Blue 80 % opacity – high-emphasis tinted areas.</summary>
        public static readonly Color Primary80 = Color.FromArgb(204, 59, 130, 246);


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  BACKGROUND LAYERS (dark → light)                              ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Deepest application background (#121212).</summary>
        public static readonly Color Background = ColorTranslator.FromHtml("#121212");

        /// <summary>Cards, panels, sidebars (#1A1A1A).</summary>
        public static readonly Color Surface = ColorTranslator.FromHtml("#1A1A1A");

        /// <summary>Hover state / elevated surface (#252525).</summary>
        public static readonly Color Surface2 = ColorTranslator.FromHtml("#252525");

        /// <summary>Input fields, code blocks, summary panels (#2D2D2D).</summary>
        public static readonly Color Surface3 = ColorTranslator.FromHtml("#2D2D2D");

        /// <summary>Darkest surface – e.g. inner card or sidebar deep BG (#0A0A0A).</summary>
        public static readonly Color SurfaceDeep = ColorTranslator.FromHtml("#0A0A0A");


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  SEMANTIC / STATUS COLORS                                      ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Success green – check icons, "Active" label (#2ECC71).</summary>
        public static readonly Color Success = ColorTranslator.FromHtml("#2ECC71");

        /// <summary>Success green 50 % opacity – success glow / shadow.</summary>
        public static readonly Color SuccessGlow = Color.FromArgb(128, 34, 197, 94);

        /// <summary>Warning amber – caution indicators (#F39C12).</summary>
        public static readonly Color Warning = ColorTranslator.FromHtml("#F39C12");

        /// <summary>Destructive red – delete, stop, error (#EF4444).</summary>
        public static readonly Color Danger = ColorTranslator.FromHtml("#EF4444");

        /// <summary>Danger hover – delete button hover tint (10 % red).</summary>
        public static readonly Color DangerHover = Color.FromArgb(26, 239, 68, 68);

        /// <summary>Danger muted – outlined danger button background (#4D1F1F).</summary>
        public static readonly Color DangerMuted = ColorTranslator.FromHtml("#4D1F1F");

        /// <summary>Purple accent – routine card icons, brand highlights (#8B5CF6).</summary>
        public static readonly Color Purple = ColorTranslator.FromHtml("#8B5CF6");


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  TEXT COLORS                                                    ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Headings, primary labels – pure white (#FFFFFF).</summary>
        public static readonly Color TextPrimary = ColorTranslator.FromHtml("#FFFFFF");

        /// <summary>Dark text for light surfaces (#1F1F1F).</summary>
        public static readonly Color TextDark = ColorTranslator.FromHtml("#1F1F1F");

        /// <summary>Subtitles, secondary descriptions (#9CA3AF).</summary>
        public static readonly Color TextSecondary = ColorTranslator.FromHtml("#9CA3AF");

        /// <summary>Disabled text, placeholders, muted hints (#6B7280).</summary>
        public static readonly Color TextMuted = ColorTranslator.FromHtml("#6B7280");

        /// <summary>Clickable text – same as Primary for consistency.</summary>
        public static readonly Color TextLink = ColorTranslator.FromHtml("#3B82F6");


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  BORDERS & OPACITY                                             ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Default subtle border (#2A2A2A).</summary>
        public static readonly Color Border = ColorTranslator.FromHtml("#2A2A2A");

        /// <summary>Hover-state border – uses Primary Blue.</summary>
        public static readonly Color BorderHover = ColorTranslator.FromHtml("#3B82F6");

        /// <summary>Focused / active border – brighter blue.</summary>
        public static readonly Color BorderFocus = ColorTranslator.FromHtml("#60A5FA");

        /// <summary>Light border for light-mode surfaces (#CCCCCC).</summary>
        public static readonly Color BorderLight = ColorTranslator.FromHtml("#CCCCCC");

        /// <summary>Black 10 % – subtle dividers on dark surfaces.</summary>
        public static readonly Color BorderBlack10 = Color.FromArgb(26, 0, 0, 0);

        /// <summary>White 10 % – subtle dividers on dark surfaces.</summary>
        public static readonly Color BorderWhite10 = Color.FromArgb(26, 255, 255, 255);

        /// <summary>White 5 % – ultra-subtle separator.</summary>
        public static readonly Color White5 = Color.FromArgb(13, 255, 255, 255);

        /// <summary>Black 5 % – ultra-subtle shadow tint.</summary>
        public static readonly Color Black5 = Color.FromArgb(13, 0, 0, 0);

        /// <summary>Black 3 % – lightest shadow layer.</summary>
        public static readonly Color Black3 = Color.FromArgb(8, 0, 0, 0);


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  SIDEBAR                                                       ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Sidebar deep background (#0D0D0D).</summary>
        public static readonly Color SidebarBg = ColorTranslator.FromHtml("#0D0D0D");

        /// <summary>Sidebar default item background (#1A1A1A).</summary>
        public static readonly Color SidebarItemBg = ColorTranslator.FromHtml("#1A1A1A");

        /// <summary>Sidebar active/selected item – uses Primary Blue.</summary>
        public static readonly Color SidebarActive = ColorTranslator.FromHtml("#3B82F6");

        /// <summary>Sidebar item hover background (#222222).</summary>
        public static readonly Color SidebarHover = ColorTranslator.FromHtml("#222222");

        /// <summary>Sidebar accent for light mode (#E5E5E5).</summary>
        public static readonly Color SidebarAccentLight = ColorTranslator.FromHtml("#E5E5E5");


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  MISCELLANEOUS COLORS                                          ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Neutral accent for light-mode elements (#E9EBEF).</summary>
        public static readonly Color Accent = ColorTranslator.FromHtml("#E9EBEF");

        /// <summary>Light background for light-mode panels (#F5F5F5).</summary>
        public static readonly Color LightBackground = ColorTranslator.FromHtml("#F5F5F5");

        /// <summary>Input field background (light mode) (#F3F3F5).</summary>
        public static readonly Color InputBackgroundLight = ColorTranslator.FromHtml("#F3F3F5");

        /// <summary>Switch/toggle unchecked background (#CBCED4).</summary>
        public static readonly Color SwitchBackground = ColorTranslator.FromHtml("#CBCED4");

        /// <summary>Console "system" text green (#22C55E).</summary>
        public static readonly Color ConsoleGreen = ColorTranslator.FromHtml("#22C55E");

        /// <summary>Console terminal dots – red (#EF4444).</summary>
        public static readonly Color ConsoleDotRed = ColorTranslator.FromHtml("#EF4444");

        /// <summary>Console terminal dots – yellow (#EAB308).</summary>
        public static readonly Color ConsoleDotYellow = ColorTranslator.FromHtml("#EAB308");

        /// <summary>Console terminal dots – green (#22C55E).</summary>
        public static readonly Color ConsoleDotGreen = ColorTranslator.FromHtml("#22C55E");

        /// <summary>"Finish &amp; Save" gradient start – green (#22C55E).</summary>
        public static readonly Color GradientGreenStart = ColorTranslator.FromHtml("#22C55E");

        /// <summary>"Finish &amp; Save" gradient end – teal (#06B6D4).</summary>
        public static readonly Color GradientGreenEnd = ColorTranslator.FromHtml("#06B6D4");


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  CHART / DATA-VISUALIZATION COLORS                             ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Chart series 1 – warm orange.</summary>
        public static readonly Color Chart1 = Color.FromArgb(217, 119, 68);

        /// <summary>Chart series 2 – teal.</summary>
        public static readonly Color Chart2 = Color.FromArgb(91, 168, 156);

        /// <summary>Chart series 3 – dark blue-gray.</summary>
        public static readonly Color Chart3 = Color.FromArgb(62, 86, 103);

        /// <summary>Chart series 4 – golden yellow.</summary>
        public static readonly Color Chart4 = Color.FromArgb(224, 192, 70);

        /// <summary>Chart series 5 – amber.</summary>
        public static readonly Color Chart5 = Color.FromArgb(210, 168, 62);


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  STAT CARD ACCENT COLORS (Dashboard top row)                   ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Total Routines card accent – Primary Blue.</summary>
        public static readonly Color StatCardBlue = ColorTranslator.FromHtml("#3B82F6");

        /// <summary>Active Routines card accent – green.</summary>
        public static readonly Color StatCardGreen = ColorTranslator.FromHtml("#22C55E");

        /// <summary>Running Now card accent – purple.</summary>
        public static readonly Color StatCardPurple = ColorTranslator.FromHtml("#8B5CF6");

        /// <summary>Total Actions card accent – red/orange.</summary>
        public static readonly Color StatCardRed = ColorTranslator.FromHtml("#EF4444");


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  TYPOGRAPHY – Font Sizes in pt                                 ║
        // ║                                                                ║
        // ║  Scale from allColors.html design tokens:                      ║
        // ║    xs = 9pt   sm = 10pt   base = 12pt   md = 14pt             ║
        // ║    lg = 18pt  xl = 24pt   2xl  = 32pt                         ║
        // ╚══════════════════════════════════════════════════════════════════╝

        private const string FontFamily = "Segoe UI";
        private const string FontFamilyMono = "Consolas";

        // ── Page-level headings (xl = 24pt) ──
        /// <summary>Page title – "Dashboard", "Create New Routine" (24pt Bold).</summary>
        public static readonly Font FontPageTitle = new Font(FontFamily, 24f, FontStyle.Bold);

        // ── Section headings (lg = 18pt) ──
        /// <summary>Section title – "Your Routines", "Action Pipeline" (18pt Bold).</summary>
        public static readonly Font FontHeader = new Font(FontFamily, 18f, FontStyle.Bold);

        // ── Sub-section headings (md = 14pt) ──
        /// <summary>Card title, dialog sub-header – "Morning Setup" (14pt Bold).</summary>
        public static readonly Font FontSubheader = new Font(FontFamily, 14f, FontStyle.Bold);

        // ── Body text (base = 12pt) ──
        /// <summary>Default body text – descriptions, labels (12pt Regular).</summary>
        public static readonly Font FontBody = new Font(FontFamily, 12f, FontStyle.Regular);

        /// <summary>Bold body – routine name in log items (12pt Bold).</summary>
        public static readonly Font FontBodyBold = new Font(FontFamily, 12f, FontStyle.Bold);

        // ── Small text (sm = 10pt) ──
        /// <summary>Small labels – schedule info, action counts (10pt Regular).</summary>
        public static readonly Font FontSmall = new Font(FontFamily, 10f, FontStyle.Regular);

        /// <summary>Small bold – chip labels, day-of-week letters (10pt Bold).</summary>
        public static readonly Font FontSmallBold = new Font(FontFamily, 10f, FontStyle.Bold);

        // ── Caption text (xs = 9pt) ──
        /// <summary>Captions – timestamps, secondary metadata (9pt Regular).</summary>
        public static readonly Font FontCaption = new Font(FontFamily, 9f, FontStyle.Regular);

        // ── Monospace (base = 12pt) ──
        /// <summary>Console output, code/command text (12pt Consolas).</summary>
        public static readonly Font FontMono = new Font(FontFamilyMono, 12f, FontStyle.Regular);

        /// <summary>Small monospace – log summary text (9pt Consolas).</summary>
        public static readonly Font FontMonoSmall = new Font(FontFamilyMono, 9f, FontStyle.Regular);

        // ── Extra-large display (2xl = 32pt) ──
        /// <summary>Stat card large numbers – "4", "3", "1", "5" (32pt Bold).</summary>
        public static readonly Font FontDisplay = new Font(FontFamily, 32f, FontStyle.Bold);


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  DIMENSIONS – Structural layout sizes (px)                     ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Left navigation panel width (250px).</summary>
        public const int SidebarWidth = 250;

        /// <summary>Top header bar height (64px).</summary>
        public const int HeaderHeight = 64;

        /// <summary>Maximum content area width (1200px).</summary>
        public const int ContainerMaxWidth = 1200;

        /// <summary>Modal/dialog default width (400px).</summary>
        public const int ModalWidth = 400;

        /// <summary>Standard text input height (40px).</summary>
        public const int InputHeight = 40;

        /// <summary>Standard button height (40px).</summary>
        public const int ButtonHeight = 40;


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  BORDER RADIUS                                                 ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>No rounding (0px).</summary>
        public const int RadiusNone = 0;

        /// <summary>Small radius – chips, tags, inner elements (4px).</summary>
        public const int RadiusSmall = 4;

        /// <summary>Medium radius – buttons, inputs (8px).</summary>
        public const int RadiusMedium = 8;

        /// <summary>Default card radius (12px).</summary>
        public const int RadiusDefault = 12;

        /// <summary>Large radius – modals, big cards (16px).</summary>
        public const int RadiusLarge = 16;

        /// <summary>Fully round – avatars, toggle switches, circle buttons (9999px).</summary>
        public const int RadiusCircle = 9999;


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  SPACING – Padding (px)                                        ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Extra-small padding (4px).</summary>
        public const int PaddingXs = 4;

        /// <summary>Small padding (8px).</summary>
        public const int PaddingSm = 8;

        /// <summary>Medium / default padding (16px).</summary>
        public const int PaddingMd = 16;

        /// <summary>Large padding (24px).</summary>
        public const int PaddingLg = 24;

        /// <summary>Extra-large padding (32px).</summary>
        public const int PaddingXl = 32;

        /// <summary>2× extra-large padding (48px).</summary>
        public const int Padding2Xl = 48;


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  SPACING – Margins (px)                                        ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Extra-small margin (4px).</summary>
        public const int MarginXs = 4;

        /// <summary>Small margin (8px).</summary>
        public const int MarginSm = 8;

        /// <summary>Medium / default margin (16px).</summary>
        public const int MarginMd = 16;

        /// <summary>Large margin (24px).</summary>
        public const int MarginLg = 24;

        /// <summary>Extra-large margin (32px).</summary>
        public const int MarginXl = 32;

        /// <summary>2× extra-large margin (48px).</summary>
        public const int Margin2Xl = 48;


        // ╔══════════════════════════════════════════════════════════════════╗
        // ║  LEGACY ALIASES                                                ║
        // ║  Kept for backward–compatibility with existing controls.       ║
        // ║  New code should use the canonical names above.                ║
        // ╚══════════════════════════════════════════════════════════════════╝

        /// <summary>Alias → <see cref="PaddingMd"/> (16px).</summary>
        public const int CardPadding = PaddingMd;
    }
}
