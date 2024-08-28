using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using UrlTracker.Modules.Options;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Configuration;
using UrlTracker.Web.Processing;
using UrlTracker.Web.Processing.Filtering;
using UrlTracker.Web.Processing.Handling;

namespace UrlTracker.Web
{
    /// <summary>
    /// The entry point for the URL Tracker web services
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EntryPoint
    {
        /// <summary>
        /// Add all web services of the URL Tracker to the dependency injection container
        /// </summary>
        /// <remarks>
        /// <para><see cref="Core.EntryPoint.ComposeUrlTrackerCore(IUmbracoBuilder)"/> must be used before invoking this method</para>
        /// </remarks>
        /// <param name="builder">The umbraco dependency collection builder</param>
        /// <returns>The umbraco dependency collection builder after all URL Tracker web services are added</returns>
        public static IUmbracoBuilder ComposeUrlTrackerWeb(this IUmbracoBuilder builder)
        {
            builder.ComposeDefaultResponseIntercepts()
                   .ComposeDefaultRequestInterceptFilters()
                   .ComposeDefaultClientErrorFilters();

            builder.Services.AddTransient<IRequestAbstraction, RequestAbstraction>();
            builder.Services.AddTransient<IResponseAbstraction, ResponseAbstraction>();

            builder.Services.AddSingleton<IResponseInterceptHandlerCollection>(factory => factory.GetRequiredService<ResponseInterceptHandlerCollection>());
            builder.Services.AddSingleton<IRequestInterceptFilterCollection>(factory => factory.GetRequiredService<RequestInterceptFilterCollection>());
            builder.Services.AddSingleton<IClientErrorFilterCollection>(factory => factory.GetRequiredService<ClientErrorFilterCollection>());
            builder.Services.AddSingleton<IReservedPathSettingsProvider, ReservedPathSettingsProvider>();

            builder.Services.AddUrlTrackerModule("Http services");

            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultResponseIntercepts(this IUmbracoBuilder builder)
        {
            builder.ResponseInterceptHandlers()!
                .Append<UrlRedirectResponseInterceptHandler>()
                .Append<ContentRedirectResponseInterceptHandler>()
                .Append<NoLongerExistsResponseInterceptHandler>()
                .Append<NullInterceptHandler>();

            builder.Services.AddSingleton<ILastChanceResponseInterceptHandler, LastChanceResponseInterceptHandler>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultRequestInterceptFilters(this IUmbracoBuilder builder)
        {
            builder.RequestInterceptFilters()!
                .Append<CoreConfigurationRequestInterceptFilter>()
                .Append<UrlReservedPathFilter>();
            return builder;
        }

        public static IUmbracoBuilder ComposeDefaultClientErrorFilters(this IUmbracoBuilder builder)
        {
            builder.ClientErrorFilters()!
                .Append<CoreConfigurationClientErrorFilter>()
                .Append<UserAgentClientErrorFilter>()
                .Append<NotFoundClientErrorFilter>()
                .Append<BlacklistedUrlsClientErrorFilter>()
                .Append<ConstantsClientErrorFilter>();
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
