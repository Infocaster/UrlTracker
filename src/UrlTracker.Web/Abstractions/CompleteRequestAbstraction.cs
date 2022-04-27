using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace UrlTracker.Web.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class CompleteRequestAbstraction
        : ICompleteRequestAbstraction
    {
        public void CompleteRequest(HttpContextBase httpContext)
        {
            httpContext.ApplicationInstance.CompleteRequest();
        }
    }

    [ExcludeFromCodeCoverage]
    public static class CompleteRequestExtensions
    {
        public static void CompleteRequest(this HttpContextBase httpContext, ICompleteRequestAbstraction abstraction)
        {
            abstraction.CompleteRequest(httpContext);
        }
    }
}
