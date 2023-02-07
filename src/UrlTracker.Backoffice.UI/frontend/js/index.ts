import '@umbraco-ui/uui';
import './dashboard/dashboard.lit'
import './dashboard/footer/dashboardFooter.lit'
import { UrlTrackerDashboardController, ngUrltrackerDashboard } from './dashboard/dashboard.directive'
import { VersionProvider } from './util/versionProvider'

const module = angular.module("umbraco");

// controllers
module.controller(UrlTrackerDashboardController.alias, UrlTrackerDashboardController);

// directives
module.directive(ngUrltrackerDashboard.alias, ngUrltrackerDashboard);

// services
module.service(VersionProvider.alias, VersionProvider);