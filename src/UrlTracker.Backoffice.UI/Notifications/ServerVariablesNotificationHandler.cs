using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI;

namespace UrlTracker.Web.Events
{
    [ExcludeFromCodeCoverage] // Code is too simple
    internal class ServerVariablesNotificationHandler : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly IUrltrackerVersionProvider _urltrackerVersionProvider;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ServerVariablesNotificationHandler(
            IUrltrackerVersionProvider urltrackerVersionProvider,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _urltrackerVersionProvider = urltrackerVersionProvider;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {

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

            Dictionary<string, string> recommendationTypeStrategies = new()
            {
                ["image"] = Core.Defaults.DatabaseSchema.RedactionScores.Media.ToString(),
                ["file"] = Core.Defaults.DatabaseSchema.RedactionScores.File.ToString(),
                ["page"] = Core.Defaults.DatabaseSchema.RedactionScores.Page.ToString(),
                ["technicalFile"] = Core.Defaults.DatabaseSchema.RedactionScores.TechnicalFile.ToString(),
            };

            Dictionary<string, object> urlTrackerVariables = new()
            {
                ["recommendationTypeStrategies"] = recommendationTypeStrategies,
                ["redirectSourceStrategies"] = redirectSourceStrategies,
                ["redirectTargetStrategies"] = redirectTargetStrategies,
                ["version"] = _urltrackerVersionProvider.GetCurrentVersion()
            };

            urlTrackerVariables = IncludeRoutes(urlTrackerVariables);

            notification.ServerVariables.Add("urlTracker", urlTrackerVariables);
        }

        private Dictionary<string, object> IncludeRoutes(Dictionary<string, object> host)
        {
            var descriptors = _actionDescriptorCollectionProvider
                .ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .Where(cad => cad.ControllerTypeInfo.Assembly.Equals(Assembly.GetExecutingAssembly()))
                .GroupBy(cad => cad.ControllerName)
                .ToList();

            foreach (var group in descriptors)
            {
                Dictionary<string, string?> routes = new ()
                {
                    ["base"] = group.Key
                };

                foreach (var descriptor in group)
                {
                    routes.Add(descriptor.ActionName, descriptor.AttributeRouteInfo?.Template?.EnsureStartsWith('/'));
                }

                host.Add(group.Key, routes);
            }

            return host;
        }
    }
}
