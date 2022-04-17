using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    public class NullInterceptHandler
        : IResponseInterceptHandler
    {
        public bool CanHandle(IIntercept intercept)
        {
            return object.ReferenceEquals(intercept, CachableInterceptBase.NullIntercept);
        }
        public ValueTask HandleAsync(HttpContextBase context, IIntercept intercept)
        {
            return new ValueTask();
        }
    }
}
