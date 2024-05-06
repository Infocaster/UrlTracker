using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using UrlTracker.Middleware.Background;
using UrlTracker.Middleware.Options;
using UrlTracker.Middleware.Processing;
using UrlTracker.Modules.Options;
using UrlTracker.Web;
using UrlTracker.Web.Processing.Filtering;

namespace UrlTracker.Middleware
{
    /// <summary>
    /// The entry point for the URL Tracker middleware
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EntryPoint
    {
        /// <summary>
        /// Add middleware for the URL Tracker to the dependency injection container
        /// </summary>
        /// <remarks>
        /// <para><see cref="Web.EntryPoint.ComposeUrlTrackerWeb(IUmbracoBuilder)"/> must be used before invoking this method</para>
        /// </remarks>
        /// <param name="builder">The umbraco dependency collection builder</param>
        /// <returns>The umbraco dependency collection builder after all services are added</returns>
        public static IUmbracoBuilder ComposeUrlTrackerMiddleware(this IUmbracoBuilder builder)
        {
            builder.ComposeConfigurations();

            builder.Services.ConfigureOptions<ConfigurePipelineOptions>();

            builder.RequestInterceptFilters()!
                .InsertBefore<CoreConfigurationRequestInterceptFilter, PipelineConfigurationRequestInterceptFilter>();

            builder.ClientErrorFilters()!
                .InsertBefore<CoreConfigurationClientErrorFilter, PipelineConfigurationClientErrorFilter>();

            builder.Services.AddSingleton<IClientErrorProcessorQueue, ClientErrorProcessorQueue>();
            builder.Services.AddHostedService<ClientErrorProcessor>();

            builder.Services.AddUrlTrackerModule("Middleware");

            return builder;
        }

        private static IUmbracoBuilder ComposeConfigurations(this IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<UrlTrackerPipelineOptions>()
                            .Bind(builder.Config.GetSection(Defaults.Options.Section))
                            .ValidateDataAnnotations();

            return builder;
        }
    }
}
