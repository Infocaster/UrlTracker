(function () {
    "use strict";
    angular.module("umbraco").controller("UrlTracker.OverviewController", ["$scope", "UrlTrackerEntryService", "editorService", "contentResource", function ($scope, UrlTrackerEntryService, editorService, contentResource) {

        var vm = this;
        //table
        vm.items = [
        ];
        vm.selectedItems = [];
        contentResource.getChildren(-1)
            .then(function (rootNodes) {
                vm.allRootNodes = rootNodes.items;
            });

        vm.allItemsSelected = false;
        vm.itemsPerPage = 20;
        vm.createButtonState = "init";
        vm.searchString = "";
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
            UrlTrackerEntryService.getEntries(vm,skipNr, vm.itemsPerPage);
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
            UrlTrackerEntryService.deleteEntry(entry.Id);
            getItems();
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
            var isInSelectionIndex = vm.selectedItems.findIndex(function (i) {
                return i.Id == item.Id
            });
            if (isInSelectionIndex != -1) {
                vm.selectedItems.splice(isInSelectionIndex, 1);
            }
            else {
                vm.selectedItems.push(item);
            }
            
        }

        vm.saveEntry= function(entry){
            UrlTrackerEntryService.saveEntry(entry);
        }

        vm.deleteSelected = function () {
            vm.selectedItems.forEach(item => {
                UrlTrackerEntryService.deleteEntry(item.Id);
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

        vm.getSiteName = function (entry) {
            return vm.allRootNodes.find(function (item) {
                return item.id == entry.RedirectRootNodeId
            }).name;
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