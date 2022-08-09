using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models.Entities;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using ILogger = UrlTracker.Core.Logging.ILogger<UrlTracker.Core.Intercepting.StaticUrlRedirectInterceptor>;

namespace UrlTracker.Core.Intercepting
{
    public class StaticUrlRedirectInterceptor
        : IInterceptor
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IStaticUrlProviderCollection _urlProviderCollection;
        private readonly ILogger _logger;

        public StaticUrlRedirectInterceptor(IRedirectRepository redirectRepository,
                                            IStaticUrlProviderCollection urlProviderCollection,
                                            ILogger logger)
        {
            _redirectRepository = redirectRepository;
            _urlProviderCollection = urlProviderCollection;
            _logger = logger;
        }

        public async ValueTask<ICachableIntercept?> InterceptAsync(Url url, IReadOnlyInterceptContext context)
        {
            var interceptStrings = _urlProviderCollection.GetUrls(url);

            var culture = context.GetCulture();
            var rootNode = context.GetRootNode();

            var results = await _redirectRepository.GetAsync(interceptStrings, rootNode, culture);
            _logger.LogResults<StaticUrlRedirectInterceptor>(results.Count);

            return GetBestIntercept(results, url, context);
        }

        protected virtual CachableInterceptBase<IRedirect>? GetBestIntercept(IReadOnlyCollection<IRedirect> intercepts, Url url, IReadOnlyInterceptContext context)
        {
            // return first intercept by default
            IRedirect? bestIntercept = intercepts.FirstOrDefault();
            return bestIntercept is not null ? new CachableInterceptBase<IRedirect>(bestIntercept) : null;
        }
    }
}
