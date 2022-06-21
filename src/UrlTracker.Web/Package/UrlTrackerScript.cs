using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.WebAssets;

namespace UrlTracker.Web.Package
{
    [ExcludeFromCodeCoverage]
    internal class UrlTrackerScript
        : JavaScriptFile
    {
        public UrlTrackerScript()
            : base("/App_Plugins/UrlTracker/script.bundle.js")
        { }
    }
}
