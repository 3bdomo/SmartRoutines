namespace SmartRoutines.Core.Models
{
    /// <summary>
    /// Represents the global configuration and user preferences for the application.
    /// </summary>
    /// <remarks>
    /// In Entity Framework Core, this entity is typically mapped to a table where only a single 
    /// row exists (e.g., Id = 1). It inherits from <see cref="BaseEntity"/> to maintain 
    /// audit trails (CreatedAt, UpdatedAt) whenever the user changes their settings.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Retrieving settings (usually via a Repository)
    /// var settings = settingsRepository.Get();
    /// 
    /// // Updating user preferences from the UI
    /// settings.UpdatePreferences(runAtStartup: true, minimizeToTray: true, theme: "Light");
    /// repository.Update(settings);
    /// </code>
    /// </example>
    public class AppSettings : BaseEntity
    {
        /// <summary>
        /// Gets a value indicating whether the application should launch automatically 
        /// when the Windows operating system starts.
        /// </summary>
        /// <value><c>true</c> if the app runs at startup; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool RunAtStartup { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the application should hide to the system tray 
        /// instead of closing when the user clicks the 'X' button.
        /// </summary>
        /// <value><c>true</c> to minimize to tray; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool MinimizeToTray { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the system should display toast notifications 
        /// when a routine executes successfully or fails.
        /// </summary>
        /// <value><c>true</c> to enable notifications; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowNotifications { get; private set; }

        /// <summary>
        /// Gets the visual theme applied to the application's user interface.
        /// </summary>
        /// <value>A string representing the theme name (e.g., "Dark", "Light"). Default is "Dark".</value>
        //[Required]
        //[MaxLength(50)]
        public string Theme { get; private set; }

        /// <summary>
        /// Gets the default language code for the application interface (e.g., "en-US", "ar-EG").
        /// </summary>
        /// <value>A 5-character string representing the culture code.</value>
        //[Required]
        //[MaxLength(10)]
        public string DefaultLanguage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class with default values.
        /// </summary>
        public AppSettings()
        {
            RunAtStartup = true;
            MinimizeToTray = true;
            ShowNotifications = true;
            Theme = "Dark";
            DefaultLanguage = "en-US";
        }

        /// <summary>
        /// Updates the global application preferences securely.
        /// </summary>
        /// <param name="runAtStartup">Flag to run at Windows startup.</param>
        /// <param name="minimizeToTray">Flag to hide to system tray.</param>
        /// <param name="showNotifications">Flag to enable system notifications.</param>
        /// <param name="theme">The UI theme name.</param>
        /// <param name="defaultLanguage">The culture code for localization.</param>
        /// <exception cref="ArgumentException">Thrown if the theme or language parameters are null or whitespace.</exception>
        public void UpdatePreferences(bool runAtStartup, bool minimizeToTray, bool showNotifications, string theme, string defaultLanguage)
        {
            if (string.IsNullOrWhiteSpace(theme))
                throw new ArgumentException("Theme cannot be null or empty.", nameof(theme));

            if (string.IsNullOrWhiteSpace(defaultLanguage))
                throw new ArgumentException("Language code cannot be null or empty.", nameof(defaultLanguage));

            RunAtStartup = runAtStartup;
            MinimizeToTray = minimizeToTray;
            ShowNotifications = showNotifications;
            Theme = theme;
            DefaultLanguage = defaultLanguage;

            // Update the audit timestamp inherited from BaseEntity
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
