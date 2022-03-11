using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Components;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Exceptions;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Preprocessing;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Core.Validation;

namespace UrlTracker.Core
{
    [ExcludeFromCodeCoverage]
    public class CoreComposer
        : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ComposeUrlTrackerCoreComponents()
                   .ComposeUrlTrackerCoreNotifications()
                   .ComposeUrlTrackerCoreConfigurations()
                   .ComposeUrlTrackerCoreMaps()
                   .ComposeUrlTrackerCoreAbstractions()
                   .ComposeDefaultInterceptors()
                   .ComposeDefaultInterceptPreprocessors()
                   .ComposeDefaultInterceptConverters();

            builder.Services.AddSingleton<IDomainProvider, DomainProvider>();
            builder.Services.Decorate<IDomainProvider>((decoratee, factory) => new DecoratorDomainProviderCaching(decoratee, factory.GetRequiredService<AppCaches>().RuntimeCache));

            builder.Services.AddSingleton<IDefaultInterceptContextFactory, DefaultInterceptContextFactory>();
            builder.Services.AddSingleton<IIntermediateInterceptService, IntermediateInterceptService>();
            builder.Services.AddSingleton<IInterceptService, InterceptService>();
            builder.Services.AddSingleton<IRedirectService, RedirectService>();
            builder.Services.AddSingleton<IClientErrorService, ClientErrorService>();
            builder.Services.AddSingleton<ILegacyService, LegacyService>();
            builder.Services.AddSingleton<IRedirectRepository, RedirectRepository>();
            builder.Services.AddSingleton<IClientErrorRepository, ClientErrorRepository>();
            builder.Services.AddSingleton<ILegacyRepository, LegacyRepository>();
            builder.Services.AddSingleton<IValidationHelper, ValidationHelper>();
            builder.Services.AddSingleton<IExceptionHelper, ExceptionHelper>();

            builder.Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));

            builder.Services.AddSingleton<IStaticUrlProviderCollection>(factory => factory.GetRequiredService<StaticUrlProviderCollection>());
            builder.Services.AddSingleton<IInterceptPreprocessorCollection>(factory => factory.GetRequiredService<InterceptPreprocessorCollection>());
            builder.Services.AddSingleton<IInterceptorCollection>(factory => factory.GetRequiredService<InterceptorCollection>());
            builder.Services.AddSingleton<IInterceptConverterCollection>(factory => factory.GetRequiredService<InterceptConverterCollection>());
        }
    }

    [ExcludeFromCodeCoverage]
    public static class IUmbracoBuilderExtensions
    {
        public static InterceptorCollectionBuilder Interceptors(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<InterceptorCollectionBuilder>();

        public static InterceptPreprocessorCollectionBuilder InterceptPreprocessors(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<InterceptPreprocessorCollectionBuilder>();

        public static StaticUrlProviderCollectionBuilder StaticUrlProviders(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<StaticUrlProviderCollectionBuilder>();

        public static InterceptConverterCollectionBuilder InterceptConverters(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<InterceptConverterCollectionBuilder>();

        public static IUmbracoBuilder ComposeDefaultInterceptors(this IUmbracoBuilder builder)
        {
            builder.Interceptors()
                .Append<StaticUrlRedirectInterceptor>()
                .Append<RegexRedirectInterceptor>()
                .Append<NoLongerExistsInterceptor>();

            builder.StaticUrlProviders()
                .Append<StaticUrlProvider>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultInterceptPreprocessors(this IUmbracoBuilder builder)
        {
            builder.InterceptPreprocessors()
                .Append<DomainUrlPreprocessor>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultInterceptConverters(this IUmbracoBuilder builder)
        {
            builder.InterceptConverters()
                .Append<MapperInterceptConverter<UrlTrackerShallowRedirect, ShallowRedirect>>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreComponents(this IUmbracoBuilder builder)
        {
            builder.AddComponent<MigrationComponent>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreNotifications(this IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<DomainDeletedNotification, ContentChangeHandlerComponent>();
            builder.AddNotificationHandler<DomainSavedNotification, ContentChangeHandlerComponent>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreConfigurations(this IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<UrlTrackerSettings>()
                            .Bind(builder.Config.GetSection(Defaults.Options.UrlTrackerSection));
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreMaps(this IUmbracoBuilder builder)
        {
            builder.MapDefinitions()
                .Add<LegacyDatabaseMap>()
                .Add<ServiceLayerMaps>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerCoreAbstractions(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IUmbracoContextFactoryAbstraction, UmbracoContextFactoryAbstraction>();
            return builder;
        }
    }
}
