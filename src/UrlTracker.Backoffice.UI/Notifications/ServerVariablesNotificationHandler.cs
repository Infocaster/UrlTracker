using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Controllers;

namespace UrlTracker.Web.Events
{
    [ExcludeFromCodeCoverage] // Code is too simple
    internal class ServerVariablesNotificationHandler : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly LinkGenerator _linkGenerator;

        public ServerVariablesNotificationHandler(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            Dictionary<string, string> recommendationVariables = new()
            {
                { "base", _linkGenerator.GetUmbracoApiServiceBaseUrl<RecommendationsController>(controller => controller.List(default!))! },
                { "list", nameof(RecommendationsController.List) }
            };

            Dictionary<string, object> urlTrackerVariables = new()
            {
                { "recommendations", recommendationVariables }
            };

            notification.ServerVariables.Add("urlTracker", urlTrackerVariables);
        }
    }
}
