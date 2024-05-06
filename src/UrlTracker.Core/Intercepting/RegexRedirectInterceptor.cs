using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using ILogger = UrlTracker.Core.Logging.ILogger<UrlTracker.Core.Intercepting.RegexRedirectInterceptor>;

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

        public async ValueTask<ICachableIntercept?> InterceptAsync(Url url, IInterceptContext context)
        {
            var regexRedirects = await _redirectRepository.GetWithRegexAsync();

            // There may be multiple regexes for which the given url has an intercept. There is no way to tell which intercept is the best,
            //    so we just take the first intercept that we can find.
            string interceptString = url.Path!.Trim('/');
            if (url.Query is not null) interceptString += "?" + url.Query;

            foreach (var redirect in regexRedirects)
            {
                if (IsRegexMatch(interceptString, redirect.Source.Value))
                {
                    _logger.LogResults(typeof(RegexRedirectInterceptor), 1);
                    return new CachableInterceptBase<IRedirect>(redirect);
                }
            }

            _logger.LogResults(typeof(RegexRedirectInterceptor), 0);
            return null;
        }

        /// <summary>
        /// Tests if the intercepted string matches the regex of the redirect. 
        /// Exceptions are caught to avoid blowing up the request pipeline if
        /// the regex pattern is invalid.
        /// </summary>
        private static bool IsRegexMatch(string interceptString, string sourceRegex)
        {
            try
            {
                return Regex.IsMatch(interceptString, sourceRegex, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
