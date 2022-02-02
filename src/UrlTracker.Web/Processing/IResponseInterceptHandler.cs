using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    public interface IResponseInterceptHandler
    {
        bool CanHandle(IIntercept intercept);
        ValueTask HandleAsync(HttpContextBase context, IIntercept intercept);
    }
}
