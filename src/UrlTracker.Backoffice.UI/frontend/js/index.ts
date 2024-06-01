import '@oddbird/popover-polyfill';
import '@umbraco-ui/uui';
import './dashboard';
import {
  ngAnalyseRecommendationSidebar,
  ngInspectRecommendationsSidebar,
  ngInspectRedirectSidebar,
  ngSimpleRedirectSidebar,
  ngUrltrackerDashboard,
} from './dashboard/directive';
import './dashboard/main.lit';
import './dashboard/notifications/notification.lit';
import './dashboard/tabs/landingpage.lit';
import './dashboard/tabs/recommendations.lit';
import './dashboard/tabs/recommendations/recommendationType';
import './dashboard/tabs/redirects.lit';
import './dashboard/tabs/redirects/source';
import './dashboard/tabs/redirects/target';

import { TabBuilder } from './util/tools/builder/tabBuilder';

window.URL_TRACKER = {
  TabBuilder: new TabBuilder(),
};

const module = angular.module('umbraco');

// directives
module.directive(ngUrltrackerDashboard.alias, ngUrltrackerDashboard);
module.directive(ngSimpleRedirectSidebar.alias, ngSimpleRedirectSidebar);
module.directive(ngInspectRedirectSidebar.alias, ngInspectRedirectSidebar);
module.directive(ngInspectRecommendationsSidebar.alias, ngInspectRecommendationsSidebar);
module.directive(ngAnalyseRecommendationSidebar.alias, ngAnalyseRecommendationSidebar);
