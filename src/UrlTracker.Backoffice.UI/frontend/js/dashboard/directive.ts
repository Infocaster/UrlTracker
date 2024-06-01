import { scopeContext, scopeContextKey } from '@/context/scope.context';
import { IEditorService, editorServiceContext, editorServiceKey } from '../context/editorservice.context';
import { IIconHelper, iconHelperContext, iconHelperKey } from '../context/iconhelper.context';
import {
  ILocalizationService,
  localizationServiceContext,
  localizationServiceKey,
} from '../context/localizationservice.context';
import { UrlTrackerDashboard } from './main.lit';
import { AnalyseRecommendationSidebar } from './sidebars/analyseRecommendation-main.lit';
import { InspectRecommendationsSidebar } from './sidebars/explainRecommendations-main.lit';
import { InspectRedirectSidebar } from './sidebars/inspectRedirect-main.lit';
import { SimpleRedirectSidebar } from './sidebars/simpleRedirect-main.lit';
import { IOverlayService, overlayServiceContext, overlayServiceContextKey } from '@/context/overlayservice.context';
import {
  IUmbracoNotificationsService,
  umbracoNotificationsServiceContext,
  umbracoNotificationsServiceContextKey,
} from '@/context/notificationsservice.context';

ngUrltrackerDashboard.alias = 'ngUrltrackerDashboard';
ngUrltrackerDashboard.$inject = [
  'localizationService',
  'iconHelper',
  'editorService',
  'overlayService',
  'notificationsService',
];
export function ngUrltrackerDashboard(
  localizationService: ILocalizationService,
  iconHelper: IIconHelper,
  editorService: IEditorService<any>,
  overlayService: IOverlayService,
  notificationsService: IUmbracoNotificationsService,
): angular.IDirective {
  return {
    restrict: 'E',
    link: function (_scope, element) {
      const dashboardElement = document.createElement('urltracker-dashboard') as UrlTrackerDashboard;

      dashboardElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);

      dashboardElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
      dashboardElement.SetContext(editorService, editorServiceContext, editorServiceKey);
      dashboardElement.SetContext(_scope, scopeContext, scopeContextKey);
      dashboardElement.SetContext(overlayService, overlayServiceContext, overlayServiceContextKey);
      dashboardElement.SetContext(
        notificationsService,
        umbracoNotificationsServiceContext,
        umbracoNotificationsServiceContextKey,
      );

      element[0].appendChild(dashboardElement);
    },
  };
}

ngSimpleRedirectSidebar.alias = 'ngUrltrackerSimpleRedirectSidebar';
ngSimpleRedirectSidebar.$inject = ['localizationService', 'iconHelper', 'editorService', 'notificationsService'];

export function ngSimpleRedirectSidebar(
  localizationService: ILocalizationService,
  iconHelper: IIconHelper,
  editorService: IEditorService<any>,
  notificationsService: IUmbracoNotificationsService,
): angular.IDirective {
  return {
    restrict: 'E',
    link: function (_scope, element) {
      const redirectSidebarElement = document.createElement(
        'urltracker-simple-redirect-sidebar',
      ) as SimpleRedirectSidebar;

      redirectSidebarElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);

      redirectSidebarElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
      redirectSidebarElement.SetContext(editorService, editorServiceContext, editorServiceKey);
      redirectSidebarElement.SetContext(_scope, scopeContext, scopeContextKey);
      redirectSidebarElement.SetContext(
        notificationsService,
        umbracoNotificationsServiceContext,
        umbracoNotificationsServiceContextKey,
      );

      element[0].appendChild(redirectSidebarElement);
    },
  };
}

ngInspectRedirectSidebar.alias = 'ngUrltrackerInspectRedirectSidebar';
ngInspectRedirectSidebar.$inject = ['localizationService', 'iconHelper', 'editorService'];

export function ngInspectRedirectSidebar(
  localizationService: ILocalizationService,
  iconHelper: IIconHelper,
  editorService: IEditorService<any>,
): angular.IDirective {
  return {
    restrict: 'E',
    link: function (_scope, element) {
      const redirectSidebarElement = document.createElement(
        'urltracker-inspect-redirect-sidebar',
      ) as InspectRedirectSidebar;

      redirectSidebarElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);

      redirectSidebarElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
      redirectSidebarElement.SetContext(editorService, editorServiceContext, editorServiceKey);
      redirectSidebarElement.SetContext(_scope, scopeContext, scopeContextKey);

      element[0].appendChild(redirectSidebarElement);
    },
  };
}

ngInspectRecommendationsSidebar.alias = 'ngUrltrackerInspectRecommendationsSidebar';
ngInspectRecommendationsSidebar.$inject = ['localizationService', 'iconHelper', 'editorService'];

export function ngInspectRecommendationsSidebar(
  localizationService: ILocalizationService,
  iconHelper: IIconHelper,
  editorService: IEditorService<any>,
): angular.IDirective {
  return {
    restrict: 'E',
    link: function (_scope, element) {
      const redirectSidebarElement = document.createElement(
        'urltracker-inspect-recommendations-sidebar',
      ) as InspectRecommendationsSidebar;

      redirectSidebarElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);

      redirectSidebarElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
      redirectSidebarElement.SetContext(editorService, editorServiceContext, editorServiceKey);
      redirectSidebarElement.SetContext(_scope, scopeContext, scopeContextKey);

      element[0].appendChild(redirectSidebarElement);
    },
  };
}

ngAnalyseRecommendationSidebar.alias = 'ngUrltrackerAnalyseRecommendationSidebar';
ngAnalyseRecommendationSidebar.$inject = ['localizationService', 'iconHelper', 'editorService'];

export function ngAnalyseRecommendationSidebar(
  localizationService: ILocalizationService,
  iconHelper: IIconHelper,
  editorService: IEditorService<any>,
): angular.IDirective {
  return {
    restrict: 'E',
    link: function (_scope, element) {
      const redirectSidebarElement = document.createElement(
        'urltracker-analyse-recommendation-sidebar',
      ) as AnalyseRecommendationSidebar;

      redirectSidebarElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);

      redirectSidebarElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
      redirectSidebarElement.SetContext(editorService, editorServiceContext, editorServiceKey);
      redirectSidebarElement.SetContext(_scope, scopeContext, scopeContextKey);

      element[0].appendChild(redirectSidebarElement);
    },
  };
}
