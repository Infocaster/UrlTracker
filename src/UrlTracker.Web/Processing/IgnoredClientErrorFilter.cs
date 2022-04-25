using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UrlTracker.Core;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public class IgnoredClientErrorFilter : IClientErrorFilter
    {
        private readonly ILegacyService _legacyService;

        public IgnoredClientErrorFilter(ILegacyService legacyService)
        {
            _legacyService = legacyService;
        }

        public async ValueTask<bool> EvaluateCandidateAsync(UrlTrackerHandled notification)
        {
            return !await _legacyService.IsIgnoredAsync(notification.Url.ToString());
        }
    }
}
