using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Middleware.Background;
using UrlTracker.Web;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware
{
    /// <summary>
    /// A middleware that detects client error responses and registers them in the URL Tracker service
    /// </summary>
    public class UrlTrackerClientErrorTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IClientErrorFilterCollection _clientErrorFilterCollection;
        private readonly ILogger<UrlTrackerClientErrorTrackingMiddleware> _logger;
        private readonly IRuntimeState _runtimeState;
        private readonly IClientErrorProcessorQueue _processorQueue;
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;
        private readonly IRequestAbstraction _requestAbstraction;

        /// <inheritdoc />
        public UrlTrackerClientErrorTrackingMiddleware(RequestDelegate next,
                                                    IClientErrorFilterCollection clientErrorFilterCollection,
                                                    ILogger<UrlTrackerClientErrorTrackingMiddleware> logger,
                                                    IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions,
                                                    IRequestAbstraction requestAbstraction,
                                                    IRuntimeState runtimeState,
                                                    IClientErrorProcessorQueue processorQueue)
        {
            _next = next;
            _clientErrorFilterCollection = clientErrorFilterCollection;
            _logger = logger;
            _requestHandlerOptions = requestHandlerOptions;
            _requestAbstraction = requestAbstraction;
            _runtimeState = runtimeState;
            _processorQueue = processorQueue;
        }

        /// <summary>
        /// Run the middleware
        /// </summary>
        /// <param name="context">Current request context</param>
        /// <returns>A task representing the progress of this method</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context).ConfigureAwait(false);

            // ignore this middleware as long as umbraco hasn't been initialised yet
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            if (!await _clientErrorFilterCollection.EvaluateCandidacyAsync(context))
            {
                _logger.LogAbortClientErrorHandling("Incoming request failed client error candidacy check");
                return;
            }

            var requestHandlerOptionsValue = _requestHandlerOptions.CurrentValue;
            var url = context.Request.GetUrl().ToString(UrlType.Absolute, requestHandlerOptionsValue.AddTrailingSlash);
            var referrer = context.Request.GetReferrer(_requestAbstraction);

            await _processorQueue.WriteAsync(new ClientErrorProcessorItem(url, DateTime.Now, referrer?.ToString()));
        }
    }
}
