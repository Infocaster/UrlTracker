using System.Diagnostics.CodeAnalysis;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Mapping;
using Umbraco.Web;
using UrlTracker.Core;
using UrlTracker.Web.Abstractions;
using UrlTracker.Web.Compatibility;
using UrlTracker.Web.Components;
using UrlTracker.Web.Configuration;
using UrlTracker.Web.Configuration.Models;
using UrlTracker.Web.Events;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Map;
using UrlTracker.Web.Package;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ExcludeFromCodeCoverage]
    public class WebComposer
        : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.ComposeUrlTrackerWebComponents()
                       .ComposeUrlTrackerWebMaps()
                       .ComposeUrlTrackerWebAbstractions()
                       .ComposeUrlTrackerWebConfigurations()
                       .ComposeDefaultResponseIntercepts()
                       .ComposeDefaultRequestInterceptFilters()
                       .ComposeDefaultClientErrorFilters();

            composition.Dashboards().Add<UrlTrackerDashboard>();

            composition.RegisterUnique<IContentValueReaderFactory, ContentValueReaderFactory>();
            composition.RegisterUnique<IRequestModelPatcher, RequestModelPatcher>();

            composition.RegisterUnique<IResponseInterceptHandlerCollection>(factory => factory.GetInstance<ResponseInterceptHandlerCollection>());
            composition.RegisterUnique<IRequestInterceptFilterCollection>(factory => factory.GetInstance<RequestInterceptFilterCollection>());
            composition.RegisterUnique<IClientErrorFilterCollection>(factory => factory.GetInstance<ClientErrorFilterCollection>());

            composition.RegisterEventSubscriber<IncomingRequestEventArgs, IncomingRequestEventSubscriber>();
            composition.RegisterEventSubscriber<ProcessedEventArgs, ProcessedEventSubscriber>();
            composition.Register(typeof(IEventPublisher<,>), typeof(EventPublisher<,>), Lifetime.Singleton);
            composition.Register(typeof(IEventPublisher<>), typeof(EventPublisher<>), Lifetime.Singleton);
        }
    }

    [ExcludeFromCodeCoverage]
    public static class CompositionExtensions
    {
        public static Composition ComposeUrlTrackerWebConfigurations(this Composition composition)
        {
            composition.Configure<ReservedPathSettings, ReservedPathSettingsProvider>()
                       .Lazy();

            return composition;
        }

        public static Composition ComposeUrlTrackerWebComponents(this Composition composition)
        {
            composition.Components()
                .Append<IncomingUrlHandlingComponent>()
                .Append<ContentChangeHandlingComponent>()
                .Append<ServerVariablesComponent>();
            return composition;
        }

        public static Composition ComposeDefaultResponseIntercepts(this Composition composition)
        {
            composition.ResponseInterceptHandlers()
                .Append<RedirectResponseInterceptHandler>()
                .Append<NoLongerExistsResponseInterceptHandler>()
                .Append<NullInterceptHandler>();

            composition.RegisterUnique<ILastChanceResponseInterceptHandler, LastChanceResponseInterceptHandler>();
            return composition;
        }

        public static Composition ComposeDefaultRequestInterceptFilters(this Composition composition)
        {
            composition.RequestInterceptFilters()
                .Append<UrlReservedPathFilter>();
            return composition;
        }

        public static Composition ComposeDefaultClientErrorFilters(this Composition composition)
        {
            composition.ClientErrorFilters()
                .Append<BlacklistedUrlsClientErrorFilter>()
                .Append<ConfigurationClientErrorFilter>()
                .Append<IgnoredClientErrorFilter>();
            return composition;
        }

        public static Composition ComposeUrlTrackerWebMaps(this Composition composition)
        {
            composition.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<RedirectMap>()
                .Add<ResponseMap>()
                .Add<RequestMap>()
                .Add<CsvMap>();
            return composition;
        }

        public static Composition ComposeUrlTrackerWebAbstractions(this Composition composition)
        {
            composition.Register<ICompleteRequestAbstraction, CompleteRequestAbstraction>();
            composition.Register<IHttpContextAccessorAbstraction, HttpContextAccessorAbstraction>();
            return composition;
        }

        public static Composition RegisterEventSubscriber<TEvent, TImplementation>(this Composition composition)
            where TImplementation : IEventSubscriber<object, TEvent>
        {
            composition.Register<IEventSubscriber<object, TEvent>, TImplementation>(Lifetime.Singleton);
            return composition;
        }

        public static ResponseInterceptHandlerCollectionBuilder ResponseInterceptHandlers(this Composition composition)
            => composition.WithCollectionBuilder<ResponseInterceptHandlerCollectionBuilder>();

        public static RequestInterceptFilterCollectionBuilder RequestInterceptFilters(this Composition composition)
            => composition.WithCollectionBuilder<RequestInterceptFilterCollectionBuilder>();

        public static ClientErrorFilterCollectionBuilder ClientErrorFilters(this Composition composition)
            => composition.WithCollectionBuilder<ClientErrorFilterCollectionBuilder>();
    }
}
