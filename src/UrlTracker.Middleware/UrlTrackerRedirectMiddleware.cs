using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware
{
    /// <summary>
    /// A middleware that uses the URL Tracker service to redirect incoming requests
    /// </summary>
    public class UrlTrackerRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UrlTrackerRedirectMiddleware> _logger;
        private readonly IInterceptService _interceptService;
        private readonly IResponseInterceptHandlerCollection _interceptHandlers;
        private readonly IRequestInterceptFilterCollection _requestFilters;
        private readonly IRuntimeState _runtimeState;

        /// <inheritdoc />
        public UrlTrackerRedirectMiddleware(RequestDelegate next,
                                    ILogger<UrlTrackerRedirectMiddleware> logger,
                                    IInterceptService interceptService,
                                    IResponseInterceptHandlerCollection interceptHandlers,
                                    IRequestInterceptFilterCollection requestFilters,
                                    IRuntimeState runtimeState)
        {
            _next = next;
            _logger = logger;
            _interceptService = interceptService;
            _interceptHandlers = interceptHandlers;
            _requestFilters = requestFilters;
            _runtimeState = runtimeState;
        }

        /// <summary>
        /// Run the middleware
        /// </summary>
        /// <param name="context">Current request context</param>
        /// <returns>A task representing the progress of this method</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // ignore this middleware as long as umbraco hasn't been initialised yet
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                await _next(context);
                return;
            }

            Url url = context.Request.GetUrl();
            _logger.LogRequestDetected(url.ToString());

            if (!await _requestFilters.EvaluateCandidateAsync(url))
            {
                _logger.LogAbortHandling("Incoming request failed intercept candidacy check.");
                await _next(context);
                return;
            }

            var intercept = await _interceptService.GetAsync(url);

            _logger.LogInterceptFound(intercept.GetType());

            var handler = _interceptHandlers.Get(intercept);
            await handler.HandleAsync(_next, context, intercept);
        }
    }
}
