using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
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

        public async ValueTask<ICachableIntercept?> InterceptAsync(Url url, IReadOnlyInterceptContext context)
        {
            var urls = _staticUrlProviders.GetUrls(url);

            var rootNodeId = context.GetRootNode();
            var culture = context.GetCulture();

            _logger.LogParameters(culture, rootNodeId, urls.ToList());

            var results = await _clientErrorRepository.GetAsync(urls);
            _logger.LogResults<NoLongerExistsInterceptor>(results.Count);

            return GetBestResult(results);
        }

        private static ICachableIntercept? GetBestResult(IReadOnlyCollection<IClientError> results)
        {
            var bestResult = results.FirstOrDefault(r => r.Strategy == Defaults.DatabaseSchema.ClientErrorStrategies.NoLongerExists);
            return bestResult is not null ? new CachableInterceptBase<IClientError>(bestResult) : null;
        }
    }
}
