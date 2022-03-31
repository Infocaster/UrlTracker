using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Abstraction
{
    public class ResponseAbstraction : IResponseAbstraction
    {
        public void Clear(HttpResponse response)
        {
            response.Clear();
        }

        public void SetRedirectLocation(HttpResponse response, string url)
        {
            response.Headers["Location"] = url;
        }
    }

    public static class ResponseExtensions
    {
        public static void Clear(this HttpResponse response, IResponseAbstraction abstraction)
        {
            abstraction.Clear(response);
        }

        public static void SetRedirectLocation(this HttpResponse response, IResponseAbstraction abstraction, string url)
        {
            abstraction.SetRedirectLocation(response, url);
        }
    }
}