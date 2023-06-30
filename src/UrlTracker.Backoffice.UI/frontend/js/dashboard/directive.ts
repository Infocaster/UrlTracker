import { UrlTrackerDashboard } from "./main.lit";
import { ILocalizationService, localizationServiceContext, localizationServiceKey } from "../context/localizationservice.context";
import { IIconHelper, iconHelperContext, iconHelperKey } from "../context/iconhelper.context";
import { IContentResource, contentResourceContext, contentResourceKey } from "../context/contentresource.context";

ngUrltrackerDashboard.alias = "ngUrltrackerDashboard";
ngUrltrackerDashboard.$inject = ["localizationService", "iconHelper", "contentResource"]
export function ngUrltrackerDashboard(localizationService: ILocalizationService, iconHelper: IIconHelper, contentResource: IContentResource): angular.IDirective {

    return {
        restrict: 'E',
        link: function (_scope, element) {

            let dashboardElement = document.createElement('urltracker-dashboard') as UrlTrackerDashboard;
            
            dashboardElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);
            dashboardElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
            dashboardElement.SetContext(contentResource, contentResourceContext, contentResourceKey)
            
            element[0].appendChild(dashboardElement);
        }
    };
}