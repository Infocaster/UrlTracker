(function () {
	"use strict";
	angular.module("umbraco").controller("UrlTracker.DetailsController", ["$scope", "UrlTrackerEntryService", "contentResource", "editorService", function ($scope, UrlTrackerEntryService, contentResource, editorService) {
		var vm = this;

		vm.isNewEntry = false;
		vm.advancedView = false;
		vm.redirectNode = null;

		contentResource.getChildren(-1)
			.then(function (rootNodes) {
				vm.allRootNodes = rootNodes.items;
			});

		if ($scope.model.entry != null) {
			vm.entry = $scope.model.entry;

			if (vm.entry.Is404) {
				vm.title = "Create redirect";
				vm.entry.RedirectHttpCode = 301;
				vm.entry.Notes = "Redirect created to prevent 404";
			} else {
				vm.title = "Edit redirect";
				vm.advancedView = true;
			}

			if (vm.entry.RedirectNodeId && vm.entry.RedirectNodeId != 0) {
				contentResource.getNiceUrl(vm.entry.RedirectNodeId).then(
					function (url) {
						vm.entry.RedirectNodeUrl = url;
					}
				);

				contentResource.getById(vm.entry.RedirectNodeId)
					.then(function (redirectNode) {
						vm.entry.RedirectNodeName = redirectNode.variants[0].name;
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

					contentResource.getNiceUrl(vm.entry.RedirectNodeId).then(
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

		vm.allowRemove = true;
		vm.allowOpen = false;
		vm.sortable = false;

		vm.nodes = null;
	}]);
})();