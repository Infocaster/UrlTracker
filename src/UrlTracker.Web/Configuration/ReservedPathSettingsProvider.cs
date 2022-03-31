using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;

namespace UrlTracker.Web.Configuration
{

    public class ReservedPathSettingsProvider : IReservedPathSettingsProvider
    {
        private readonly IOptions<GlobalSettings> _globalSettings;
        private HashSet<string>? _paths = null;

        public ReservedPathSettingsProvider(IOptions<GlobalSettings> globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public HashSet<string> Paths
        {
            get
            {
                // reserved paths are defined by umbraco in the global settings
                if (_paths is not null) return _paths;

                var globalSettingsValue = _globalSettings.Value;
                return _paths = new HashSet<string>(from path in globalSettingsValue.ReservedPaths.Split(',').Concat(globalSettingsValue.ReservedUrls.Split(','))
                                                    where !string.IsNullOrWhiteSpace(path)
                                                    select path.Trim(' ', '~', '/', '\\') + '/');
            }
        }
    }
}
