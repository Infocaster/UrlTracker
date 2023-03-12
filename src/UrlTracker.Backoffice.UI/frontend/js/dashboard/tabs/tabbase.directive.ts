import { basePath } from "../../util/constants";

ngUrlTrackerTabBase.alias = "ngUrltrackerTab"
export function ngUrlTrackerTabBase() : angular.IDirective {

    return {
        restrict: 'E',
        templateUrl: basePath + '/dashboard/tabs/tabbase.directive.html',
        transclude: true,
        scope:{
            category: '<'
        }
    }
}