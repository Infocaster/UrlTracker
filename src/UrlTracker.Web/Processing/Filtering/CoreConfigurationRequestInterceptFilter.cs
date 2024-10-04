using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing.Filtering
{
    /// <summary>
    /// An implementation of <see cref="IRequestInterceptFilter" /> that ensures intercept handling only when it is enabled by configuration
    /// </summary>
    public class CoreConfigurationRequestInterceptFilter : IRequestInterceptFilter
    {
        private readonly IOptionsMonitor<UrlTrackerSettings> _options;

        /// <inheritdoc />
        public CoreConfigurationRequestInterceptFilter(IOptionsMonitor<UrlTrackerSettings> options)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public ValueTask<bool> EvaluateCandidateAsync(Url url)
            => new(EvaluateCandidate());

        private bool EvaluateCandidate()
        {
            var optionsValue = _options.CurrentValue;

            return optionsValue.Enable;
        }
    }
}
