namespace UrlTracker.Middleware
{
    /// <inheritdoc cref="Core.Defaults" />
    public static class Defaults
    {
        /// <inheritdoc cref="Core.Defaults.Options" />
        public static class Options
        {
            /// <summary>
            /// The configuration section where the options for middleware are stored
            /// </summary>
            public const string Section = Core.Defaults.Options.UrlTrackerSection + ":Pipeline";
        }
    }
}
