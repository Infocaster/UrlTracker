using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Web.Events.Models
{
    [ExcludeFromCodeCoverage]
    public class ProcessedEventArgs
        : EventArgs
    {
        public ProcessedEventArgs(HttpContextBase httpContext, Url url)
        {
            HttpContext = httpContext;
            Url = url;
        }

        public HttpContextBase HttpContext { get; }
        public Url Url { get; }
    }
}
