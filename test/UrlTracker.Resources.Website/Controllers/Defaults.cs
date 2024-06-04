namespace UrlTracker.Resources.Website.Controllers
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
            /// Routing details for API version 1.
            /// </summary>
            public static partial class V1
            {
                /// <summary>
                /// The unique name for the URL Tracker api version 1. Used in swagger
                /// </summary>
                public const string ApiName = "url-tracker-test-v1";

                /// <summary>
                /// The version string for the URL Tracker api version 1. Used in swagger and in the route
                /// </summary>
                public const string ApiVersion = "1.0";

                /// <summary>
                /// The base route for any API controller of the URL Tracker version 1.
                /// </summary>
                public const string Route = "api/v{version:apiVersion}/" + Area + "/[controller]";
            }

            /// <summary>
            /// The controller area. Adds an area segment to endpoint urls
            /// </summary>
            public const string Area = "UrlTrackerTest";
        }
    }
}
