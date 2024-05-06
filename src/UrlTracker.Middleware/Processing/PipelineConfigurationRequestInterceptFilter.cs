using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Models;
using UrlTracker.Middleware.Options;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware.Processing
{
    /// <summary>
    /// An implementation of <see cref="IRequestInterceptFilter" /> that ensures intercept handling only when it is enabled by configuration
    /// </summary>
    public class PipelineConfigurationRequestInterceptFilter : IRequestInterceptFilter
    {
        private readonly IOptionsMonitor<UrlTrackerPipelineOptions> _options;

        /// <inheritdoc />
        public PipelineConfigurationRequestInterceptFilter(IOptionsMonitor<UrlTrackerPipelineOptions> options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public ValueTask<bool> EvaluateCandidateAsync(Url url)
            => new(EvaluateCandidate());

        private bool EvaluateCandidate()
        {
            var optionsValue = _options.CurrentValue;

            return optionsValue.Enable;
        }
    }
}
