using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace UrlTracker.Web.Events.Models
{
    [ExcludeFromCodeCoverage]
    public class ProcessedEventArgs
        : EventArgs
    {
        public ProcessedEventArgs(HttpContextBase httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContextBase HttpContext { get; }
    }
}
