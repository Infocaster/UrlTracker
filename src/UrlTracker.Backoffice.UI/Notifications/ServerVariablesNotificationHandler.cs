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
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RecommendationsController>(controller => controller.List(default!))!,
                ["list"] = nameof(RecommendationsController.List)
            };

            Dictionary<string, string> redirectVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RedirectsController>(controller => controller.List(default!))!,
                ["list"] = nameof(RedirectsController.List),
                ["get"] = nameof(RedirectsController.Get),
                ["delete"] = nameof(RedirectsController.Delete),
                ["create"] = nameof(RedirectsController.Create),
                ["update"] = nameof(RedirectsController.Update)
            };

            Dictionary<string, string> notificationVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<NotificationsController>(controller => controller.Get(default!))!,
                ["get"] = nameof(NotificationsController.Get)
            };

            Dictionary<string, string> redirectSourceStrategies = new()
            {
                ["url"] = Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url.ToString(),
                ["regex"] = Core.Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression.ToString()
            };

            Dictionary<string, string> redirectTargetStrategies = new()
            {
                ["url"] = Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Url.ToString(),
                ["content"] = Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content.ToString(),
                ["media"] = Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Media.ToString()
            };

            Dictionary<string, object> urlTrackerVariables = new()
            {
                ["recommendations"] = recommendationVariables,
                ["notifications"] = notificationVariables,
                ["redirects"] = redirectVariables,
                ["redirectSourceStrategies"] = redirectSourceStrategies,
                ["redirectTargetStrategies"] = redirectTargetStrategies
            };

            notification.ServerVariables.Add("urlTracker", urlTrackerVariables);
        }
    }
}
