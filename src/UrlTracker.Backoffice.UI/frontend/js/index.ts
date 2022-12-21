import '@umbraco-ui/uui';
import './dashboard/dashboard.lit'
import './dashboard/dashboardFooter.lit'
import { UrlTrackerDashboardController } from './dashboard/dashboard.controller'

const module = angular.module("umbraco");

// controllers
module.controller("UrlTracker.Dashboard.Controller", UrlTrackerDashboardController);

// directives

// services