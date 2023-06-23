import '@umbraco-ui/uui';
import './dashboard/main.lit'
import './dashboard/notifications/notification.lit'
import { ngUrltrackerDashboard } from './dashboard/directive'
import './dashboard/tabs/redirects.lit';
import './dashboard/tabs/landingpage.lit';
import './dashboard/tabs/recommendations.lit';
import './dashboard/tabs/advancedredirects.lit';

const module = angular.module("umbraco");

// directives
module.directive(ngUrltrackerDashboard.alias, ngUrltrackerDashboard);