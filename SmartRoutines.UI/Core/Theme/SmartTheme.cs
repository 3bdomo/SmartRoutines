namespace SmartRoutines.UI.Core.Theme
{
    public static class SmartTheme
    {
        // ── Background Layers ───────────────────────────────────────── 
        public static Color Background = ColorTranslator.FromHtml("#121212"); // Deepest bg
        public static Color Surface = ColorTranslator.FromHtml("#1E1E1E"); // Cards, panels
        public static Color Surface2 = ColorTranslator.FromHtml("#252525"); // Hover state
        public static Color Surface3 = ColorTranslator.FromHtml("#2D2D2D"); // Input fields

        // ── Borders ─────────────────────────────────────────────────── 
        public static Color Border = ColorTranslator.FromHtml("#2A2A2A"); // Default border
        public static Color BorderHover = ColorTranslator.FromHtml("#0078D4"); // Hover border
        public static Color BorderFocus = ColorTranslator.FromHtml("#0099FF"); // Focused border

        // ── Accent Colors ───────────────────────────────────────────── 
        public static Color Primary = ColorTranslator.FromHtml("#0078D4"); // Microsoft Blue
        public static Color PrimaryHover = ColorTranslator.FromHtml("#006CBE"); // Darkened blue
        public static Color Success = ColorTranslator.FromHtml("#2ECC71"); // Green 
        public static Color Warning = ColorTranslator.FromHtml("#F39C12"); // Amber 
        public static Color Danger = ColorTranslator.FromHtml("#E74C3C"); // Red 
        public static Color Purple = ColorTranslator.FromHtml("#8B5CF6"); // Purple accent
        // ── Text Colors ─────────────────────────────────────────────── 
        public static Color TextPrimary = ColorTranslator.FromHtml("#FFFFFF"); // Headings
        public static Color TextSecondary = ColorTranslator.FromHtml("#A0A0A0"); // Subtitles
        public static Color TextMuted = ColorTranslator.FromHtml("#606060"); // Disabled/hints
        public static Color TextLink = ColorTranslator.FromHtml("#0078D4"); // Clickable text

        // ── Typography (Font sizes in pt) ───────────────────────────── 
        public static Font FontHeader = new Font("Segoe UI", 16f, FontStyle.Bold);
        public static Font FontSubheader = new Font("Segoe UI", 12f, FontStyle.Bold);
        public static Font FontBody = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static Font FontBodyBold = new Font("Segoe UI", 9f, FontStyle.Bold);
        public static Font FontCaption = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static Font FontMono = new Font("Consolas", 9f, FontStyle.Regular);

        // ── Dimensions ──────────────────────────────────────────────── 
        public const int RadiusDefault = 12;  // Standard card radius 
        public const int RadiusSmall = 8;  // Small elements (chips, tags) 
        public const int RadiusLarge = 16;  // Large cards, modals 
        public const int RadiusCircle = 50;  // Fully round (avatars, toggle) 
        public const int SidebarWidth = 220; // Left navigation width 
        public const int CardPadding = 16;  // Internal card padding 

        // ── Sidebar Items ───────────────────────────────────────────── 
        public static Color SidebarBg = ColorTranslator.FromHtml("#0D0D0D");
        public static Color SidebarItemBg = ColorTranslator.FromHtml("#1A1A1A");
        public static Color SidebarActive = ColorTranslator.FromHtml("#0078D4");
        public static Color SidebarHover = ColorTranslator.FromHtml("#222222");
    }
}
