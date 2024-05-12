using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Controllers;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;
using UrlTracker.Backoffice.UI.Notifications;
using UrlTracker.Backoffice.UI.UserNotifications;
using UrlTracker.Modules.Options;
using UrlTracker.Web.Events;

namespace UrlTracker.Backoffice.UI
{
    /// <summary>
    /// The entry point for the URL Tracker backoffice interface
    /// </summary>
    [ExcludeFromCodeCoverage]
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
            builder.AddDefaultUrlTrackerNotifications();

            builder.ManifestFilters()
                .Append<UrlTrackerManifestFilter>();
            builder.BackOfficeAssets()
                .Append<UrlTrackerScript>();

            builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesNotificationHandler>();
            builder.AddNotificationHandler<ServingRedirectsNotification, PreloadRedirectTargetNotificationHandler>();

            builder.Services.AddSingleton<IUrltrackerVersionProvider, UrltrackerVersionProvider>();
            builder.Services.AddScoped<IRedirectRequestHandler, RedirectRequestHandler>();
            builder.Services.AddScoped<IRedirectTargetRequestHandler, RedirectTargetRequestHandler>();
            builder.Services.AddScoped<IRedirectImportRequestHandler, RedirectImportRequestHandler>();
            builder.Services.AddScoped<IRecommendationRequestHandler, RecommendationRequestHandler>();
            builder.Services.AddScoped<IRecommendationAnalysisRequestHandler, RecommendationAnalysisRequestHandler>();
            builder.Services.AddScoped<IScoringRequestHandler, ScoringRequestHandler>();
            builder.Services.AddScoped<INotificationsRequestHandler, NotificationsRequestHandler>();

            builder.AddMvcAndRazor(options =>
            {
                options.ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new UrlTrackerControllerFeatureProvider());
                });
            });

            builder.Services.AddUrlTrackerModule("Backoffice user interface");

            return builder;
        }

        /// <summary>
        /// Composes the default URL Tracker dashboard notifications
        /// </summary>
        /// <param name="builder">The Umbraco service collection</param>
        /// <returns>The Umbraco service collection after all the services are added</returns>
        public static IUmbracoBuilder AddDefaultUrlTrackerNotifications(this IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<DashboardWelcomeUserNotificationConfiguration>();

            return builder;
        }
    }
}
