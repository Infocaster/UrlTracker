using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Abstraction
{
    public interface IResponseAbstraction
    {
        void Clear(HttpResponse response);
        void SetRedirectLocation(HttpResponse response, string url);
    }
}