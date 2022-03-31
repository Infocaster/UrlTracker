using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web
{
    public static class UmbracoMapperExtensions
    {
        const string _httpContextKey = "ic:urltracker:httpcontext";

        [ExcludeFromCodeCoverage]
        public static void SetHttpContext(this MapperContext context, HttpContext httpContext)
        {
            context.Items[_httpContextKey] = httpContext;
        }

        [ExcludeFromCodeCoverage]
        public static HttpContext? GetHttpContext(this MapperContext context)
        {
            return context.Items.TryGetValue(_httpContextKey, out var httpContext) ? httpContext as HttpContext : null;
        }

        public static Url MapToUrl(this IUmbracoMapper mapper, ShallowRedirect redirect, HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            return mapper.Map<Url>(redirect, context =>
            {
                context.SetHttpContext(httpContext);
            });
        }


        public static Url MapToUrl(this MapperContext context, ShallowRedirect redirect, HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            context.SetHttpContext(httpContext);
            return context.Map<Url>(redirect);
        }
    }
}
