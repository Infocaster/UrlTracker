using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing
{
    public class RedirectResponseInterceptHandler
        : ResponseInterceptHandlerBase<Redirect>
    {
        private readonly ILogger<RedirectResponseInterceptHandler> _logger;
        private readonly IResponseAbstraction _responseAbstraction;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;
        private readonly IRedirectToUrlConverterCollection _urlConverterCollection;

        public RedirectResponseInterceptHandler(ILogger<RedirectResponseInterceptHandler> logger,
                                                IResponseAbstraction responseAbstraction,
                                                IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                                IRedirectToUrlConverterCollection urlConverterCollection)
        {
            _logger = logger;
            _responseAbstraction = responseAbstraction;
            _umbracoContextFactory = umbracoContextFactory;
            _urlConverterCollection = urlConverterCollection;
        }

        protected override async ValueTask HandleAsync(RequestDelegate next, HttpContext context, Redirect intercept)
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
                response.StatusCode = (int)(intercept.Permanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.Redirect);
                _logger.LogRequestRedirected(url);
            }
            else
            {
                response.StatusCode = 410;
                _logger.LogRequestConvertedToGone();
            }
        }

        private static bool ShouldForceIntercept(Redirect intercept)
        {
            return intercept.Force;
        }

        private string? GetUrl(HttpContext context, Redirect intercept)
        {
            return _urlConverterCollection.Convert(intercept, context);
        }

        private bool ShouldIntercept()
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            return cref.GetResponseCode() == (int)HttpStatusCode.NotFound;
        }
    }
}
