using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using ILogger = UrlTracker.Core.Logging.ILogger<UrlTracker.Core.Intercepting.NoLongerExistsInterceptor>;

namespace UrlTracker.Core.Intercepting
{
    public class NoLongerExistsInterceptor
        : IInterceptor
    {
        private readonly IClientErrorRepository _clientErrorRepository;
        private readonly IStaticUrlProviderCollection _staticUrlProviders;
        private readonly ILogger _logger;

        public NoLongerExistsInterceptor(IClientErrorRepository clientErrorRepository,
                                         IStaticUrlProviderCollection staticUrlProviders,
                                         ILogger logger)
        {
            _clientErrorRepository = clientErrorRepository;
            _staticUrlProviders = staticUrlProviders;
            _logger = logger;
        }

        public async ValueTask<ICachableIntercept?> InterceptAsync(Url url, IInterceptContext context)
        {
            var urls = _staticUrlProviders.GetUrls(url);

            var results = await _clientErrorRepository.GetNoLongerExistsAsync(urls);
            _logger.LogResults(typeof(NoLongerExistsInterceptor), results.Count);

            return GetBestResult(results);
        }

        private static ICachableIntercept? GetBestResult(IReadOnlyCollection<IClientError> results)
        {
            var bestResult = results.FirstOrDefault(r => r.Strategy == Defaults.DatabaseSchema.ClientErrorStrategies.NoLongerExists);
            return bestResult is not null ? new CachableInterceptBase<IClientError>(bestResult) : null;
        }
    }
}
