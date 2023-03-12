import '@umbraco-ui/uui';
import './dashboard/dashboard.lit'
import './dashboard/footer/dashboardFooter.lit'
import './dashboard/notifications/notification.lit'
import { UrlTrackerDashboardController, ngUrltrackerDashboard } from './dashboard/dashboard.directive'
import { VersionProvider } from './util/versionProvider'
import { NotificationResource, NotificationService } from './dashboard/notifications/notification.service';
import { UrlTrackerNotificationController, ngUrlTrackerNotification } from './dashboard/notifications/notification.directive';
import { ngUrlTrackerTabBase } from './dashboard/tabs/tabbase.directive';
import { UrlResource } from './util/UrlResource';
import { ngUrlTrackerLandingpageRecommendations, UrlTrackerLandingpageRecommendations } from './dashboard/tabs/landingpage/recommendations.directive';
import { RecommendationsService } from './services/Recommendations.service';

const module = angular.module("umbraco");

// controllers
module.controller(UrlTrackerDashboardController.alias, UrlTrackerDashboardController);
module.controller(UrlTrackerNotificationController.alias, UrlTrackerNotificationController);
module.controller(UrlTrackerLandingpageRecommendations.alias, UrlTrackerLandingpageRecommendations);

// directives
module.directive(ngUrltrackerDashboard.alias, ngUrltrackerDashboard);
module.directive(ngUrlTrackerNotification.alias, ngUrlTrackerNotification);
module.directive(ngUrlTrackerTabBase.alias, ngUrlTrackerTabBase);
module.directive(ngUrlTrackerLandingpageRecommendations.alias, ngUrlTrackerLandingpageRecommendations);

// services
module.service(VersionProvider.alias, VersionProvider);
module.service(NotificationService.alias, NotificationService);
module.service(NotificationResource.alias, NotificationResource);
module.service(UrlResource.alias, UrlResource);

module.service(RecommendationsService.alias, RecommendationsService);