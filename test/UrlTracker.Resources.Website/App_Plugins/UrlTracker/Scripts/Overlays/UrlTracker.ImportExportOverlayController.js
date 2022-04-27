(function () {
	"use strict";
	angular.module("umbraco").controller("UrlTracker.ImportExportOverlayController", ["$scope", "urlTrackerEntryService", 'notificationsService', function ($scope, urlTrackerEntryService, notificationsService) {
		var vm = this;

		vm.mode = "";
		vm.title = "";
		vm.file = null;
		vm.fileInputDisabled = false;

		vm.export = {
			loading: false
		}

		vm.import = {
			loading: false
		}

		vm.exportRedirects = function () {
			if (!vm.export.loading) {
				vm.export.loading = true;

				urlTrackerEntryService.exportRedirects().then(() => {
					vm.export.loading = false;
				});

			}
		}

		vm.importRedirects = function () {
			vm.mode = "import";
			vm.title = "Import redirects";
		}

		vm.back = function () {
			if (vm.mode == "import" && vm.import.loading == true)
				return;

			vm.mode = "";
			vm.title = "";
			vm.file = null;
		}

		vm.fileChanged = function (files, event) {
			if (files && files.length > 0) {
				vm.file = files[0];
			}
		}

		vm.submitImport = function () {
			if (vm.file && !vm.import.loading) {
				vm.import.loading = true;
				vm.fileInputDisabled = true;

				urlTrackerEntryService.importRedirects(vm.file).then((response) => {
					notificationsService.success("Success", `Succesfully imported ${response.data} redirects`);
					vm.import.loading = false;
					vm.fileInputDisabled = false;
					$scope.model.refreshRedirects();
				}).catch((response) => {
					notificationsService.error("Error", response.data.Message);
					vm.import.loading = false;
					vm.fileInputDisabled = false;
				})
			}
		}
	}]);
})();