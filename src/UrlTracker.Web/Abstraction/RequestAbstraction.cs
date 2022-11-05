using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Abstraction
{
    [ExcludeFromCodeCoverage]
    public class RequestAbstraction : IRequestAbstraction
    {
        public Uri? GetReferrer(HttpRequest request)
        {
            return request.GetTypedHeaders().Referer;
        }
    }

    [ExcludeFromCodeCoverage]
    public static class RequestExtensions
    {
        public static Uri? GetReferrer(this HttpRequest request, IRequestAbstraction abstraction)
            => abstraction.GetReferrer(request);
    }
}