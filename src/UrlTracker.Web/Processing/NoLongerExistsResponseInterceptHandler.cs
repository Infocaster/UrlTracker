using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Web;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Database.Models;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing
{
    public class NoLongerExistsResponseInterceptHandler
        : ResponseInterceptHandlerBase<UrlTrackerShallowClientError>
    {
        private readonly IResponseAbstraction _responseAbstraction;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public NoLongerExistsResponseInterceptHandler(IResponseAbstraction responseAbstraction, IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _responseAbstraction = responseAbstraction;
            _umbracoContextFactory = umbracoContextFactory;
        }

        protected override async ValueTask HandleAsync(RequestDelegate next, HttpContext context, UrlTrackerShallowClientError intercept)
        {
            if (ShouldRedirect(context))
            {
                var response = context.Response;
                response.Clear(_responseAbstraction);
                response.StatusCode = 410;
                await context.Response.CompleteAsync();
                return;
            }

            await next(context);
        }

        private bool ShouldRedirect(HttpContext context)
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            return cref.GetResponseCode() >= 400;
        }
    }
}
