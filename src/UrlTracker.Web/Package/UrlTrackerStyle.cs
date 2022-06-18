using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.WebAssets;

namespace UrlTracker.Web.Package
{
    [ExcludeFromCodeCoverage]
    internal class UrlTrackerStyle
        : CssFile
    {
        public UrlTrackerStyle()
            : base("/App_Plugins/UrlTracker/style.css")
        { }
    }
}
