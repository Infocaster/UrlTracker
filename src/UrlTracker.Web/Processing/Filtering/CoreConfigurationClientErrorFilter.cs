using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Web.Processing.Filtering
{
    /// <summary>
    /// An implementation of <see cref="IClientErrorFilter" /> that ensures client error tracking only when it is enabled by configuration
    /// </summary>
    public class CoreConfigurationClientErrorFilter : IClientErrorFilter
    {
        private readonly IOptionsMonitor<UrlTrackerSettings> _options;

        /// <inheritdoc />
        public CoreConfigurationClientErrorFilter(IOptionsMonitor<UrlTrackerSettings> options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => new(EvaluateCandidate());

        private bool EvaluateCandidate()
        {
            var optionsValue = _options.CurrentValue;

            return optionsValue.Enable;
        }
    }
}
