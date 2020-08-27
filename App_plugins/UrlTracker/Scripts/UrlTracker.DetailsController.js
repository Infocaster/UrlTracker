(function () {
    "use strict";

    function Controller() {

        var vm = this;

        vm.ancestors = ["Overview"]
        vm.isNewEntry = false;
        vm.advancedView = false;

        if ($location.search().hasOwnProperty('id')) {
            //show excisting 
            var id = $location.search()["id"];
            vm.entry = urlTrackerService.getEntryDetails(id)
            vm.ancestors.append("Details #"+id)
        }
        else {
            //show empty
            vm.isNewEntry = true;
            vm.advancedView = true;
            vm.entry.oldUrl = "";
            vm.entry.oldUrlQueryString = "";
            vm.entry.redirectNodeId = null;
            vm.entry.oldRexEx = "";
            vm.entry.redirectUrl = "";
            vm.entry.Refferer = "";
            vm.entry.forceRedirect = false;
            vm.entry.redirectPassThroughQueryString = false;
            vm.entry.redirectHttpCode = 301;
            vm.entry.notes = "";
            vm.entry.redirectRootNodeId = -1;

        }

        function toggleAdvancedView() {
            vm.advancedView != vm.advancedView;
            //do stuff and reload?
        }

        function saveChanges() {
            if (vm.isNewEntry) {
                urlTrackerService.createEntry(vm.entry)
            }
            else {
                urlTrackerService.saveEntry(vm.entry)
            }
            location.assign("/UrlTrackerManager.html")
        }

        vm.saveChanges = saveChanges;
        vm.toggleAdvancedView = toggleAdvancedView;

        angular.module("UrlTracker").controller("UrlTracker.DetailsController", Controller);
    }
})();