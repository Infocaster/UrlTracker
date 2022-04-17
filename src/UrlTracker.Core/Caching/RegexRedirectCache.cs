using System.Collections.Generic;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Caching
{
    public class RegexRedirectCache
        : TypedMemoryCache<string, IReadOnlyCollection<UrlTrackerShallowRedirect>>, IRegexRedirectCache
    {
        public RegexRedirectCache()
            : base(1) // cache size is always 1, because there will only be 1 item stored in this cache
        { }
    }
}
