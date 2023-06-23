import { ILocalizationService } from "../umbraco/localizationService";
import { UrlTrackerDashboard } from "./main.lit";
import { localizationServiceContext, localizationServiceKey } from "../context/localizationservice.context";

ngUrltrackerDashboard.alias = "ngUrltrackerDashboard";
ngUrltrackerDashboard.$inject = ["localizationService"]
export function ngUrltrackerDashboard(localizationService: ILocalizationService): angular.IDirective {

    return {
        restrict: 'E',
        link: function (_scope, element) {

            let dashboardElement = document.createElement('urltracker-dashboard') as UrlTrackerDashboard;
            
            dashboardElement.SetContext(localizationService, localizationServiceContext, localizationServiceKey);
            
            element[0].appendChild(dashboardElement);
        }
    };
}