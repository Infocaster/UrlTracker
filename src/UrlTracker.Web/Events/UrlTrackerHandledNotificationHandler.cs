using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using UrlTracker.Core;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Events
{
    public class UrlTrackerHandledNotificationHandler
        : INotificationAsyncHandler<UrlTrackerHandled>
    {
        private readonly IClientErrorService _clientErrorService;
        private readonly IClientErrorFilterCollection _clientErrorFilterCollection;
        private readonly ILogger<UrlTrackerHandledNotificationHandler> _logger;
        private readonly IRequestAbstraction _requestAbstraction;

        public UrlTrackerHandledNotificationHandler(IClientErrorService clientErrorService,
                                                    IClientErrorFilterCollection clientErrorFilterCollection,
                                                    ILogger<UrlTrackerHandledNotificationHandler> logger,
                                                    IRequestAbstraction requestAbstraction)
        {
            _clientErrorService = clientErrorService;
            _clientErrorFilterCollection = clientErrorFilterCollection;
            _logger = logger;
            _requestAbstraction = requestAbstraction;
        }

        public async Task HandleAsync(UrlTrackerHandled notification, CancellationToken token)
        {
            if (notification.HttpContext.Response.StatusCode != 404)
            {
                _logger.LogAbortClientErrorHandling("Response is not 404");
                return;
            }

            if (!await _clientErrorFilterCollection.EvaluateCandidacyAsync(notification))
            {
                _logger.LogAbortClientErrorHandling("Incoming request failed client error candidacy check");
                return;
            }

            await _clientErrorService.AddAsync(CreateNotFound(notification));
        }

        [ExcludeFromCodeCoverage]
        private NotFound CreateNotFound(UrlTrackerHandled args)
        {
            return new NotFound(args.Url.ToString())
            {
                Referrer = args.HttpContext.Request.GetReferrer(_requestAbstraction)?.AbsoluteUri
            };
        }
    }
}
