using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Notifications;

namespace UrlTracker.Web.Events.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerHandled
        : INotification
    {
        public UrlTrackerHandled(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; }
    }
}
