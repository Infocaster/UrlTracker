using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using UrlTracker.Core;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Logging;
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
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;
        private readonly IRequestAbstraction _requestAbstraction;

        public UrlTrackerHandledNotificationHandler(IClientErrorService clientErrorService,
                                                    IClientErrorFilterCollection clientErrorFilterCollection,
                                                    ILogger<UrlTrackerHandledNotificationHandler> logger,
                                                    IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions,
                                                    IRequestAbstraction requestAbstraction)
        {
            _clientErrorService = clientErrorService;
            _clientErrorFilterCollection = clientErrorFilterCollection;
            _logger = logger;
            _requestHandlerOptions = requestHandlerOptions;
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

            var requestHandlerOptions = _requestHandlerOptions.CurrentValue;
            var url = notification.Url.ToString(UrlType.Absolute, requestHandlerOptions.AddTrailingSlash);
            var referrer = notification.HttpContext.Request.GetReferrer(_requestAbstraction);
            await _clientErrorService.ReportAsync(url, DateTime.Now, referrer?.ToString());
        }
    }
}
