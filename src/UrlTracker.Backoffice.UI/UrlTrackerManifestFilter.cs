using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace UrlTracker.Backoffice.UI
{
    [ExcludeFromCodeCoverage]
    internal class UrlTrackerManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest
            {
                PackageName = "URL Tracker",
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? string.Empty
            });
        }
    }
}
