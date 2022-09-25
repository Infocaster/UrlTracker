using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Backoffice.UI.Compatibility;
using UrlTracker.Backoffice.UI.Map;
using UrlTracker.Web.Events;

namespace UrlTracker.Backoffice.UI
{
    /// <summary>
    /// The entry point for the URL Tracker backoffice interface
    /// </summary>
    public static class EntryPoint
    {
        /// <summary>
        /// Add a management interface in the Umbraco backoffice for the URL Tracker
        /// </summary>
        /// <remarks>
        /// <para><see cref="Core.EntryPoint.ComposeUrlTrackerCore(IUmbracoBuilder)"/> must be used before invoking this method</para>
        /// </remarks>
        /// <param name="builder">The umbraco dependency collection builder</param>
        /// <returns>The umbraco dependency collection builder after all services are added</returns>
        public static IUmbracoBuilder ComposeUrlTrackerBackoffice(this IUmbracoBuilder builder)
        {
            builder.AddDashboard<UrlTrackerDashboard>();
            builder.ManifestFilters()
                .Append<UrlTrackerManifestFilter>();
            builder.BackOfficeAssets()
                .Append<UrlTrackerScript>()
                .Append<UrlTrackerStyle>();

            builder.MapDefinitions()
                .Add<ResponseMap>()
                .Add<RequestMap>()
                .Add<CsvMap>();

            builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesNotificationHandler>();

            builder.Services.AddSingleton<IRequestModelPatcher, RequestModelPatcher>();

            return builder;
        }
    }
}
