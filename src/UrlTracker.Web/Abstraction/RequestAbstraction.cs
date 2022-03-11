using System;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Abstraction
{
    public class RequestAbstraction : IRequestAbstraction
    {
        public Uri GetReferrer(HttpRequest request)
        {
            return request.GetTypedHeaders().Referer;
        }
    }

    public static class RequestExtensions
    {
        public static Uri GetReferrer(this HttpRequest request, IRequestAbstraction abstraction)
            => abstraction.GetReferrer(request);
    }
}