using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.Notifications.Cleanup
{
    internal interface ICleanupProcessor
    {
        Task ProcessAsync(CancellationToken stoppingToken);
    }

    internal class CleanupProcessor : ICleanupProcessor
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IClientErrorService _clientErrorService;
        private readonly IScopeProvider _scopeProvider;

        public CleanupProcessor(
            IRecommendationService recommendationService,
            IClientErrorService clientErrorService,
            IScopeProvider scopeProvider)
        {
            _recommendationService = recommendationService;
            _clientErrorService = clientErrorService;
            _scopeProvider = scopeProvider;
        }

        public async Task ProcessAsync(CancellationToken stoppingToken)
        {
            /* BUSINESS RULES:
             * - Every client error older than a month is deleted
             * - Every recommendation with extremely low score is deleted, except if it's ignored (otherwise it might return)
             */
            using var scope = _scopeProvider.CreateScope();

            var upperDate = DateTime.Now.AddMonths(-1);
            await _clientErrorService.CleanupAsync(upperDate);
            stoppingToken.ThrowIfCancellationRequested();

            await _recommendationService.CleanupAsync(0.0005);

            scope.Complete();
        }
    }
}
