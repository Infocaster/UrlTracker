using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Caching
{
    [ExcludeFromCodeCoverage]
    public class InterceptCache : TypedMemoryCache<Url, ICachableIntercept>, IInterceptCache
    {
        public InterceptCache(IOptions<UrlTrackerSettings> options)
            : base(options.Value.MaxCachedIntercepts)
        { }
    }
}
