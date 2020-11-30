(function () {
	"use strict";
	angular.module("umbraco").controller("UrlTracker.DetailsController", ["$scope", "$location", "urlTrackerEntryService", "contentResource", "editorService", "entityResource", function ($scope, $location, urlTrackerEntryService, contentResource, editorService, entityResource) {
		var vm = this;

		vm.previousCulture = null;
		vm.isNewEntry = false;
		vm.advancedView = false;
		vm.redirectNode = null;

		entityResource.getChildren(-1, 'Document')
			.then(function (rootNodes) {
				vm.allRootNodes = rootNodes;
			});

		if ($scope.model.entry != null) {
			urlTrackerEntryService.getLanguagesOutNodeDomains(vm, $scope.model.entry.RedirectRootNodeId);
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
						console.log(url);
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

			if(vm.languages != null)
				vm.entry.Culture = vm.languages[0].IsoCode;

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
		}

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
				$location.search('mculture', vm.previousCulture);

				$scope.model.submit($scope.model);
			}
		}

		function close() {
			if ($scope.model.close) {
				$location.search('mculture', vm.previousCulture);
				$scope.model.close();
			}
		}

		vm.saveChanges = submit;
		vm.close = close;

		vm.allowRemove = true;
		vm.allowOpen = false;
		vm.sortable = false;

		vm.nodes = null;

		$scope.$watch('vm.entry.RedirectRootNodeId', function (e) {
			if (vm.entry.RedirectRootNodeId == -1)
				return;

			urlTrackerEntryService.getLanguagesOutNodeDomains(vm, vm.entry.RedirectRootNodeId);
		});

		$scope.$watch('vm.entry.Culture', function (e) {
			if ($location.search()["mculture"])
				vm.previousCulture = $location.search()["mculture"];
			$location.search('mculture', e);
		});
	}]);
})();