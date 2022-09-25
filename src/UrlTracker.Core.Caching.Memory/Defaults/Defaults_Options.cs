namespace UrlTracker.Core.Caching.Memory
{
    /// <inheritdoc cref="Core.Defaults" />
    public static partial class Defaults
    {
        /// <inheritdoc cref="Core.Defaults.Options" />
        public static partial class Options
        {
            /// <summary>
            /// The configuration section where the options for in-memory caching are stored
            /// </summary>
            public const string Section = Core.Defaults.Options.UrlTrackerSection + ":Caching:Memory";
        }
    }
}
