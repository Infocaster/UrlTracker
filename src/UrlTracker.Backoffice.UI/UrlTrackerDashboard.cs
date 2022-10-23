using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;

namespace UrlTracker.Backoffice.UI
{
    /// <summary>
    /// The URL Tracker management interface in the content section of Umbraco
    /// </summary>
    [Weight(15)]
    [ExcludeFromCodeCoverage]
    public class UrlTrackerDashboard : IDashboard
    {
        /// <inheritdoc/>
        public virtual string[] Sections => new[] { Constants.Applications.Content };

        /// <inheritdoc/>
        public virtual IAccessRule[] AccessRules => Array.Empty<IAccessRule>();

        /// <inheritdoc/>
        public virtual string Alias => "UrlTracker";

        /// <inheritdoc/>
        public virtual string View => "/App_Plugins/UrlTracker/dashboard/dashboard.html";
    }
}
