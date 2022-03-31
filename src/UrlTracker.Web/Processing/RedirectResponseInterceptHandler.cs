using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Web;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing
{
    public class RedirectResponseInterceptHandler
        : ResponseInterceptHandlerBase<ShallowRedirect>
    {
        private readonly ILogger<RedirectResponseInterceptHandler> _logger;
        private readonly IUmbracoMapper _mapper;
        private readonly IResponseAbstraction _responseAbstraction;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public RedirectResponseInterceptHandler(ILogger<RedirectResponseInterceptHandler> logger,
                                                IUmbracoMapper mapper,
                                                IResponseAbstraction responseAbstraction,
                                                IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _responseAbstraction = responseAbstraction;
            _umbracoContextFactory = umbracoContextFactory;
        }

        protected override async ValueTask HandleAsync(RequestDelegate next, HttpContext context, ShallowRedirect intercept)
        {
            bool shouldForceIntercept = ShouldForceIntercept(intercept);
            bool shouldIntercept = ShouldIntercept();
            if (!shouldForceIntercept && !shouldIntercept)
            {
                _logger.LogInterceptCancelled("requirements not met.", intercept);
                await next(context);
                return;
            }

            // get actual redirect url and write to the response
            string? url = GetUrl(context, intercept);

            var response = context.Response;
            response.Clear(_responseAbstraction);
            if (url is not null)
            {
                response.SetRedirectLocation(_responseAbstraction, url);
                response.StatusCode = ((int)intercept.TargetStatusCode);
                _logger.LogRequestRedirected(url);
            }
            else
            {
                response.StatusCode = 410;
                _logger.LogRequestConvertedToGone();
            }
        }

        private static bool ShouldForceIntercept(ShallowRedirect intercept)
        {
            return intercept.Force;
        }

        private string? GetUrl(HttpContext context, ShallowRedirect intercept)
        {
            Url url = _mapper.MapToUrl(intercept, context);
            var result = url?.ToString(UrlType.Absolute);
            return result;
        }

        private bool ShouldIntercept()
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            return cref.GetResponseCode() == (int)HttpStatusCode.NotFound;
        }
    }
}
