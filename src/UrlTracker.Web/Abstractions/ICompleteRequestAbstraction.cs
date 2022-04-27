using System.Web;

namespace UrlTracker.Web.Abstractions
{
    public interface ICompleteRequestAbstraction
    {
        void CompleteRequest(HttpContextBase httpContext);
    }
}