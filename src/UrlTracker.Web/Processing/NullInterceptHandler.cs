using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public ValueTask HandleAsync(RequestDelegate next, HttpContext context, IIntercept intercept)
        {
            return new ValueTask(next(context));
        }
    }
}
