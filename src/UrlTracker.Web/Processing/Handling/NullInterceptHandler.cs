using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing.Handling
{
    [ExcludeFromCodeCoverage]
    public class NullInterceptHandler
        : ISpecificResponseInterceptHandler
    {
        public bool CanHandle(IIntercept intercept)
        {
            return ReferenceEquals(intercept, CachableInterceptBase.NullIntercept);
        }

        public ValueTask HandleAsync(RequestDelegate next, HttpContext context, IIntercept intercept)
        {
            return new ValueTask(next(context));
        }
    }
}
