using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    public interface IResponseInterceptHandler
    {
        ValueTask HandleAsync(HttpContextBase context, IIntercept intercept);
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
