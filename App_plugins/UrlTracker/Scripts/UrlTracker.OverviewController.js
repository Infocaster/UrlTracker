(function () {
    "use strict";
    angular.module("umbraco").controller("UrlTracker.OverviewController", ["UrlTrackerEntryService", function (UrlTrackerEntryService) {

        var vm = this;
        //table
        vm.options = {
            includeProperties: [
                { alias: "Culture", header: "Site" }
            ]
        }
        vm.itemsPerPage = 20;
        vm.selectItem = selectItem;
        vm.clickItem = clickItem;
        vm.selectAll = selectAll;
        vm.isSelectedAll = isSelectedAll;
        vm.isSortDirection = isSortDirection;
        vm.sort = sort;

        //buttons
        vm.createButtonState = "init";
        vm.clickCreateButton = clickCreateButton;
        vm.pageSizeChanged = pageSizeChanged;

        //pagination
        vm.pagination = {
            pageNumber: 1,
            totalPages: 10
        }

        vm.nextPage = nextPage;
        vm.prevPage = prevPage;
        vm.goToPage = goToPage;

        vm.pageSizeOptions = [
            { "value": 10 },
            { "value": 20 },
            { "value": 30 },
            { "value": 50 },
            { "value": 100 },
            { "value": 200 },
        ];
        getItems();

        function getItems() {
            var skipNr = 0;
            if (vm.pagination.pageNumber > 1) {
                skipNr = (vm.pagination.pageNumber - 1) * vm.itemsPerPage;
            }
            var apiResult = UrlTrackerEntryService.getEntries(vm,skipNr, vm.itemsPerPage);
            //vm.items = apiResult.Entries;
            //vm.pagination.totalPages = apiResult.TotalPages;
            //vm.items = [];
            //vm.pagination.totalPages = 1;
        }

        function selectAll($event) {

        }

        function isSelectedAll() {

        }

        function clickItem(item) {
            window.location.assign("/App_plugins/UrlTracker/Views/UrlTrackerDetails.html?id=" + item.id);
        }

        function selectItem(selectedItem, $index, $event) {

        }

        function isSortDirection(col, direction) {

        }

        function sort(field, allow, isSystem) {
            //todo: get items sorted
        }

        function nextPage(pageNumber) {
            vm.pageNumber.pageNumber = pageNumber;
            getItems();
        }

        function prevPage(pageNumber) {
            vm.pageNumber.pageNumber = pageNumber;
            getItems();
        }

        function goToPage(pageNumber) {
            vm.pageNumber.pageNumber = pageNumber;
            getItems();
        }

        function clickCreateButton() {
            window.location.assign("/App_plugins/UrlTracker/Views/UrlTrackerDetails.html");
        }

        function filter() {

        }

        function pageSizeChanged(size) {
            vm.itemsPerPage = size;
            getItems();
        }
    }]);
})();