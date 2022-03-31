using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core;
using UrlTracker.Core.Models;
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

        public ProcessedEventSubscriber(IClientErrorService clientErrorService,
                                        IClientErrorFilterCollection clientErrorFilterCollection,
                                        ILogger logger)
        {
            _clientErrorService = clientErrorService;
            _clientErrorFilterCollection = clientErrorFilterCollection;
            _logger = logger;
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

            if (!await _clientErrorFilterCollection.EvaluateCandidacyAsync(args.HttpContext))
            {
                _logger.LogAbortClientErrorHandling<ProcessedEventSubscriber>("Incoming request failed client error candidacy check");
                return;
            }

            NotFound notFound = CreateNotFound(args);
            args.HttpContext.Items.Add(_cacheKey, notFound);

            await _clientErrorService.AddAsync(notFound);
        }

        [ExcludeFromCodeCoverage]
        private static NotFound CreateNotFound(ProcessedEventArgs args)
        {
            return new NotFound
            {
                Url = args.HttpContext.Request.Url.AbsoluteUri,
                Referrer = args.HttpContext.Request.UrlReferrer?.AbsoluteUri
            };
        }
    }
}
