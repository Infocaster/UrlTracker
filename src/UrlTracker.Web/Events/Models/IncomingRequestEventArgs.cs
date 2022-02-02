using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace UrlTracker.Web.Events.Models
{
    [ExcludeFromCodeCoverage]
    public class IncomingRequestEventArgs
        : EventArgs
    {
        public IncomingRequestEventArgs(HttpContextBase httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContextBase HttpContext { get; }
    }
}
