using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Caching.Memory
{
    /// <summary>
    /// An implementation of <see cref="IInterceptCache"/> as a <see cref="TypedMemoryCache{Url, ICachableIntercept}"/>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InterceptCache : TypedMemoryCache<Url, ICachableIntercept>, IInterceptCache
    {
        /// <inheritdoc />
        public InterceptCache(IOptions<UrlTrackerMemoryCacheOptions> options)
            : base(options.Value.MaxCachedIntercepts)
        { }
    }
}
