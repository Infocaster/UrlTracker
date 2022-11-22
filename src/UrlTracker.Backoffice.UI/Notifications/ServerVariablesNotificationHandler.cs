﻿using System.Collections.Generic;
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
            Dictionary<string, string?> urlTrackerVariables = new()
            {
                { "base", _linkGenerator.GetUmbracoApiServiceBaseUrl<UrlTrackerManagerController>(controller => controller.DeleteEntry(default!)) },
                { "deleteEntry", nameof(UrlTrackerManagerController.DeleteEntry) },
                { "getRedirects", nameof(UrlTrackerManagerController.GetRedirects) },
                { "getNotFounds", nameof(UrlTrackerManagerController.GetNotFounds) },
                { "countNotFoundsThisWeek", nameof(UrlTrackerManagerController.CountNotFoundsThisWeek) },
                { "addIgnore404", nameof(UrlTrackerManagerController.AddIgnore404) },
                { "importRedirects", nameof(UrlTrackerManagerController.ImportRedirects) },
                { "exportRedirects", nameof(UrlTrackerManagerController.ExportRedirects) }
            };

            notification.ServerVariables.Add("urlTracker", urlTrackerVariables);
        }
    }
}
