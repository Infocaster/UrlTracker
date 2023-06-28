import { ILocalizationService } from "../umbraco/localization.service";
import { UrlTrackerDashboard } from "./main.lit";
import { localizationServiceContext, localizationServiceKey } from "../context/localizationservice.context";
import { IIconHelper } from "../umbraco/icon.service";
import { iconHelperContext, iconHelperKey } from "../context/iconhelper.context";

ngUrltrackerDashboard.alias = "ngUrltrackerDashboard";
ngUrltrackerDashboard.$inject = ["localizationService", "iconHelper"]
export function ngUrltrackerDashboard(localizationService: ILocalizationService, iconHelper: IIconHelper): angular.IDirective {

    return {
        restrict: 'E',
        link: function (_scope, element) {

            let dashboardElement = document.createElement('urltracker-dashboard') as UrlTrackerDashboard;
            
            dashboardElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);
            dashboardElement.SetContext(iconHelper, iconHelperContext, iconHelperKey);
            
            element[0].appendChild(dashboardElement);
        }
    };
}