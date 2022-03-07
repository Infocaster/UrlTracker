using System.Collections.Generic;

namespace UrlTracker.Web.Configuration
{
    public interface IReservedPathSettingsProvider
    {
        HashSet<string> Paths { get; }
    }
}
