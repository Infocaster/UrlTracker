using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.WebAssets;

namespace UrlTracker.Backoffice.UI
{
    [ExcludeFromCodeCoverage]
    internal class UrlTrackerScript
        : JavaScriptFile
    {
        public UrlTrackerScript()
            : base(Defaults.Routing.AppPluginFolder + "script.iife.js")
        { }
    }
}
