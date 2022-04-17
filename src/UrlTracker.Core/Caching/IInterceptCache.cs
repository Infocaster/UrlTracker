using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Caching
{
    public interface IInterceptCache
        : ITypedMemoryCache<Url, ICachableIntercept>
    { }
}
