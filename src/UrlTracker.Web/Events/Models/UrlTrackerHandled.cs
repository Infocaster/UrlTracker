using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Web.Events.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerHandled
        : INotification
    {
        public UrlTrackerHandled(HttpContext httpContext, Url url)
        {
            HttpContext = httpContext;
            Url = url;
        }

        public HttpContext HttpContext { get; }
        public Url Url { get; }
    }
}
