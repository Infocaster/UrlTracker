using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    public interface IResponseInterceptHandler
    {
        ValueTask HandleAsync(RequestDelegate next, HttpContext context, IIntercept intercept);
    }

    public interface ISpecificResponseInterceptHandler
        : IResponseInterceptHandler
    {
        bool CanHandle(IIntercept intercept);
    }

    public interface ILastChanceResponseInterceptHandler
        : IResponseInterceptHandler
    { }
}
