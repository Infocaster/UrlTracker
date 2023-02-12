import '@umbraco-ui/uui';
import './dashboard/dashboard.lit'
import './dashboard/footer/dashboardFooter.lit'
import './dashboard/notifications/notification.lit'
import { UrlTrackerDashboardController, ngUrltrackerDashboard } from './dashboard/dashboard.directive'
import { VersionProvider } from './util/versionProvider'
import { NotificationResource, NotificationService } from './dashboard/notifications/notification.service';
import { UrlTrackerNotificationController, ngUrlTrackerNotification } from './dashboard/notifications/notification.directive';

const module = angular.module("umbraco");

// controllers
module.controller(UrlTrackerDashboardController.alias, UrlTrackerDashboardController);
module.controller(UrlTrackerNotificationController.alias, UrlTrackerNotificationController);

// directives
module.directive(ngUrltrackerDashboard.alias, ngUrltrackerDashboard);
module.directive(ngUrlTrackerNotification.alias, ngUrlTrackerNotification);

// services
module.service(VersionProvider.alias, VersionProvider);
module.service(NotificationService.alias, NotificationService);
module.service(NotificationResource.alias, NotificationResource);