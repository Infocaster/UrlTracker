using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing.Handling
{
    public class NoLongerExistsResponseInterceptHandler
        : ResponseInterceptHandlerBase<IClientError>
    {
        private readonly IResponseAbstraction _responseAbstraction;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public NoLongerExistsResponseInterceptHandler(IResponseAbstraction responseAbstraction, IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _responseAbstraction = responseAbstraction;
            _umbracoContextFactory = umbracoContextFactory;
        }

        protected override async ValueTask HandleAsync(RequestDelegate next, HttpContext context, IClientError intercept)
        {
            if (ShouldRedirect())
            {
                var response = context.Response;
                response.Clear(_responseAbstraction);
                response.StatusCode = 410;
                await context.Response.CompleteAsync();
                return;
            }

            await next(context);
        }

        private bool ShouldRedirect()
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            return cref.GetResponseCode() >= 400;
        }
    }
}
