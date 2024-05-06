using System.Threading.Tasks;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Intercepting
{
    public interface IInterceptor
    {
        ValueTask<ICachableIntercept?> InterceptAsync(Url url, IInterceptContext context);
    }
}
