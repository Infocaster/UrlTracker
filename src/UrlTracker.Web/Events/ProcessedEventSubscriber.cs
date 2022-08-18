using System;
using System.Threading.Tasks;
using Umbraco.Core.Configuration.UmbracoSettings;
using UrlTracker.Core;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Processing;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Events
{
    public class ProcessedEventSubscriber
        : IEventSubscriber<object, ProcessedEventArgs>
    {
        private const string _cacheKey = "RegisteredNotFound";
        private readonly IClientErrorService _clientErrorService;
        private readonly IClientErrorFilterCollection _clientErrorFilterCollection;
        private readonly ILogger _logger;
        private readonly IUmbracoSettingsSection _umbracoSettings;

        public ProcessedEventSubscriber(IClientErrorService clientErrorService,
                                        IClientErrorFilterCollection clientErrorFilterCollection,
                                        ILogger logger,
                                        IUmbracoSettingsSection umbracoSettings)
        {
            _clientErrorService = clientErrorService;
            _clientErrorFilterCollection = clientErrorFilterCollection;
            _logger = logger;
            _umbracoSettings = umbracoSettings;
        }

        public async Task HandleAsync(object source, ProcessedEventArgs args)
        {
            if (args.HttpContext.Response.StatusCode != 404)
            {
                _logger.LogAbortClientErrorHandling<ProcessedEventSubscriber>("Response is not 404");
                return;
            }

            if (!(args.HttpContext.Items[_cacheKey] is null))
            {
                _logger.LogAbortClientErrorHandling<ProcessedEventSubscriber>("Client error has already been processed in this request.");
                return;
            }

            if (!await _clientErrorFilterCollection.EvaluateCandidacyAsync(args))
            {
                _logger.LogAbortClientErrorHandling<ProcessedEventSubscriber>("Incoming request failed client error candidacy check");
                return;
            }

            args.HttpContext.Items.Add(_cacheKey, new object());

            var url = args.Url.ToString(UrlType.Absolute, _umbracoSettings.RequestHandler.AddTrailingSlash);
            var referrer = args.HttpContext.Request.UrlReferrer;
            await _clientErrorService.ReportAsync(url, DateTime.Now, referrer?.ToString());
        }
    }
}
