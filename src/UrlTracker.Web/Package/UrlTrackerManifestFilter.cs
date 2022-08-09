using System;
using System.Collections.Generic;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace UrlTracker.Web.Package
{
    internal class UrlTrackerManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest
            {
                PackageName = "URL Tracker",
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? String.Empty
            });
        }
    }
}
