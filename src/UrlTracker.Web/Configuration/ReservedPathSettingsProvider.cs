using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Configuration;
using UrlTracker.Core.Configuration;
using UrlTracker.Web.Configuration.Models;

namespace UrlTracker.Web.Configuration
{
    public class ReservedPathSettingsProvider
        : IConfiguration<ReservedPathSettings>
    {
        private readonly IGlobalSettings _globalSettings;

        public ReservedPathSettingsProvider(IGlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public ReservedPathSettings Value
        {
            get
            {
                // reserved paths are defined by umbraco in the global settings
                var paths = _globalSettings.ReservedPaths.Split(',').Concat(_globalSettings.ReservedUrls.Split(','));

                return new ReservedPathSettings
                {
                    Paths = new HashSet<string>(paths)
                };
            }
        }
    }
}
