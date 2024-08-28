using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core;

namespace UrlTracker.Web.Processing.Filtering
{
    /// <summary>
    /// An implementation of <see cref="IClientErrorFilter" /> that prevents registration of client errors on urls with predefined paths
    /// </summary>
    public class ConstantsClientErrorFilter
        : IClientErrorFilter
    {
        /// <inheritdoc />
        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => new(EvaluateCandidate(context));

        private static bool EvaluateCandidate(HttpContext context)
        {
            // absolute path starts with /, so patterns should also take that into account
            if (Defaults.Tracking.IgnoredUrlPaths.Any(rx
                => rx.IsMatch(context.Request.GetUrl().Path!))) return false;

            return true;
        }
    }
}
