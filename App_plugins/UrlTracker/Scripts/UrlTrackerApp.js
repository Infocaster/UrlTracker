(function () {
    angular.module("UrlTracker", ["ngRoute"])
        .config(["$routeProvider", "$locationProvider", function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/overview', {
                    templateUrl: '/App_Plugins/UrlTracker/Views/UrlTrackerManager.html',
                    controller: 'UrlTracker.OverviewController',
                    controllerAs: 'vm'
                })
                .when('/details', {
                    templateUrl: '/App_Plugins/UrlTracker/Views/UrlTrackerDetails.html',
                    controller: 'UrlTracker.DetailsController',
                    controllerAs: 'vm'
                })
                .otherwise({
                    redirectTo: '/overview'
                });

            $locationProvider.html5Mode({
                enabled: true
            });
        }]);

})();