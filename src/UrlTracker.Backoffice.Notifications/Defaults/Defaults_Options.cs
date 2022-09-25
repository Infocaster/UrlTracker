namespace UrlTracker.Backoffice.Notifications
{
    /// <inheritdoc cref="Core.Defaults" />
    public static partial class Defaults
    {
        /// <inheritdoc cref="Core.Defaults.Options" />
        public static partial class Options
        {
            /// <summary>
            /// The configuration section where the options for backoffice notifications are stored
            /// </summary>
            public const string Section = Core.Defaults.Options.UrlTrackerSection + ":Backoffice:Notifications";
        }
    }
}
