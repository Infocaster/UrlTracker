using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrlTracker.Core.Logging;

namespace UrlTracker.Backoffice.Notifications.Cleanup
{
    internal class CleanupScheduler : BackgroundService
    {
        private readonly CleanupQueue _queue;
        private readonly Core.Logging.ILogger<CleanupScheduler> _logger;

        public CleanupScheduler(CleanupQueue queue, Core.Logging.ILogger<CleanupScheduler> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var timer = new PeriodicTimer(TimeSpan.FromHours(2));
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await _queue.ScheduleAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Deliberately left empty
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cleanup scheduling is discontinued due to an unexpected error.");
            }
        }
    }
}
