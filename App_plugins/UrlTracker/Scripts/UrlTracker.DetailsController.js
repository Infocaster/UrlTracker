(function () {
    "use strict";
    angular.module("umbraco").controller("UrlTracker.DetailsController", ["$scope", "UrlTrackerEntryService", function ($scope, UrlTrackerEntryService) {

        var vm = this;
        vm.show = false;
        vm.isNewEntry = false;
        vm.advancedView = false;

        if ($scope.model.entry != null) {
            vm.entry = $scope.model.entry
        }
        else {
            //show empty
            vm.isNewEntry = true;
            vm.advancedView = true;
            vm.entry = {
                OldUrl: "",
                OldUrlQueryString: "",
                RedirectNodeId: null,
                OldRexEx: "",
                RedirectUrl: "",
                Refferer: "",
                ForceRedirect: false,
                RedirectPassThroughQueryString: false,
                RedirectHttpCode: 301,
                Notes: "",
                RedirectRootNodeId: -1
            };
        }

        function toggleAdvancedView() {
            vm.advancedView != vm.advancedView;
        }

        function submit() {
            if ($scope.model.submit) {
                if (vm.isNewEntry) {
                    UrlTrackerEntryService.createEntry($scope.model.entry);
                }
                else{
                    UrlTrackerEntryService.saveEntry($scope.model.entry);
                }
                
                $scope.model.submit($scope.model);
            }
        }

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        }

        vm.saveChanges = submit;
        vm.close = close;
        vm.toggleAdvancedView = toggleAdvancedView;
    }]);
})();