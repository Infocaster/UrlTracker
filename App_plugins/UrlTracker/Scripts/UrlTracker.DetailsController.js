(function () {
    "use strict";
    angular.module("umbraco").controller("UrlTracker.DetailsController", ["$scope", "UrlTrackerEntryService", "contentResource", "editorService", function ($scope, UrlTrackerEntryService, contentResource, editorService) {

        var vm = this;
        vm.show = false;
        vm.isNewEntry = false;
        vm.advancedView = false;
        vm.redirectNode = null;

        contentResource.getChildren(-1)
            .then(function (rootNodes) {
                vm.allRootNodes = rootNodes.items;
            })


        if ($scope.model.entry != null) {
            vm.entry = $scope.model.entry
            contentResource.getById(vm.entry.RedirectNodeId)
                .then(function (redirectNode) {
                    vm.redirectNode = redirectNode.contentTypeName;
                });
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

        vm.setRootNode = function (node) {
            vm.entry.RedirectRootNodeId = node;
        }

        vm.openNodePicker = function () {
            var pickerOptions = {
                startNodeId: vm.entry.RedirectNodeId,
                multiPicker: false,
                submit: function (model) {
                    vm.redirectNode = model.selection[0].name;
                    vm.entry.RedirectNodeId = model.selection[0].id;
                    editorService.close();
                },
                close: function (model) {
                    editorService.close();
                }
            }
            editorService.contentPicker(pickerOptions);
        }

        function toggleAdvancedView() {
            vm.advancedView != vm.advancedView;
        }

        function submit() {
            if ($scope.model.submit) {

                $scope.model.isNewEntry = vm.isNewEntry;
                $scope.model.entry = vm.entry;
                
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