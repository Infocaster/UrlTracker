using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI;
using UrlTracker.Backoffice.UI.Controllers;

namespace UrlTracker.Web.Events
{
    [ExcludeFromCodeCoverage] // Code is too simple
    internal class ServerVariablesNotificationHandler : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IUrltrackerVersionProvider _urltrackerVersionProvider;

        public ServerVariablesNotificationHandler(LinkGenerator linkGenerator, IUrltrackerVersionProvider urltrackerVersionProvider)
        {
            _linkGenerator = linkGenerator;
            _urltrackerVersionProvider = urltrackerVersionProvider;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            Dictionary<string, string> landingspageVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<LandingPageController>(controller => controller.GetNumericMetric())!,
                ["numericMetric"] = nameof(LandingPageController.GetNumericMetric),
            };

            Dictionary<string, string> recommendationVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RecommendationsController>(controller => controller.List(default!))!,
                ["list"] = nameof(RecommendationsController.List),
                ["update"] = nameof(RecommendationsController.Update),
                ["updateBulk"] = nameof(RecommendationsController.UpdateBulk),
                ["delete"] = nameof(RecommendationsController.Delete)
            };

            Dictionary<string, string> recommendationAnalysisVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RecommendationAnalysisController>(controller => controller.GetHistoryAsync(default, default))!,
                ["getHistory"] = "GetHistory",
                ["getReferrers"] = "GetReferrers"
            };

            Dictionary<string, string> redirectVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RedirectsController>(controller => controller.List(default!))!,
                ["list"] = nameof(RedirectsController.List),
                ["get"] = nameof(RedirectsController.Get),
                ["delete"] = nameof(RedirectsController.Delete),
                ["create"] = nameof(RedirectsController.Create),
                ["update"] = nameof(RedirectsController.Update),
                ["updateBulk"] = nameof(RedirectsController.UpdateBulk),
                ["deleteBulk"] = nameof(RedirectsController.DeleteBulk)
            };

            Dictionary<string, string> redirectImportVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RedirectImportController>(controller => controller.Content(default!))!,
                ["import"] = "Import",
                ["export"] = "Export",
                ["exportTemplate"] = "ExportExample"
            };

            Dictionary<string, string> redirectTargetVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<RedirectTargetController>(controller => controller.Content(default!))!,
                ["content"] = nameof(RedirectTargetController.Content)
            };

            Dictionary<string, string> notificationVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<NotificationsController>(controller => controller.Get(default!))!,
                ["get"] = nameof(NotificationsController.Get)
            };

            Dictionary<string, string> scoringVariables = new()
            {
                ["base"] = _linkGenerator.GetUmbracoApiServiceBaseUrl<ScoringController>(controller => controller.RedactionScores())!,
                ["redactionScores"] = nameof(ScoringController.RedactionScores),
                ["scoreParameters"] = nameof(ScoringController.ScoreParameters)
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

            Dictionary<string, string> recommendationTypeStrategies = new()
            {
                ["image"] = Core.Defaults.DatabaseSchema.RedactionScores.Media.ToString(),
                ["file"] = Core.Defaults.DatabaseSchema.RedactionScores.File.ToString(),
                ["page"] = Core.Defaults.DatabaseSchema.RedactionScores.Page.ToString(),
                ["technicalFile"] = Core.Defaults.DatabaseSchema.RedactionScores.TechnicalFile.ToString(),
            };

            Dictionary<string, object> urlTrackerVariables = new()
            {
                ["landingspage"] = landingspageVariables,
                ["recommendations"] = recommendationVariables,
                ["recommendationAnalysis"] = recommendationAnalysisVariables,
                ["recommendationTypeStrategies"] = recommendationTypeStrategies,
                ["notifications"] = notificationVariables,
                ["scoring"] = scoringVariables,
                ["redirects"] = redirectVariables,
                ["redirectimport"] = redirectImportVariables,
                ["redirectTarget"] = redirectTargetVariables,
                ["redirectSourceStrategies"] = redirectSourceStrategies,
                ["redirectTargetStrategies"] = redirectTargetStrategies,
                ["version"] = _urltrackerVersionProvider.GetCurrentVersion()
            };

            notification.ServerVariables.Add("urlTracker", urlTrackerVariables);
        }
    }
}
