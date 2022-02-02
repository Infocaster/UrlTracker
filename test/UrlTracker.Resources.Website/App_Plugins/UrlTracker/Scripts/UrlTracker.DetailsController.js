(function () {
    "use strict";
    angular.module("umbraco").controller("UrlTracker.DetailsController", ["$scope", "$location", "$q", "urlTrackerEntryService", "contentResource", "editorService", "entityResource", function ($scope, $location, $q, urlTrackerEntryService, contentResource, editorService, entityResource) {
        var vm = this;
        var isLoaded = false;

        vm.previousCulture = null;
        vm.isNewEntry = false;
        vm.advancedView = false;
        vm.redirectNode = null;
        vm.loading = true;

        LoadDomains().then(() => {
            LoadEntry();
        });

        vm.setRootNode = function (node) {
            vm.entry.RedirectRootNodeId = node;
        }

        vm.openNodePicker = function () {
            var pickerOptions = {
                startNodeId: vm.entry.RedirectRootNodeId,
                multiPicker: false,
                submit: function (model) {
                    vm.entry.RedirectNodeName = model.selection[0].name;
                    vm.entry.RedirectNodeId = model.selection[0].id;
                    vm.entry.RedirectNodeIcon = model.selection[0].icon;

                    entityResource.getUrl(vm.entry.RedirectNodeId, 'Document', vm.entry.Culture).then(
                        function (url) {
                            vm.entry.RedirectNodeUrl = url;
                        }
                    );

                    editorService.close();
                },
                close: function (model) {
                    editorService.close();
                }
            }
            editorService.contentPicker(pickerOptions);
        }

        vm.removeRedirectNode = function () {
            vm.entry.RedirectNodeName = vm.entry.RedirectNodeId = vm.entry.RedirectNodeIcon = null;
        }

        function submit() {
            if ($scope.model.submit) {
                $scope.model.isNewEntry = vm.isNewEntry;
                $scope.model.entry = vm.entry;
                $scope.model.rootNodes = vm.rootNodes;

                $location.search('mculture', vm.previousCulture);

                $scope.model.submit($scope.model);
            }
        }

        function close() {
            if ($scope.model.close) {
                $scope.model.rootNodes = vm.rootNodes;

                $location.search('mculture', vm.previousCulture);

                $scope.model.close($scope.model);
            }
        }

        vm.saveChanges = submit;
        vm.close = close;

        vm.allowRemove = true;
        vm.allowOpen = false;
        vm.sortable = false;

        vm.nodes = null;

        vm.updateLanguages = function () {
            if (vm.entry.RedirectRootNodeId == 0 || vm.entry.RedirectRootNodeId == -1 || vm.rootNodes == null) {
                vm.languages = null;
                vm.entry.Culture = null;
            } else {
                var domain = vm.rootNodes.find(n => n.id == vm.entry.RedirectRootNodeId);

                if (domain != null && domain.domainLanguages != null) {
                    vm.languages = domain.domainLanguages;

                    if (vm.entry.Culture == null || !vm.languages.find(x => x.IsoCode == vm.entry.Culture)) {

                        if (vm.languages.length > 0 && vm.languages[0] != null) {
                            vm.entry.Culture = vm.languages[0].IsoCode;
                        }
                    }
                } else {
                    vm.languages = null;
                    vm.entry.Culture = null;
                }
            }
        }

        function LoadDomains() {
            if ($scope.model.rootNodes != null) {
                return $q((resolve, reject) => {
                    vm.rootNodes = $scope.model.rootNodes;
                    resolve();
                });
            } else {
                return $q((resolve, reject) => {
                    entityResource.getChildren(-1, 'Document')
                        .then(function (rootNodes) {
                            var languagesPromise = [];
                            vm.rootNodes = rootNodes;

                            if (vm.rootNodes != null) {
                                vm.rootNodes.forEach(function (n) {
                                    languagesPromise.push(urlTrackerEntryService.getLanguagesOutNodeDomains(n, n.id));
                                });
                            }

                            $q.all(languagesPromise).then(() => {
                                resolve();
                            });
                        });
                });
            }
        }

        function LoadEntry() {
            if ($scope.model.entry != null) {
                vm.entry = $scope.model.entry;

                if (vm.entry.Is404) {
                    vm.title = "Create redirect";
                    vm.entry.remove404 = true;
                    vm.entry.RedirectHttpCode = 301;
                    vm.entry.Notes = "Redirect created to prevent 404";
                } else {
                    vm.title = "Edit redirect";
                    vm.advancedView = true;
                }

                if (vm.entry.RedirectNodeId && vm.entry.RedirectNodeId != 0) {
                    entityResource.getUrl(vm.entry.RedirectNodeId, 'Document', vm.entry.Culture).then(
                        function (url) {
                            vm.entry.RedirectNodeUrl = url;
                        }
                    );

                    entityResource.getById(vm.entry.RedirectNodeId, 'Document', vm.entry.Culture)
                        .then(function (redirectNode) {
                            vm.entry.RedirectNodeName = redirectNode.name;
                            vm.entry.RedirectNodeIcon = redirectNode.icon;
                        });
                }
            }
            else {
                vm.title = "Create redirect";

                vm.isNewEntry = true;
                vm.advancedView = true;

                vm.entry = {
                    OldUrl: "",
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

                if (vm.rootNodes != null) {
                    vm.entry.RedirectRootNodeId = vm.rootNodes[0].id;
                }
            }

            vm.updateLanguages();
            vm.loading = false;
        }

        $scope.$watch('vm.entry.Culture', function (e) {
            if (!vm.loading) {
                if ($location.search()["mculture"])
                    vm.previousCulture = $location.search()["mculture"];
                $location.search('mculture', e);
            }
        });
    }]);
})();