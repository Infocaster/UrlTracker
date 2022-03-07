using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    public interface IResponseInterceptHandler
    {
        bool CanHandle(IIntercept intercept);
        ValueTask HandleAsync(RequestDelegate next, HttpContext context, IIntercept intercept);
    }
}
