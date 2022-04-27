using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Resources.Website.Preset
{
    public class DecoratorUrlTrackerConfigurationHijack : IConfiguration<UrlTrackerSettings>
    {
        private readonly IConfiguration<UrlTrackerSettings> _decoratee;
        private readonly IUrlTrackerConfigurationManager _configurationManager;

        public DecoratorUrlTrackerConfigurationHijack(IConfiguration<UrlTrackerSettings> decoratee, IUrlTrackerConfigurationManager configurationManager)
        {
            _decoratee = decoratee;
            _configurationManager = configurationManager;
        }

        public UrlTrackerSettings Value => _configurationManager.Configuration ?? _decoratee.Value;
    }
}