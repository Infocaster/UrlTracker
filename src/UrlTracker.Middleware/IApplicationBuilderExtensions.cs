using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace UrlTracker.Middleware
{
    /// <summary>
    /// Extensions for registering URL Tracker middleware to the request pipeline
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Redirect incoming requests using the URL Tracker service
        /// </summary>
        /// <param name="app">The pipeline builder</param>
        /// <returns>The pipeline builder after adding this middleware</returns>
        public static IUmbracoApplicationBuilderContext UseUrlTrackerRedirects(this IUmbracoApplicationBuilderContext app)
        {
            app.AppBuilder.UseUrlTrackerRedirects();
            return app;
        }

        /// <summary>
        /// Redirect incoming requests using the URL Tracker service
        /// </summary>
        /// <param name="app">The pipeline builder</param>
        /// <returns>The pipeline builder after adding this middleware</returns>
        public static IApplicationBuilder UseUrlTrackerRedirects(this IApplicationBuilder app)
        {
            app.UseMiddleware<UrlTrackerRedirectMiddleware>();
            return app;
        }

        /// <summary>
        /// Track client error responses using the URL Tracker service
        /// </summary>
        /// <param name="app">The pipeline builder</param>
        /// <returns>The pipeline builder after adding this middleware</returns>
        public static IUmbracoApplicationBuilderContext UseUrlTrackerClientErrorTracking(this IUmbracoApplicationBuilderContext app)
        {
            app.AppBuilder.UseUrlTrackerClientErrorTracking();
            return app;
        }

        /// <summary>
        /// Track client error responses using the URL Tracker service
        /// </summary>
        /// <param name="app">The pipeline builder</param>
        /// <returns>The pipeline builder after adding this middleware</returns>
        public static IApplicationBuilder UseUrlTrackerClientErrorTracking(this IApplicationBuilder app)
        {
            app.UseMiddleware<UrlTrackerClientErrorTrackingMiddleware>();
            return app;
        }

        /// <summary>
        /// Track recommendation opportunities using the URL Tracker service
        /// </summary>
        /// <param name="app">The pipeline builder</param>
        /// <returns>The pipeline builder after adding this middleware</returns>
        public static IUmbracoApplicationBuilderContext UseUrlTrackerRecommendationTracking(this IUmbracoApplicationBuilderContext app)
        {
            app.AppBuilder.UseUrlTrackerRecommendationTracking();
            return app;
        }

        /// <summary>
        /// Track recommendation opportunities using the URL Tracker service
        /// </summary>
        /// <param name="app">The pipeline builder</param>
        /// <returns>The pipeline builder after adding this middleware</returns>
        public static IApplicationBuilder UseUrlTrackerRecommendationTracking(this IApplicationBuilder app)
        {
            app.UseMiddleware<UrlTrackerRecommendationTrackingMiddleware>();
            return app;
        }
    }
}
