using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting
{
    public class NullInterceptor
        : ILastChanceInterceptor
    {
        public ValueTask<ICachableIntercept> InterceptAsync(Url url, IReadOnlyInterceptContext context)
        {
            return new ValueTask<ICachableIntercept>(CachableInterceptBase.NullIntercept);
        }
    }
}
