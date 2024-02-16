using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Core.Caching.Memory.Options
{
    /// <summary>
    /// URL Tracker options related to in-memory caching
    /// </summary>
    public class UrlTrackerMemoryCacheOptions
    {
        /// <summary>
        /// Set this value to <see langword="false"/> to completely disable caching of intercepts
        /// </summary>
        /// <remarks>
        /// <para>It is not recommended to disable intercept caching, as searching for intercepts is an expensive operation</para>
        /// <para>Defaults to <see langword="true"/></para>
        /// </remarks>
        public bool EnableInterceptCaching { get; set; } = true;

        /// <summary>
        /// Set this value to <see langword="false"/> to disable caching of all regex redirects
        /// </summary>
        /// <remarks>
        /// <para>It is not recommended to disable regex redirect caching, as reading them from the database is an expensive operation</para>
        /// <para>Defaults to <see langword="true"/></para>
        /// </remarks>
        public bool EnableRegexRedirectCaching { get; set; } = true;

        /// <summary>
        /// Set this value to <see langword="true"/> to increase cachability and performance at the expense of working memory
        /// </summary>
        /// <remarks>
        /// <para>This option will load all redirects and no-longer-exists responses into memory to reduce traffic between the application and the database.</para>
        /// </remarks>
        public bool EnableActiveCache { get; set; } = false;

        /// <summary>
        /// The time in minutes before an intercept is removed from cache after its last use
        /// </summary>
        /// <remarks>
        /// <para>Value must be a value higher than 1 or <see langword="null"/></para>
        /// <para>Setting this value to <see langword="null"/> will disable cache expiration for intercepts</para>
        /// <para>Defaults to 2 days</para>
        /// </remarks>
        [Range(1, int.MaxValue, ErrorMessage = "Value must be 1 or more")]
        public int? InterceptSlidingCacheMinutes { get; set; } = 60 * 24 * 2; // cache for 2 days by default

        /// <summary>
        /// The maximum amount of intercepts to cache
        /// </summary>
        /// <remarks>
        /// <para>Defaults to 5000</para>
        /// </remarks>
        public long MaxCachedIntercepts { get; set; } = 5000;
    }
}
