using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Backoffice.Notifications.Cleanup;
using UrlTracker.Backoffice.Notifications.Content;
using UrlTracker.Backoffice.Notifications.Options;
using UrlTracker.Modules.Options;

namespace UrlTracker.Backoffice.Notifications
{
    /// <summary>
    /// The entry point for the URL Tracker core services
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EntryPoint
    {
        /// <summary>
        /// Add content notification handlers for the URL Tracker to the dependency injection container
        /// </summary>
        /// <remarks>
        /// <para><see cref="Core.EntryPoint.ComposeUrlTrackerCore(IUmbracoBuilder)"/> must be used before invoking this method</para>
        /// </remarks>
        /// <param name="builder">The umbraco dependency collection builder</param>
        /// <returns>The umbraco dependency collection builder after all services are added</returns>
        public static IUmbracoBuilder ComposeUrlTrackerBackofficeNotifications(this IUmbracoBuilder builder)
        {
            builder.ComposeContentHandling();
            builder.ComposeCleanupHandling();
            builder.ComposeConfigurations();

            builder.Services.AddUrlTrackerModule("Backoffice notifications");

            return builder;
        }

        private static IUmbracoBuilder ComposeCleanupHandling(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<CleanupQueue>();
            builder.Services.AddSingleton<ICleanupProcessor, CleanupProcessor>();
            builder.Services.AddHostedService<CleanupWorker>();
            builder.Services.AddHostedService<CleanupScheduler>();

            return builder;
        }

        private static IUmbracoBuilder ComposeContentHandling(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IContentValueReaderFactory, ContentValueReaderFactory>();

            builder.AddNotificationAsyncHandler<ContentMovingNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationAsyncHandler<ContentMovedNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationAsyncHandler<ContentPublishingNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationAsyncHandler<ContentPublishedNotification, ContentChangeNotificationHandler>();

            return builder;
        }

        private static IUmbracoBuilder ComposeConfigurations(this IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<UrlTrackerNotificationsOptions>()
                            .Bind(builder.Config.GetSection(Defaults.Options.Section))
                            .ValidateDataAnnotations();

            return builder;
        }
    }
}
