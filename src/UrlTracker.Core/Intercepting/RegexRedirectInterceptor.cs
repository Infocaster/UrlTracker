using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Core.Intercepting
{
    public class RegexRedirectInterceptor
        : IInterceptor
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly ILogger _logger;

        public RegexRedirectInterceptor(IRedirectRepository redirectRepository,
                                        ILogger logger)
        {
            _redirectRepository = redirectRepository;
            _logger = logger;
        }

        public async ValueTask<ICachableIntercept> InterceptAsync(Url url, IReadOnlyInterceptContext context)
        {
            var regexRedirects = await _redirectRepository.GetShallowWithRegexAsync();

            // There may be multiple regexes for which the given url has an intercept. There is no way to tell which intercept is the best,
            //    so we just take the first intercept that we can find.
            int? rootNodeId = context.GetRootNode();

            string interceptString = url.Path.Trim('/');
            if (url.Query != null) interceptString += "?" + url.Query;

            foreach (var redirect in regexRedirects)
            {
                if ((rootNodeId == null
                    || redirect.TargetRootNodeId == null
                    || redirect.TargetRootNodeId == -1
                    || redirect.TargetRootNodeId == rootNodeId)
                   && Regex.IsMatch(interceptString, redirect.SourceRegex, RegexOptions.IgnoreCase))
                {
                    _logger.LogResults<RegexRedirectInterceptor>(1);
                    return new CachableInterceptBase<UrlTrackerShallowRedirect>(redirect);
                }
            }

            _logger.LogResults<RegexRedirectInterceptor>(0);
            return null;
        }
    }
}
