using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;

namespace UrlTracker.Web.Configuration
{

    public class ReservedPathSettingsProvider : IReservedPathSettingsProvider
    {
        private readonly IOptions<GlobalSettings> _globalSettings;
        private Lazy<HashSet<string>> _lazyPaths;

        public ReservedPathSettingsProvider(IOptions<GlobalSettings> globalSettings)
        {
            _globalSettings = globalSettings;
            _lazyPaths = new Lazy<HashSet<string>>(Create);
        }

        public HashSet<string> Paths
            => _lazyPaths.Value;

        private HashSet<string> Create()
        {
            var globalSettingsValue = _globalSettings.Value;
            return new HashSet<string>(from path in globalSettingsValue.ReservedPaths.Split(',').Concat(globalSettingsValue.ReservedUrls.Split(','))
                                       where !string.IsNullOrWhiteSpace(path)
                                       select path.Trim(' ', '~', '/', '\\') + '/');
        }
    }
}
