using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting
{
    public interface IInterceptorCollection
    {
        ValueTask<ICachableIntercept> InterceptAsync(Url url, IReadOnlyInterceptContext context);
    }
}