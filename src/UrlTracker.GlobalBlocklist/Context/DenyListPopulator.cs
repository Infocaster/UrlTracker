using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using UrlTracker.GlobalBlocklist.Services;

namespace UrlTracker.GlobalBlocklist.Context
{
    internal partial class DenyListPopulator : BackgroundService
    {
        private readonly IRetrieveDenyListService _retrieveDenyListService;
        private readonly IDenyListContext _context;
        private readonly Core.Logging.ILogger<DenyListPopulator> _logger;
        private readonly ResiliencePipeline _retryPolicy;

        public DenyListPopulator(
            IRetrieveDenyListService retrieveBlocklistService,
            IDenyListContext context,
            Core.Logging.ILogger<DenyListPopulator> logger)
        {
            _retrieveDenyListService = retrieveBlocklistService;
            _context = context;
            _logger = logger;

            _retryPolicy = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    DelayGenerator = (args) =>
                    {
                        var delay = args.AttemptNumber switch
                        {
                            0 => TimeSpan.FromMinutes(1),
                            1 => TimeSpan.FromMinutes(5),
                            2 => TimeSpan.FromMinutes(10),
                            _ => TimeSpan.FromMinutes(10),
                        };

                        return ValueTask.FromResult<TimeSpan?>(delay);
                    },
                    MaxRetryAttempts = 3,
                    OnRetry = (args) =>
                    {
                        LogFetchAttemptsRetry(_logger, args.AttemptNumber);
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await UpdateDenyListAsync(stoppingToken);

                using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await UpdateDenyListAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Deliberately left empty
            }
            catch (Exception e)
            {
                LogUnexpectedError(_logger, e);
            }
        }

        private async Task UpdateDenyListAsync(CancellationToken stoppingToken)
        {
            try
            {
                LogRefresh(_logger);
                var response = await _retryPolicy.ExecuteAsync((_) => new ValueTask<DenyListResponse>(_retrieveDenyListService.GetGlobalSettings()), stoppingToken);
                _context.SetList(response.GlobalBlocklist);
            }
            catch (OperationCanceledException)
            {
                // Cancelled exceptions need to be propagated
                throw;
            }
            catch (Exception e)
            {
                // All other failures must be logged, but MUST NOT PROPAGATE. Otherwise the timer will stop.
                LogFailureToFetch(_logger, e);
            }
        }

        [LoggerMessage(2, LogLevel.Error, "Failed to fetch the global blocklist from the external source.")]
        private static partial void LogFailureToFetch(ILogger logger, Exception exception);

        [LoggerMessage(1, LogLevel.Error, "Daily fetching of the global block list has been discontinued due to an unexpected error.")]
        private static partial void LogUnexpectedError(ILogger logger, Exception exception);

        [LoggerMessage(3, LogLevel.Warning, "Previous fetch failed, attempt: {attempt}")]
        private static partial void LogFetchAttemptsRetry(ILogger logger, int attempt);

        [LoggerMessage(4, LogLevel.Information, "Refreshing global deny list")]
        private static partial void LogRefresh(ILogger logger);
    }
}
