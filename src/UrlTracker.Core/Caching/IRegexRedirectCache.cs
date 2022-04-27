using System.Collections.Generic;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Caching
{
    public interface IRegexRedirectCache
        : ITypedMemoryCache<string, IReadOnlyCollection<UrlTrackerShallowRedirect>>
    { }
}