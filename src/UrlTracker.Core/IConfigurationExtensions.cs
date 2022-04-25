using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Core
{
    public static class IConfigurationExtensions
    {
        [ExcludeFromCodeCoverage]
        public static UrlTrackerSettings GetUrlTrackerSettings(this IConfiguration configuration)
        {
            var settings = new UrlTrackerSettings();
            configuration.GetSection(Defaults.Options.UrlTrackerSection).Bind(settings);
            return settings;
        }
    }
}
