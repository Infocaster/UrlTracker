using System.Diagnostics.CodeAnalysis;
using LightInject;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Components;
using UrlTracker.Core.Configuration;
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
        : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var container = composition.Concrete as IServiceRegistry;

            composition.ComposeUrlTrackerCoreComponents()
                       .ComposeUrlTrackerCoreConfigurations()
                       .ComposeUrlTrackerCoreMaps()
                       .ComposeUrlTrackerCoreAbstractions()
                       .ComposeDefaultInterceptors()
                       .ComposeDefaultInterceptPreprocessors()
                       .ComposeDefaultInterceptConverters();

            composition.RegisterUnique<IDomainProvider, DomainProvider>();
            container.Decorate<IDomainProvider>((factory, decoratee) => new DecoratorDomainProviderCaching(decoratee, factory.GetInstance<AppCaches>().RuntimeCache));

            composition.RegisterUnique<IDefaultInterceptContextFactory, DefaultInterceptContextFactory>();
            composition.RegisterUnique<IIntermediateInterceptService, IntermediateInterceptService>();
            composition.RegisterUnique<IInterceptService, InterceptService>();
            composition.RegisterUnique<IRedirectService, RedirectService>();
            composition.RegisterUnique<IClientErrorService, ClientErrorService>();
            composition.RegisterUnique<ILegacyService, LegacyService>();
            composition.RegisterUnique<IRedirectRepository, RedirectRepository>();
            composition.RegisterUnique<IClientErrorRepository, ClientErrorRepository>();
            composition.RegisterUnique<ILegacyRepository, LegacyRepository>();
            composition.RegisterUnique<IValidationHelper, ValidationHelper>();
            composition.RegisterUnique<IExceptionHelper, ExceptionHelper>();

            composition.Register<ILogger, Logger>(Lifetime.Transient);

            composition.RegisterUnique<IStaticUrlProviderCollection>(factory => factory.GetInstance<StaticUrlProviderCollection>());
            composition.RegisterUnique<IInterceptPreprocessorCollection>(factory => factory.GetInstance<InterceptPreprocessorCollection>());
            composition.RegisterUnique<IInterceptorCollection>(factory => factory.GetInstance<InterceptorCollection>());
            composition.RegisterUnique<IInterceptConverterCollection>(factory => factory.GetInstance<InterceptConverterCollection>());
        }
    }

    [ExcludeFromCodeCoverage]
    public static class CompositionExtensions
    {
        public static InterceptorCollectionBuilder Interceptors(this Composition composition)
            => composition.WithCollectionBuilder<InterceptorCollectionBuilder>();

        public static InterceptPreprocessorCollectionBuilder InterceptPreprocessors(this Composition composition)
            => composition.WithCollectionBuilder<InterceptPreprocessorCollectionBuilder>();

        public static StaticUrlProviderCollectionBuilder StaticUrlProviders(this Composition composition)
            => composition.WithCollectionBuilder<StaticUrlProviderCollectionBuilder>();

        public static InterceptConverterCollectionBuilder InterceptConverters(this Composition composition)
            => composition.WithCollectionBuilder<InterceptConverterCollectionBuilder>();

        public static ConfigurationComposition<T> Configure<T, P>(this IRegister composition)
            where T : class
            where P : IConfiguration<T>
        {
            return ConfigurationComposition<T>.Create<P>(composition);
        }

        public static Composition ComposeDefaultInterceptors(this Composition composition)
        {
            composition.Interceptors()
                .Append<StaticUrlRedirectInterceptor>()
                .Append<RegexRedirectInterceptor>()
                .Append<NoLongerExistsInterceptor>();

            composition.StaticUrlProviders()
                .Append<StaticUrlProvider>();
            return composition;
        }

        public static Composition ComposeDefaultInterceptPreprocessors(this Composition composition)
        {
            composition.InterceptPreprocessors()
                .Append<DomainUrlPreprocessor>();
            return composition;
        }

        public static Composition ComposeDefaultInterceptConverters(this Composition composition)
        {
            composition.InterceptConverters()
                .Append<MapperInterceptConverter<UrlTrackerShallowRedirect, ShallowRedirect>>();
            return composition;
        }

        public static Composition ComposeUrlTrackerCoreComponents(this Composition composition)
        {
            composition.Components()
                .Append<MigrationComponent>()
                .Append<ContentChangeHandlerComponent>();
            return composition;
        }

        public static Composition ComposeUrlTrackerCoreConfigurations(this Composition composition)
        {
            composition.Configure<UrlTrackerSettings, UrlTrackerConfigurationProvider>()
                       .Lazy();

            return composition;
        }

        public static Composition ComposeUrlTrackerCoreMaps(this Composition composition)
        {
            composition.MapDefinitions()
                .Add<LegacyDatabaseMap>()
                .Add<ServiceLayerMaps>();
            return composition;
        }

        public static Composition ComposeUrlTrackerCoreAbstractions(this Composition composition)
        {
            composition.RegisterUnique<IAppSettingsAbstraction, AppSettingsAbstraction>();
            composition.RegisterUnique<IUmbracoContextFactoryAbstraction, UmbracoContextFactoryAbstraction>();
            return composition;
        }
    }
}
