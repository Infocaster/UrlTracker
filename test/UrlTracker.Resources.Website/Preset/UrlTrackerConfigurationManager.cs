using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Resources.Website.Preset
{
    public interface IUrlTrackerConfigurationManager
    {
        UrlTrackerSettings Configuration { get; set; }
    }

    public class UrlTrackerConfigurationManager : IUrlTrackerConfigurationManager
    {
        public UrlTrackerSettings Configuration { get; set; }
    }
}