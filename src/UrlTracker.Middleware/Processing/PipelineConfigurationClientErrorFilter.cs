using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UrlTracker.Middleware.Options;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware.Processing
{
    /// <summary>
    /// An implementation of <see cref="IClientErrorFilter" /> that ensures registration only when it is enabled by configuration
    /// </summary>
    public class PipelineConfigurationClientErrorFilter : IClientErrorFilter
    {
        private readonly IOptionsMonitor<UrlTrackerPipelineOptions> _options;

        /// <inheritdoc />
        public PipelineConfigurationClientErrorFilter(IOptionsMonitor<UrlTrackerPipelineOptions> options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => new(EvaluateCandidate());

        private bool EvaluateCandidate()
        {
            var optionsValue = _options.CurrentValue;

            return optionsValue.Enable && optionsValue.EnableClientErrorTracking;
        }
    }
}
