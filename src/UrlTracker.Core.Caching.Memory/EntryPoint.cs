using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Core.Caching.Memory.Active;
using UrlTracker.Core.Caching.Memory.Database;
using UrlTracker.Core.Caching.Memory.Intercepting;
using UrlTracker.Core.Caching.Memory.Notifications;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Database;
using UrlTracker.Core.Intercepting;
using UrlTracker.Modules.Options;

namespace UrlTracker.Core.Caching.Memory
{
    /// <summary>
    /// The entry point for the URL Tracker in-memory cache services
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EntryPoint
    {
        /// <summary>
        /// Add all in-memory cache services of the URL Tracker to the dependency injection container
        /// </summary>
        /// <remarks>
        /// <para><see cref="Core.EntryPoint.ComposeUrlTrackerCore(IUmbracoBuilder)"/> must be used before invoking this method</para>
        /// </remarks>
        /// <param name="builder">The umbraco dependency collection builder</param>
        /// <returns>The umbraco dependency collection builder after all URL Tracker in-memory cache services are added</returns>
        public static IUmbracoBuilder ComposeUrlTrackerMemoryCache(this IUmbracoBuilder builder)
        {
            var settings = builder.Config.GetOptions();
            if (settings.EnableInterceptCaching)
            {
                builder.Services.Decorate<IIntermediateInterceptService>((decoratee, factory) => new DecoratorIntermediateInterceptServiceCaching(decoratee, factory.GetRequiredService<IInterceptCache>(), factory.GetRequiredService<IOptions<UrlTrackerMemoryCacheOptions>>()));
            }
            if (settings.EnableRegexRedirectCaching)
            {
                builder.Services.Decorate<IRedirectRepository>((decoratee, factory)
                    => new DecoratorRedirectRepositoryCaching(
                        decoratee,
                        factory.GetRequiredService<IMemoryCache>(),
                        factory.GetRequiredService<DistributedCache>(),
                        factory.GetRequiredService<IActiveCacheAccessor>(),
                        factory.GetRequiredService<IOptions<UrlTrackerMemoryCacheOptions>>()));
            }

            builder.Services.Decorate<IClientErrorRepository, DecoratorClientErrorRepositoryCaching>();

            builder.Services.AddSingleton<IInterceptCache, InterceptCache>();
            builder.Services.AddSingleton<IActiveCacheAccessor, ActiveCacheAccessor>();
            builder.Services.AddSingleton<IActiveRedirectCacheWriter, ActiveRedirectCacheWriter>();
            builder.Services.AddSingleton<IActiveClientErrorCacheWriter, ActiveClientErrorCacheWriter>();
            builder.AddComponent<ActiveCacheBootloader>();

            builder.ComposeNotificationHandlers()
                .ComposeConfigurations();

            builder.Services.AddUrlTrackerModule("Core in-memory cache");

            return builder;
        }

        private static UrlTrackerMemoryCacheOptions GetOptions(this IConfiguration configuration)
        {
            var settings = new UrlTrackerMemoryCacheOptions();
            configuration.GetSection(Defaults.Options.Section).Bind(settings);
            return settings;
        }

        private static IUmbracoBuilder ComposeNotificationHandlers(this IUmbracoBuilder builder)
        {
            builder.CacheRefreshers().Add<RedirectsCacheRefresher>();
            builder.AddNotificationHandler<RedirectsChangedNotification, RedirectsChangedNotificationHandler>();
            builder.AddNotificationHandler<RedirectsChangedNotification, ActiveCacheRedirectChangeHandler>();
            builder.AddNotificationHandler<DomainDeletedNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationHandler<DomainSavedNotification, ContentChangeNotificationHandler>();
            return builder;
        }

        private static IUmbracoBuilder ComposeConfigurations(this IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<UrlTrackerMemoryCacheOptions>()
                            .Bind(builder.Config.GetSection(Defaults.Options.Section))
                            .ValidateDataAnnotations();
            return builder;
        }
    }
}
