using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware
{
    /// <summary>
    /// A middleware that detects client error responses and registers them in the URL Tracker service
    /// </summary>
    public class UrlTrackerRecommendationTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IClientErrorFilterCollection _clientErrorFilterCollection;
        private readonly IRecommendationService _recommendationService;
        private readonly IUrlClassifierStrategyCollection _classifierStrategyCollection;
        private readonly ILogger<UrlTrackerRecommendationTrackingMiddleware> _logger;
        private readonly IRuntimeState _runtimeState;
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;
        private readonly IRequestAbstraction _requestAbstraction;

        /// <inheritdoc />
        public UrlTrackerRecommendationTrackingMiddleware(RequestDelegate next,
                                                          IClientErrorFilterCollection clientErrorFilterCollection,
                                                          IRecommendationService recommendationService,
                                                          IUrlClassifierStrategyCollection classifierStrategyCollection,
                                                          ILogger<UrlTrackerRecommendationTrackingMiddleware> logger,
                                                          IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions,
                                                          IRequestAbstraction requestAbstraction,
                                                          IRuntimeState runtimeState)
        {
            _next = next;
            _clientErrorFilterCollection = clientErrorFilterCollection;
            _recommendationService = recommendationService;
            _classifierStrategyCollection = classifierStrategyCollection;
            _logger = logger;
            _requestHandlerOptions = requestHandlerOptions;
            _requestAbstraction = requestAbstraction;
            _runtimeState = runtimeState;
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
                _logger.LogAbortClientErrorHandling("Incoming request failed recommendation candidacy check");
                return;
            }

            var url = context.Request.GetUrl();
            var classification = _classifierStrategyCollection.Classify(url);
            
            var requestHandlerOptionsValue = _requestHandlerOptions.CurrentValue;
            var urlString = context.Request.GetUrl().ToString(UrlType.Absolute, requestHandlerOptionsValue.AddTrailingSlash);

            var recommendation = _recommendationService.GetOrCreate(urlString, classification);
            recommendation.VariableScore++;

            _recommendationService.Save(recommendation);
        }
    }
}
