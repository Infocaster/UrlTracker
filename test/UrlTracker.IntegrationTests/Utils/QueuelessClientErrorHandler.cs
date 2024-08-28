using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Core;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Models;
using UrlTracker.Middleware.Background;

namespace UrlTracker.IntegrationTests.Utils
{
    internal class QueuelessClientErrorHandler
        : IClientErrorProcessorQueue
    {
        private readonly IClientErrorService _clientErrorService;
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;
        private readonly IUrlClassifierStrategyCollection _classifierStrategyCollection;
        private readonly IRecommendationService _recommendationService;

        public QueuelessClientErrorHandler(
            IClientErrorService clientErrorService,
            IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions,
            IUrlClassifierStrategyCollection classifierStrategyCollection,
            IRecommendationService recommendationService)
        {
            _clientErrorService = clientErrorService;
            _requestHandlerOptions = requestHandlerOptions;
            _classifierStrategyCollection = classifierStrategyCollection;
            _recommendationService = recommendationService;
        }

        public async ValueTask<ClientErrorProcessorItem> ReadAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(-1, cancellationToken);
            throw new InvalidOperationException("You're not supposed to get here");
        }

        public async ValueTask WriteAsync(ClientErrorProcessorItem item)
        {
            var urlString = item.Url.ToString(UrlType.Absolute, _requestHandlerOptions.CurrentValue.AddTrailingSlash);
            await _clientErrorService.ReportAsync(urlString, item.Moment, item.Referrer);

            var classification = _classifierStrategyCollection.Classify(item.Url);

            var recommendation = _recommendationService.GetOrCreate(urlString, classification);
            recommendation.VariableScore++;

            _recommendationService.Save(recommendation);
        }
    }
}
