using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing.Handling
{
    public abstract class RedirectResponseInterceptHandler<TStrategy>
        : ResponseInterceptHandlerBase<Redirect>
        where TStrategy : ITargetStrategy
    {
        protected ILogger Logger { get; }

        protected IResponseAbstraction ResponseAbstraction { get; }

        protected IUmbracoContextFactoryAbstraction UmbracoContextFactory { get; }

        public RedirectResponseInterceptHandler(ILogger logger,
                                                IResponseAbstraction responseAbstraction,
                                                IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            Logger = logger;
            ResponseAbstraction = responseAbstraction;
            UmbracoContextFactory = umbracoContextFactory;
        }

        protected override bool CanHandle(Redirect input)
        {
            return input.Target is TStrategy;
        }

        protected override async ValueTask HandleAsync(RequestDelegate next, HttpContext context, Redirect intercept)
        {
            bool shouldForceIntercept = ShouldForceIntercept(intercept);
            bool shouldIntercept = ShouldIntercept();
            if (!shouldForceIntercept && !shouldIntercept)
            {
                Logger.LogInterceptCancelled("requirements not met.", intercept);
                await next(context);
                return;
            }

            // get actual redirect url and write to the response
            string? url = GetUrl(context, intercept, (TStrategy)intercept.Target);

            var response = context.Response;
            response.Clear(ResponseAbstraction);
            if (url is not null)
            {
                response.SetRedirectLocation(ResponseAbstraction, url);
                response.StatusCode = (int)(intercept.Permanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.Redirect);
                Logger.LogRequestRedirected(url);
            }
            else
            {
                response.StatusCode = 410;
                Logger.LogRequestConvertedToGone();
            }
        }

        private static bool ShouldForceIntercept(Redirect intercept)
        {
            return intercept.Force;
        }

        protected abstract string? GetUrl(HttpContext context, Redirect intercept, TStrategy target);
        //{
        //    return _urlConverterCollection.Convert(intercept, context);
        //}

        private bool ShouldIntercept()
        {
            using var cref = UmbracoContextFactory.EnsureUmbracoContext();
            return cref.GetResponseCode() == (int)HttpStatusCode.NotFound;
        }
    }
}
