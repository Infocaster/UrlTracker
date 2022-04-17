using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Caching
{
    public class InterceptCache : TypedMemoryCache<Url, ICachableIntercept>, IInterceptCache
    {
        public InterceptCache(IConfiguration<UrlTrackerSettings> options)
            : base(options.Value.MaxCachedIntercepts)
        { }
    }
}
