using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Compatibility;
using UrlTracker.Backoffice.UI.Controllers;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;
using UrlTracker.Backoffice.UI.Extensions;
using UrlTracker.Backoffice.UI.Map;
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
            builder.AddDefaultUrlTrackerDashboardPages();

            builder.ManifestFilters()
                .Append<UrlTrackerManifestFilter>();
            builder.BackOfficeAssets()
                .Append<UrlTrackerScript>()
                .Append<UrlTrackerStyle>();

            builder.MapDefinitions()
                .Add<CsvMap>()
                .Add<RedirectMap>()
                .Add<RecommendationMap>()
                .Add<ExtensionMap>();

            builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesNotificationHandler>();

            builder.Services.AddSingleton<IRequestModelPatcher, RequestModelPatcher>();
            builder.Services.AddScoped<IRedirectRequestHandler, RedirectRequestHandler>();
            builder.Services.AddScoped<IRecommendationRequestHandler, RecommendationRequestHandler>();

            builder.AddMvcAndRazor(options =>
            {
                options.ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new UrlTrackerControllerFeatureProvider());
                });
            });

            return builder;
        }

        /// <summary>
        /// Add or change pages on the URL Tracker dashboard
        /// </summary>
        /// <param name="builder">The Umbraco service collection</param>
        /// <returns>A builder for URL Tracker dashboard pages</returns>
        public static UrlTrackerDashboardPageCollectionBuilder UrlTrackerDashboardPages(this IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IUrlTrackerDashboardPageCollection>(sp => sp.GetRequiredService<UrlTrackerDashboardPageCollection>());
            return builder.WithCollectionBuilder<UrlTrackerDashboardPageCollectionBuilder>();
        }

        /// <summary>
        /// Composes the default URL Tracker dashboard pages
        /// </summary>
        /// <param name="builder">The Umbraco service collection</param>
        /// <returns>The Umbraco service collection after all the services are added</returns>
        public static IUmbracoBuilder AddDefaultUrlTrackerDashboardPages(this IUmbracoBuilder builder)
        {
            builder.UrlTrackerDashboardPages()
                .Add<OverviewDashboardPage>()
                .Add<RecommendationsDashboardPage>()
                .Add<RedirectsDashboardPage>()
                .Add<AdvancedRedirectsDashboardPage>();

            return builder;
        }
    }
}
