using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Models;

namespace UrlTracker.Web
{
    public static class HttpContextExtensions
    {
        [ExcludeFromCodeCoverage]
        public static Url GetUrl(this HttpRequest request)
        {
            var result = request.HttpContext.Features.Get<Url>();
            if (result is null)
            {
                result = Url.Create(
                    request.IsHttps ? Protocol.Https : Protocol.Http,
                    request.Host.Host,
                    request.Host.Port,
                    request.Path,
                    request.QueryString.Value
                );
                request.HttpContext.Features.Set(result);
            }

            return result;
        }
    }
}