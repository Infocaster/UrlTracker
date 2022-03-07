using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Web
{
    public static class HttpContextExtensions
    {
        public static Url GetUrl(this HttpRequest request)
        {
            return Url.Create(
                request.IsHttps ? Protocol.Https : Protocol.Http,
                request.Host.Host,
                request.Host.Port,
                request.Path,
                request.QueryString.Value
            );
        }
    }
}