using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAppSettingsAbstraction _appSettingsAbstraction;

        public UrlTrackerConfigurationProvider(IAppSettingsAbstraction appSettingsAbstraction)
        {
            _appSettingsAbstraction = appSettingsAbstraction;
        }

        public UrlTrackerSettings Value
        {
            get
            {
                bool disabled = GetBooleanSetting(Defaults.Configuration.Disabled);
                bool trackingDisabled = GetBooleanSetting(Defaults.Configuration.TrackingDisabled);
                bool notFoundTrackingDisabled = GetBooleanSetting(Defaults.Configuration.NotFoundTrackingDisabled);
                bool loggingEnabled = GetBooleanSetting(Defaults.Configuration.EnableLogging);
                bool appendPortNumber = GetBooleanSetting(Defaults.Configuration.AppendPortNumber);
                bool hasDomainOnChildNode = GetBooleanSetting(Defaults.Configuration.HasDomainOnChildNode);
                long maxCachedIntercepts = GetLongSetting(Defaults.Configuration.MaxCachedIntercepts, 5000);
                bool cacheRegexRedirects = GetBooleanSetting(Defaults.Configuration.CacheRegexRedirects);
                int? interceptSlidingCacheMinutes = GetMaybeIntSetting(Defaults.Configuration.InterceptSlidingCacheMinutes, 60 * 24 * 2);
                bool enableInterceptCaching = GetBooleanSetting(Defaults.Configuration.EnableInterceptCaching, true);
                List<string> blockedUrlsList = GetStringListSetting(Defaults.Configuration.BlockedUrlsList);

                return new UrlTrackerSettings(disabled,
                                              trackingDisabled,
                                              loggingEnabled,
                                              notFoundTrackingDisabled,
                                              appendPortNumber,
                                              hasDomainOnChildNode,
                                              maxCachedIntercepts,
                                              cacheRegexRedirects,
                                              interceptSlidingCacheMinutes,
                                              enableInterceptCaching,
                                              blockedUrlsList);
            }
        }

        private List<string> GetStringListSetting(string name)
        {
            //get the items as string and remove all whitespaces
            var appSetting = GetAppSetting(name);
            if (string.IsNullOrEmpty(appSetting)) return new List<string>();
            return appSetting
                .Replace(" ", String.Empty)
                .Split(',')
                .ToList();
        }

        private bool GetBooleanSetting(string name, bool @default = false)
        {
            return bool.TryParse(GetAppSetting(name), out var result) ? result : @default;
        }

        private long GetLongSetting(string name, long @default = 0)
        {
            return long.TryParse(GetAppSetting(name), out var result) ? result : @default;
        }

        private int? GetMaybeIntSetting(string name, int? @default = null)
        {
            string stringValue = GetAppSetting(name);
            if ("null".Equals(stringValue, StringComparison.InvariantCultureIgnoreCase)) return null;
            return int.TryParse(stringValue, out var result) ? result : @default;
        }

        private string GetAppSetting(string name)
        {
            return _appSettingsAbstraction.Get(Defaults.Configuration.Prefix + name);
        }
    }
}
