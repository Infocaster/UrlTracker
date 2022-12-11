using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace UrlTracker.Middleware
{
    /// <summary>
    /// An implementation of <see cref="IUmbracoPipelineFilter"/> that adds URL Tracker middleware to the pipeline
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlTrackerStartupFilter : IUmbracoPipelineFilter
    {
        /// <inheritdoc/>
        public string Name => "URL Tracker";

        /// <inheritdoc/>
        public void OnEndpoints(IApplicationBuilder app)
        { }

        /// <inheritdoc/>
        public void OnPostPipeline(IApplicationBuilder app)
        {
            app.UseUrlTrackerRecommendationTracking()
               .UseUrlTrackerClientErrorTracking()
               .UseUrlTrackerRedirects();
        }

        /// <inheritdoc/>
        public void OnPrePipeline(IApplicationBuilder app)
        { }
    }

    /// <summary>
    /// An implementation of <see cref="IConfigureOptions{TOptions}"/> that adds middleware to the umbraco pipeline
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConfigurePipelineOptions : IConfigureOptions<UmbracoPipelineOptions>
    {
        /// <inheritdoc/>
        public void Configure(UmbracoPipelineOptions options)
        {
            options.AddFilter(new UrlTrackerStartupFilter());
        }
    }
}
