export interface IUrlTrackerDashboardScope extends angular.IScope { }

export class UrlTrackerDashboardController {

    public static $inject = ["$scope"]
    constructor(private $scope: IUrlTrackerDashboardScope) {

        const $this = this;

        // initialise
    }
}
