import { ngUrltrackerDashboard } from './dashboard/directive'
import '@umbraco-ui/uui';
import './dashboard/main.lit'
import './dashboard/notifications/notification.lit'
import './dashboard/tabs/redirects.lit';
import './dashboard/tabs/landingpage.lit';
import './dashboard/tabs/recommendations.lit';
import './dashboard/tabs/advancedredirects.lit';
import './dashboard';
import './dashboard/tabs/redirects/source';
import './dashboard/tabs/redirects/target';


const module = angular.module("umbraco");

// directives
module.directive(ngUrltrackerDashboard.alias, ngUrltrackerDashboard);