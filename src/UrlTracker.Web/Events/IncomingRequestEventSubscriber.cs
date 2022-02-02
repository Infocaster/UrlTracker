using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Processing;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Events
{
    public class IncomingRequestEventSubscriber
        : IEventSubscriber<object, IncomingRequestEventArgs>
    {
        private readonly ILogger _logger;
        private readonly IInterceptService _interceptService;
        private readonly IResponseInterceptHandlerCollection _interceptHandlers;
        private readonly IRequestInterceptFilterCollection _requestFilters;
        private readonly IEventPublisher<ProcessedEventArgs> _onProcessedEvent;

        public IncomingRequestEventSubscriber(ILogger logger,
                                              IInterceptService interceptService,
                                              IResponseInterceptHandlerCollection interceptHandlers,
                                              IRequestInterceptFilterCollection requestFilters,
                                              IEventPublisher<ProcessedEventArgs> onProcessedEvent)
        {
            _logger = logger;
            _interceptService = interceptService;
            _interceptHandlers = interceptHandlers;
            _requestFilters = requestFilters;
            _onProcessedEvent = onProcessedEvent;
        }

        public async Task HandleAsync(object source, IncomingRequestEventArgs args)
        {
            var context = args.HttpContext;
            Url url = Url.FromAbsoluteUri(context.Request.Url);
            _logger.LogRequestDetected<IncomingRequestEventSubscriber>(context.Request.Url.AbsoluteUri);

            await DoInterceptAsync(context, url);
            await _onProcessedEvent.PublishAsync(this, new ProcessedEventArgs(context));
        }

        public async Task DoInterceptAsync(HttpContextBase context, Url url)
        {
            if (!await _requestFilters.EvaluateCandidateAsync(url))
            {
                _logger.LogAbortHandling<IncomingRequestEventSubscriber>("Incoming request failed intercept candidacy check.");
                return;
            }

            var intercept = await _interceptService.GetAsync(url);
            if (intercept is null)
            {
                _logger.LogAbortHandling<IncomingRequestEventSubscriber>("No intercept was found");
                return;
            }

            _logger.LogInterceptFound<IncomingRequestEventSubscriber>(intercept.GetType());

            var handler = _interceptHandlers.Get(intercept);
            await handler.HandleAsync(context, intercept);
        }
    }
}
