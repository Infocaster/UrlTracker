using System.Threading.Tasks;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Intercepting
{
    public class NullInterceptor
        : ILastChanceInterceptor
    {
        public ValueTask<ICachableIntercept> InterceptAsync(Url url, IInterceptContext context)
        {
            return new ValueTask<ICachableIntercept>(CachableInterceptBase.NullIntercept);
        }
    }
}
