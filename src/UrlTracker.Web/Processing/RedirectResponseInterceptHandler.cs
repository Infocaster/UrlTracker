using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mapping;
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
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;

        public RedirectResponseInterceptHandler(ILogger<RedirectResponseInterceptHandler> logger,
                                                IUmbracoMapper mapper,
                                                IResponseAbstraction responseAbstraction,
                                                IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                                IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions)
        {
            _logger = logger;
            _mapper = mapper;
            _responseAbstraction = responseAbstraction;
            _umbracoContextFactory = umbracoContextFactory;
            _requestHandlerOptions = requestHandlerOptions;
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
                // If redirect is a regex match, ensure that potential capture tokens are replaced in the target url
                // TODO: Evaluate side effects!
                //    This logic has been taken from the old code base. It has a potential side effect.
                //    If a pattern matches on a partial string, the non-matched part will stay in the url
                //    example: regex:"(ipsum)" targeturl: "http://example.com/$1" input: "lorem/ipsum/dolor" result: "lorem/http://example.com/ipsum/dolor"
                if (string.IsNullOrWhiteSpace(intercept.SourceUrl) && !string.IsNullOrWhiteSpace(intercept.SourceRegex))
                {
                    url = Regex.Replace((context.Request.Path + context.Request.QueryString.Value).TrimStart('/'), intercept.SourceRegex, url);
                }

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
            var requestHandlerOptionsValue = _requestHandlerOptions.CurrentValue;

            var url = _mapper.MapToUrl(intercept, context);
            var result = url?.ToString(UrlType.Absolute, requestHandlerOptionsValue.AddTrailingSlash);
            return result;
        }

        private bool ShouldIntercept()
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            return cref.GetResponseCode() == (int)HttpStatusCode.NotFound;
        }
    }
}
