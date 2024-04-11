using UrlTracker.Core;
using UrlTracker.Middleware.Background;

namespace UrlTracker.IntegrationTests.Utils
{
    internal class QueuelessClientErrorHandler
        : IClientErrorProcessorQueue
    {
        private readonly IClientErrorService _clientErrorService;

        public QueuelessClientErrorHandler(IClientErrorService clientErrorService)
        {
            _clientErrorService = clientErrorService;
        }

        public async ValueTask<ClientErrorProcessorItem> ReadAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(-1, cancellationToken);
            throw new InvalidOperationException("You're not supposed to get here");
        }

        public ValueTask WriteAsync(ClientErrorProcessorItem item)
        {
            return new ValueTask(_clientErrorService.ReportAsync(item.Url, item.Moment, item.Referrer));
        }
    }
}
