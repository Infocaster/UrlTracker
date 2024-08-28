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
            /// The controller area. Adds an area segment to endpoint urls
            /// </summary>
            public const string Route = "Umbraco/Backoffice/"+Area+"/[controller]";

            /// <summary>
            /// The base folder for all static resources
            /// </summary>
            public const string AppPluginFolder = "/app_plugins/urltracker/";

            /// <summary>
            /// The base folder for all dashboard pages
            /// </summary>
            public const string DashboardPageFolder = AppPluginFolder + "dashboard/tabs/";
        }
    }
}
