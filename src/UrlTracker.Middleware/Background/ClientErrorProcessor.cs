using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Core;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;

namespace UrlTracker.Middleware.Background
{
    /// <summary>
    /// A single instance of a client error to process on the background
    /// </summary>
    /// <param name="Url">The URL that generated the client error</param>
    /// <param name="Moment">The time and date at which the client error was generated</param>
    /// <param name="Referrer">The URL from which the current URL is requested</param>
    public record ClientErrorProcessorItem(Url Url, DateTime Moment, string? Referrer);

    /// <summary>
    /// A device to queue client errors to a background runner
    /// </summary>
    public interface IClientErrorProcessorQueue
    {
        /// <summary>
        /// Call this method to get the next item from the queue. For internal use only.
        /// </summary>
        /// <param name="cancellationToken">Pass in a cancellation token to break execution upon cancellation</param>
        /// <returns>The next item on the queue</returns>
        ValueTask<ClientErrorProcessorItem> ReadAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Call this method to add a client error to the queue. This item will be retrieved by a call to <see cref="ReadAsync(CancellationToken)"/>
        /// </summary>
        /// <param name="item">The data to enqueue</param>
        /// <returns>A task that completes after pushing the item onto the queue</returns>
        ValueTask WriteAsync(ClientErrorProcessorItem item);
    }

    // Based on: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio#queued-background-tasks
    internal sealed class ClientErrorProcessorQueue : IClientErrorProcessorQueue
    {
        private readonly Channel<ClientErrorProcessorItem> _queue;

        public ClientErrorProcessorQueue()
        {
            var options = new BoundedChannelOptions(500)
            {
                FullMode = BoundedChannelFullMode.DropWrite
            };

            _queue = Channel.CreateBounded<ClientErrorProcessorItem>(options);
        }

        public ValueTask WriteAsync(ClientErrorProcessorItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            return _queue.Writer.WriteAsync(item);
        }

        public async ValueTask<ClientErrorProcessorItem> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await _queue.Reader.ReadAsync(cancellationToken);
            return result;
        }
    }


    internal class ClientErrorProcessor
        : BackgroundService
    {
        private readonly IClientErrorProcessorQueue _queue;
        private readonly ILogger<ClientErrorProcessor> _logger;
        private readonly IClientErrorService _clientErrorService;
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;
        private readonly IUrlClassifierStrategyCollection _classifierStrategyCollection;
        private readonly IRecommendationService _recommendationService;

        public ClientErrorProcessor(
            IClientErrorProcessorQueue queue,
            ILogger<ClientErrorProcessor> logger,
            IClientErrorService clientErrorService,
            IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions,
            IUrlClassifierStrategyCollection classifierStrategyCollection,
            IRecommendationService recommendationService)
        {
            _queue = queue;
            _logger = logger;
            _clientErrorService = clientErrorService;
            _requestHandlerOptions = requestHandlerOptions;
            _classifierStrategyCollection = classifierStrategyCollection;
            _recommendationService = recommendationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var item = await _queue.ReadAsync(stoppingToken);
                    var urlString = item.Url.ToString(UrlType.Absolute, _requestHandlerOptions.CurrentValue.AddTrailingSlash);
                    await _clientErrorService.ReportAsync(urlString, item.Moment, item.Referrer);

                    var classification = _classifierStrategyCollection.Classify(item.Url);

                    var recommendation = _recommendationService.GetOrCreate(urlString, classification);
                    recommendation.VariableScore++;

                    _recommendationService.Save(recommendation);
                }
                catch (OperationCanceledException)
                {
                    // No need to do anything. The process is stopping, the exception signals cancellation
                }
                catch (Exception e)
                {
                    _logger.LogBackgroundProcessingFailure(e);
                }
            }
        }
    }
}
