(function () {
    "use strict";
    angular.module("umbraco").controller("UrlTracker.OverviewController", ["$scope","UrlTrackerEntryService","editorService", function ($scope, UrlTrackerEntryService, editorService) {

        var vm = this;
        //table
        vm.items = [
        ];
        vm.selectedItems = [];
        vm.options = {
            includeProperties: [
                { alias: "RedirectNodeId", header: "NodeId" },
                { alias: "OldUrl", header: "OldUrl" },
                { alias: "RedirectUrl", header: "NewUrl" },
                { alias: "Notes", header: "Notes" },
                { alias: "Inserted", header: "CreatedAt" }
            ]
        }
        vm.allItemsSelected = false;
        vm.itemsPerPage = 20;
        //buttons
        vm.createButtonState = "init";
        vm.searchString = "";
        //pagination
        vm.pagination = {
            pageNumber: 1,
            totalPages: 1
        }

        vm.pageSizeOptions = [
            { "value": 10 },
            { "value": 20 },
            { "value": 30 },
            { "value": 50 },
            { "value": 100 },
            { "value": 200 },
        ];

        vm.dropdownOpen = false;
        vm.showDetailPage = false;

        getItems();

        vm.dropDownClose = function() {
            vm.dropdownOpen = false;
        }

        vm.toggle = function() {
            vm.dropdownOpen = true;
        }

        vm.dropDownSelect =  function(item) {
            vm.itemsPerPage = item.value
        }

        function getItems() {
            var skipNr = 0;
            if (vm.pagination.pageNumber > 1) {
                skipNr = (vm.pagination.pageNumber - 1) * vm.itemsPerPage;
            }
            var apiResult = UrlTrackerEntryService.getEntries(vm,skipNr, vm.itemsPerPage);
            vm.items = apiResult.Entries;
            vm.pagination.totalPages = apiResult.TotalPages;
        }


        vm.clickEntry = function(item) {
            vm.overlay.entry = item;
            editorService.open(vm.overlay);
        }

        vm.nextPage = function (pageNumber) {
            vm.pagination.pageNumber = pageNumber;
            getItems();
        }

        vm.prevPage = function(pageNumber) {
            vm.pagination.pageNumber = pageNumber;
            getItems();
        }

        vm.goToPage = function(pageNumber) {
            vm.pagination.pageNumber = pageNumber;
            getItems();
        }

        vm.editEntry = function (entry) {
            vm.openPanel(entry);
        } 

        vm.deleteEntry = function (entry) {
            UrlTrackerEntryService.deleteEntry(entry.id);
        }

        vm.clickCreateButton = function () {
            vm.openPanel(null);
        }

        vm.openPanel = function (model) {
            vm.overlay.entry = model;
            editorService.open(vm.overlay)
        }

        vm.pageSizeChanged= function() {
            getItems();
        }

        vm.overlay = {
            view: "/App_Plugins/UrlTracker/Views/UrlTrackerDetails.html",
            entry: null,
            submit: function (model) {
                editorService.close();
                getItems();
            },
            close: function (oldModel) {
                editorService.close();
            }
        };

        vm.selectAll = function () {
            if (vm.allItemsSelected) {
                vm.allItemsSelected = false;
                vm.selectedItems = []
            }
            else {
                vm.allItemsSelected = true;
                vm.selectedItems = vm.items;
            }
        }

        vm.selectEntry = function (item) {
            vm.selectedItems.push(item);
        }

        vm.saveEntry= function(entry){
            UrlTrackerEntryService.saveEntry(entry);
        }

        vm.deleteSelected = function () {
            vm.selectedItems.forEach(item => {
                UrlTrackerEntryService.deleteEntry(item.id);
            });
            vm.selectedItems = [];
            getItems();
        }

        vm.search = function () {
            if (vm.searchString !== "") {
                var skipNr = 0;
                if (vm.pagination.pageNumber > 1) {
                    skipNr = (vm.pagination.pageNumber - 1) * vm.itemsPerPage;
                }
                var apiResult = UrlTrackerEntryService.search(vm, vm.searchString, skipNr, vm.itemsPerPage);
                vm.items = apiResult.Entries;
                vm.pagination.totalPages = apiResult.TotalPages;
            }
            else{
                getItems();
            }
        }

    }]);

    angular.module("umbraco").directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });
                    event.preventDefault();
                }
            });
        };
    });

})();