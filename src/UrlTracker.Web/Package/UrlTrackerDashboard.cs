using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Dashboards;

namespace UrlTracker.Web.Package
{
    [Weight(15)]
    [ExcludeFromCodeCoverage]
    internal class UrlTrackerDashboard : IDashboard
    {
        public string[] Sections => new[] { Constants.Applications.Content };
        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
        public string Alias => "UrlTracker";
        public string View => "/App_Plugins/UrlTracker/Dashboards/overview.html";
    }
}
