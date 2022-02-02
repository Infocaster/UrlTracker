using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Core.Configuration
{
    // This class replaces the old UrlTrackerSettings: https://dev.azure.com/infocaster/Umbraco%20Awesome/_git/UrlTracker?path=/Settings/UrlTrackerSettings.cs
    // By passing the configurations into the constructor, we prevent other code from unintentionally editting the configuration.
    //    The settings-as-an-object pattern also makes it easier to port this code to .NET 5
    // I don't like it yet, the configuration model uses some awkward constants and some non-serializable objects, which are taken from the old code
    public class UrlTrackerConfigurationProvider
        : IConfiguration<UrlTrackerSettings>
    {
        private const string _prefix = "urlTracker:";
        private readonly IAppSettingsAbstraction _appSettingsAbstraction;

        public UrlTrackerConfigurationProvider(IAppSettingsAbstraction appSettingsAbstraction)
        {
            _appSettingsAbstraction = appSettingsAbstraction;
        }

        public UrlTrackerSettings Value
        {
            get
            {
                bool disabled = GetBooleanSetting("disabled");
                bool trackingDisabled = GetBooleanSetting("trackingDisabled");
                bool notFoundTrackingDisabled = GetBooleanSetting("notFoundTrackingDisabled");
                bool loggingEnabled = GetBooleanSetting("enableLogging");
                bool appendPortNumber = GetBooleanSetting("appendPortNumber");
                bool hasDomainOnChildNode = GetBooleanSetting("hasDomainOnChildNode");

                return new UrlTrackerSettings(disabled,
                                              trackingDisabled,
                                              loggingEnabled,
                                              notFoundTrackingDisabled,
                                              appendPortNumber,
                                              hasDomainOnChildNode);
            }
        }

        private bool GetBooleanSetting(string name, bool @default = false)
        {
            return bool.TryParse(GetAppSetting(name), out var result) ? result : @default;
        }

        private string GetAppSetting(string name)
        {
            return _appSettingsAbstraction.Get(_prefix + name);
        }
    }
}
