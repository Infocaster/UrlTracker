using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Compatibility;
using UrlTracker.Web.Configuration;
using UrlTracker.Web.Events;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Map;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web
{
    [ExcludeFromCodeCoverage]
    public class WebComposer
        : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ComposeUrlTrackerWebComponents()
                   .ComposeUrlTrackerWebMaps()
                   .ComposeDefaultResponseIntercepts()
                   .ComposeDefaultRequestInterceptFilters()
                   .ComposeDefaultClientErrorFilters();

            builder.Services.AddSingleton<IRequestModelPatcher, RequestModelPatcher>();
            builder.Services.AddTransient<IRequestAbstraction, RequestAbstraction>();
            builder.Services.AddTransient<IResponseAbstraction, ResponseAbstraction>();

            builder.Services.AddSingleton<IResponseInterceptHandlerCollection>(factory => factory.GetRequiredService<ResponseInterceptHandlerCollection>());
            builder.Services.AddSingleton<IRequestInterceptFilterCollection>(factory => factory.GetRequiredService<RequestInterceptFilterCollection>());
            builder.Services.AddSingleton<IClientErrorFilterCollection>(factory => factory.GetRequiredService<ClientErrorFilterCollection>());
            builder.Services.AddSingleton<IReservedPathSettingsProvider, ReservedPathSettingsProvider>();
            builder.Services.AddSingleton<IContentValueReaderFactory, ContentValueReaderFactory>();
        }
    }

    [ExcludeFromCodeCoverage]
    public static class IUmbracoBuilderExtensions
    {
        public static IUmbracoBuilder ComposeUrlTrackerWebComponents(this IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentMovingNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationHandler<ContentMovedNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationHandler<ContentPublishingNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationHandler<ContentPublishedNotification, ContentChangeNotificationHandler>();
            builder.AddNotificationAsyncHandler<UrlTrackerHandled, UrlTrackerHandledNotificationHandler>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultResponseIntercepts(this IUmbracoBuilder builder)
        {
            builder.ResponseInterceptHandlers()!
                .Append<RedirectResponseInterceptHandler>()
                .Append<NoLongerExistsResponseInterceptHandler>()
                .Append<NullInterceptHandler>();

            builder.Services.AddSingleton<ILastChanceResponseInterceptHandler, LastChanceResponseInterceptHandler>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultRequestInterceptFilters(this IUmbracoBuilder builder)
        {
            builder.RequestInterceptFilters()!
                .Append<UrlReservedPathFilter>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultClientErrorFilters(this IUmbracoBuilder builder)
        {
            builder.ClientErrorFilters()!
                .Append<ConfigurationClientErrorFilter>()
                .Append<IgnoredClientErrorFilter>();
            return builder;
        }

        public static IUmbracoBuilder ComposeUrlTrackerWebMaps(this IUmbracoBuilder builder)
        {
            builder.MapDefinitions()!
                .Add<RedirectMap>()
                .Add<ResponseMap>()
                .Add<RequestMap>()
                .Add<CsvMap>();
            return builder;
        }

        public static ResponseInterceptHandlerCollectionBuilder? ResponseInterceptHandlers(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<ResponseInterceptHandlerCollectionBuilder>();

        public static RequestInterceptFilterCollectionBuilder? RequestInterceptFilters(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<RequestInterceptFilterCollectionBuilder>();

        public static ClientErrorFilterCollectionBuilder? ClientErrorFilters(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<ClientErrorFilterCollectionBuilder>();
    }
}
