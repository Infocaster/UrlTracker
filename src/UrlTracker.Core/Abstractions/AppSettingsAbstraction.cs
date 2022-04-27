using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class AppSettingsAbstraction
        : IAppSettingsAbstraction
    {
        public string Get(string name)
            => ConfigurationManager.AppSettings[name];
    }
}
