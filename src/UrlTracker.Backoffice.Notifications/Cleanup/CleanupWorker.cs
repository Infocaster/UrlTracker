using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Logging;

namespace UrlTracker.Backoffice.Notifications.Cleanup
{
    internal class CleanupWorker : BackgroundService
    {
        private readonly Core.Logging.ILogger<CleanupWorker> _logger;
        private readonly CleanupQueue _queue;
        private readonly ICleanupProcessor _processor;
        private readonly IRuntimeState _runtimeState;

        public CleanupWorker(
            Core.Logging.ILogger<CleanupWorker> logger,
            CleanupQueue queue,
            ICleanupProcessor processor,
            IRuntimeState runtimeState)
        {
            _logger = logger;
            _queue = queue;
            _processor = processor;
            _runtimeState = runtimeState;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _queue.WaitForSignalAsync(stoppingToken);
                    if (_runtimeState.Level < Umbraco.Cms.Core.RuntimeLevel.Run)
                    {
                        // cleanup should wait until Umbraco is completely ready
                        continue;
                    }

                    await _processor.ProcessAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Deliberately left empty
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Something went wrong while handling cleanup in the background. Check the exception for more details");
                }
            }
        }
    }
}
