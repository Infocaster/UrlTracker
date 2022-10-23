namespace UrlTracker.Backoffice.UI
{
    /// <inheritdoc cref="Core.Defaults" />
    public static partial class Defaults
    {
        /// <summary>
        /// Constants related to mvc and api controllers
        /// </summary>
        public static partial class Routing
        {
            /// <summary>
            /// The controller area. Adds an area segment to endpoint urls
            /// </summary>
            public const string Area = "UrlTracker";

            /// <summary>
            /// The base folder for all static resources
            /// </summary>
            public const string AppPluginFolder = "app_plugins/urltracker/";

            public const string DashboardPageFolder = AppPluginFolder + "dashboard/pages/";
        }
    }
}
