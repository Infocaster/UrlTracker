using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Mappers;
using UrlTracker.Core.Database.Migrations;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Preprocessing;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Core.Validation;
using UrlTracker.Modules.Options;

namespace UrlTracker.Core
{
    /// <summary>
    /// The entry point for the URL Tracker core services
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EntryPoint
    {
        /// <summary>
        /// Add all core services of the URL Tracker to the dependency injection container
        /// </summary>
        /// <param name="builder">The umbraco dependency collection builder</param>
        /// <returns>The umbraco dependency collection builder after all URL Tracker core services are added</returns>
        public static IUmbracoBuilder ComposeUrlTrackerCore(this IUmbracoBuilder builder)
        {
            builder.ComposeUrlTrackerCoreComponents()
                   .ComposeUrlTrackerCoreConfigurations()
                   .ComposeUrlTrackerCoreMaps()
                   .ComposeUrlTrackerCoreAbstractions()
                   .ComposeDefaultInterceptors()
                   .ComposeDefaultInterceptConverters()
                   .ComposeDefaultUrlClassifiers()
                   .ComposeDefaultStrategyMaps();

            builder.Services.AddSingleton<IDefaultInterceptContextFactory, DefaultInterceptContextFactory>();
            builder.Services.AddSingleton<IIntermediateInterceptService, IntermediateInterceptService>();
            builder.Services.AddSingleton<IInterceptService, InterceptService>();
            builder.Services.AddSingleton<IRedirectService, RedirectService>();
            builder.Services.AddSingleton<IClientErrorService, ClientErrorService>();
            builder.Services.AddSingleton<IRedactionScoreService, RedactionScoreService>();
            builder.Services.AddSingleton<IRecommendationService, RecommendationService>();
            builder.Services.AddSingleton<IRedirectRepository, RedirectRepository>();
            builder.Services.AddSingleton<IReferrerRepository, ReferrerRepository>();
            builder.Services.AddSingleton<IRedactionScoreRepository, RedactionScoreRepository>();
            builder.Services.AddSingleton<IRecommendationRepository, RecommendationRepository>();

            builder.Services.AddSingleton<IClientErrorRepository, ClientErrorRepository>();
            builder.Services.AddSingleton<IValidationHelper, ValidationHelper>();
            builder.Services.AddSingleton<IMigrationPlanFactory, MigrationPlanFactory>();

            builder.Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));

            builder.Services.AddSingleton<IStrategyMapCollection>(factory => factory.GetRequiredService<StrategyMapCollection>());
            builder.Services.AddSingleton<IStaticUrlProviderCollection>(factory => factory.GetRequiredService<StaticUrlProviderCollection>());
            builder.Services.AddSingleton<IInterceptorCollection>(factory => factory.GetRequiredService<InterceptorCollection>());
            builder.Services.AddSingleton<IInterceptConverterCollection>(factory => factory.GetRequiredService<InterceptConverterCollection>());
            builder.Services.AddSingleton<IUrlClassifierStrategyCollection>(factory => factory.GetRequiredService<UrlClassifierStrategyCollection>());

            builder.Services.AddUrlTrackerModule("Core services");

            return builder;
        }

        public static InterceptorCollectionBuilder? Interceptors(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<InterceptorCollectionBuilder>();

        public static StaticUrlProviderCollectionBuilder? StaticUrlProviders(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<StaticUrlProviderCollectionBuilder>();

        public static InterceptConverterCollectionBuilder? InterceptConverters(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<InterceptConverterCollectionBuilder>();

        public static StrategyMapCollectionBuilder? StrategyMaps(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<StrategyMapCollectionBuilder>();

        public static UrlClassifierStrategyCollectionBuilder? UrlClassifiers(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<UrlClassifierStrategyCollectionBuilder>();

        public static IUmbracoBuilder ComposeDefaultStrategyMaps(this IUmbracoBuilder builder)
        {
            builder.StrategyMaps()!
                .Append<UrlSourceStrategyMap>()
                .Append<RegexSourceStrategyMap>()
                .Append<UrlTargetStrategyMap>()
                .Append<ContentPageTargetStrategyMap>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultInterceptors(this IUmbracoBuilder builder)
        {
            builder.Interceptors()!
                .Append<StaticUrlRedirectInterceptor>()
                .Append<RegexRedirectInterceptor>()
                .Append<NoLongerExistsInterceptor>();

            builder.Services.AddSingleton<ILastChanceInterceptor, NullInterceptor>();

            builder.StaticUrlProviders()!
                .Append<StaticUrlProvider>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultUrlClassifiers(this IUmbracoBuilder builder)
        {
            builder.UrlClassifiers()!
                .Append<MediaUrlClassifierStrategy>()
                .Append<TechnicalFileUrlClassifierStrategy>()
                .Append<FileUrlClassifierStrategy>();

            builder.Services.AddSingleton<IFallbackUrlClassifier, FallbackUrlClassifier>();

            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultInterceptConverters(this IUmbracoBuilder builder)
        {
            builder.InterceptConverters()!
                .Append<MapperInterceptConverter<IRedirect, Redirect>>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreComponents(this IUmbracoBuilder builder)
        {
            builder.AddComponent<MigrationComponent>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreConfigurations(this IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<UrlTrackerSettings>()
                            .Bind(builder.Config.GetSection(Defaults.Options.UrlTrackerSection))
                            .ValidateDataAnnotations();

            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreMaps(this IUmbracoBuilder builder)
        {
            // business logic mappers
            builder.MapDefinitions()!
                .Add<ServiceLayerMaps>();

            // sql entity mappers
            builder.Mappers()!
                .Add<RedirectMapper>()
                .Add<ClientErrorMapper>()
                .Add<ReferrerMapper>()
                .Add<RecommendationMapper>()
                .Add<RedactionScoreMapper>();

            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreAbstractions(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IUmbracoContextFactoryAbstraction, UmbracoContextFactoryAbstraction>();
            return builder;
        }
    }
}
