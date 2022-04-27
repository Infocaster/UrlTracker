using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Core.Intercepting
{
    public class NoLongerExistsInterceptor
        : IInterceptor
    {
        private readonly IClientErrorRepository _clientErrorRepository;
        private readonly IStaticUrlProviderCollection _staticUrlProviders;
        private readonly ILogger _logger;

        public NoLongerExistsInterceptor(IClientErrorRepository notFoundRepository,
                                         IStaticUrlProviderCollection staticUrlProviders,
                                         ILogger logger)
        {
            _clientErrorRepository = notFoundRepository;
            _staticUrlProviders = staticUrlProviders;
            _logger = logger;
        }

        public async ValueTask<ICachableIntercept> InterceptAsync(Url url, IReadOnlyInterceptContext context)
        {
            var urls = _staticUrlProviders.GetUrls(url);

            var rootNodeId = context.GetRootNode();
            var culture = context.GetCulture();

            _logger.LogParameters<NoLongerExistsInterceptor>(culture, rootNodeId, urls.ToList());

            var results = await _clientErrorRepository.GetShallowAsync(urls, rootNodeId, culture);
            _logger.LogResults<NoLongerExistsInterceptor>(results.Count);

            return GetBestResult(results);
        }

        private ICachableIntercept GetBestResult(IReadOnlyCollection<UrlTrackerShallowClientError> results)
        {
            var bestResult = results.FirstOrDefault(r => r.TargetStatusCode == HttpStatusCode.Gone);
            return !(bestResult is null) ? new CachableInterceptBase<UrlTrackerShallowClientError>(bestResult) : null;
        }
    }
}
