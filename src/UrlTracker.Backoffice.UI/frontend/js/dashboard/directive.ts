import { UrlTrackerDashboard } from "./main.lit";
import {
  ILocalizationService,
  localizationServiceContext,
  localizationServiceKey,
} from "../context/localizationservice.context";
import {
  IIconHelper,
  iconHelperContext,
  iconHelperKey,
} from "../context/iconhelper.context";
import {
  IEditorService,
  editorServiceContext,
  editorServiceKey,
} from "../context/editorservice.context";
import { scopeContext, scopeContextKey } from "@/context/scope.context";

ngUrltrackerDashboard.alias = "ngUrltrackerDashboard";
ngUrltrackerDashboard.$inject = [
  "localizationService",
  "iconHelper",
  "editorService",
];
export function ngUrltrackerDashboard(
  localizationService: ILocalizationService,
  iconHelper: IIconHelper,
  editorService: IEditorService<any>
): angular.IDirective {
  return {
    restrict: "E",
    link: function (_scope, element) {
      let dashboardElement = document.createElement(
        "urltracker-dashboard"
      ) as UrlTrackerDashboard;

      dashboardElement.SetContext(
        localizationService,
        localizationServiceContext,
        localizationServiceKey
      );
      dashboardElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
      dashboardElement.SetContext(
        editorService,
        editorServiceContext,
        editorServiceKey
      );
      dashboardElement.SetContext(_scope, scopeContext, scopeContextKey);

      element[0].appendChild(dashboardElement);
    },
  };
}
