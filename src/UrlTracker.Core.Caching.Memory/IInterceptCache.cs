using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Caching.Memory
{
    /// <summary>
    /// When implemented, this type provides caching capabilities for intercepts by url
    /// </summary>
    public interface IInterceptCache
        : ITypedMemoryCache<Url, ICachableIntercept>
    { }
}