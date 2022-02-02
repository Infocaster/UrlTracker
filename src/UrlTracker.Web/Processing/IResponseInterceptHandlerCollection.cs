using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    public interface IResponseInterceptHandlerCollection
    {
        IResponseInterceptHandler Get(IIntercept intercept);
    }
}