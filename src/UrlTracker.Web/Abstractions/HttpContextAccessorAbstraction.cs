using System.Diagnostics.CodeAnalysis;
using System.Web;
using Umbraco.Web;

namespace UrlTracker.Web.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class HttpContextAccessorAbstraction : IHttpContextAccessorAbstraction
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessorAbstraction(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContextBase HttpContext => new HttpContextWrapper(_httpContextAccessor.HttpContext);
    }
}
