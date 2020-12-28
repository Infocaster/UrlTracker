(function () {
	"use strict";
	angular.module("umbraco").controller("UrlTracker.OverviewController", ["$scope", "urlTrackerEntryService", "editorService", "notificationsService", "entityResource", "overlayService", function ($scope, urlTrackerEntryService, editorService, notificationsService, entityResource, overlayService) {
		//#region General
		var vm = this;

		urlTrackerEntryService.getSettings(vm);

		vm.dashboard = {
			notFoundsThisWeek: null,
			notFounds: {
				loading: false,
				items: [] //top 10
			},
			redirects: {
				loading: false,
				items: [] //newest
			},

		}

		vm.redirects = {
			items: [],
			numberOfItems: 0,
			selectedItems: [],
			allSelected: false,
			itemsPerPage: 20,
			createButtonState: "init",
			searchString: "",
			filter: "CreatedDesc",
			pagination: {
				pageNumber: 1,
				totalPages: 1
			},
			loading: false
		};

		vm.notFounds = {
			items: [],
			numberOfItems: 0,
			selectedItems: [],
			allSelected: false,
			itemsPerPage: 20,
			searchString: "",
			filter: "LastOccurredDesc",
			pagination: {
				pageNumber: 1,
				totalPages: 1
			},
			loading: false
		};

		entityResource.getChildren(-1, "Document").then(function (rootNodes) {
			vm.allRootNodes = rootNodes;
		});

		UpdateDashboard();
		GetRedirects(vm.redirects);
		GetNotFounds(vm.notFounds);

		vm.tabs = [
			{
				"alias": "dashboard",
				"label": "Dashboard",
				"active": true
			},
			{
				"alias": "notFounds",
				"label": "Not founds"
			},
			{
				"alias": "redirects",
				"label": "Redirects"
			},
			{
				"alias": "info",
				"label": "Info"
			}
		];

		vm.pageSizeOptions = [
			{ "value": 10 },
			{ "value": 20 },
			{ "value": 30 },
			{ "value": 50 },
			{ "value": 100 },
			{ "value": 200 }
		];

		vm.changeTab = function (alias) {
			vm.tabs.forEach(x => x.active = false);
			vm.tabs.find(x => x.alias == alias).active = true;
		}

		vm.getSiteName = function (entry) {
			if (vm.allRootNodes) {
				var rootNode = vm.allRootNodes.find(function (item) {
					return item.id == entry.RedirectRootNodeId;
				});

				if (rootNode) {
					return rootNode.name;
				}
			}
		}

		function GetRedirects(scope) {
			var skip = 0;

			if (scope.pagination.pageNumber > 1)
				skip = (scope.pagination.pageNumber - 1) * scope.itemsPerPage;

			urlTrackerEntryService.getRedirects(scope, skip, scope.itemsPerPage, scope.searchString, scope.filter);
		}

		function GetNotFounds(scope) {
			var skip = 0;
			if (scope.pagination.pageNumber > 1)
				skip = (scope.pagination.pageNumber - 1) * scope.itemsPerPage;

			urlTrackerEntryService.getNotFounds(scope, skip, scope.itemsPerPage, scope.searchString, scope.filter);
		}

		function UpdateDashboard() {
			urlTrackerEntryService.getRedirects(vm.dashboard.redirects, 0, 10, "", "CreatedDesc");
			urlTrackerEntryService.getNotFounds(vm.dashboard.notFounds, 0, 10, "", "OccurrencesDesc");
			urlTrackerEntryService.countNotFoundsThisWeek(vm.dashboard);
		}

		vm.entryDetailsOverlay = {
			view: "/App_Plugins/UrlTracker/Views/UrlTrackerDetails.html",
			size: "small",
			entry: null,
			rootNodes: null,
			submit: function (model) {
				var promise;
				if (model.isNewEntry || model.entry.Is404) {
					promise = urlTrackerEntryService.addRedirect(vm, model.entry);
				} else {
					promise = urlTrackerEntryService.updateRedirect(vm, model.entry);
				}

				promise.then(function () {
					if (model.isNewEntry || model.entry.Is404) {
						notificationsService.success("Created", "Succesfully created a new redirect");
					} else {
						notificationsService.success("Edited", "Succesfully edited redirect");
					}

					editorService.close();
					GetRedirects(vm.redirects);
					GetNotFounds(vm.notFounds);
					UpdateDashboard();
				}).catch(function (message) {
					notificationsService.error("Error", `An error occurred: ${message}`);
				});

				if (model.rootNodes != null && vm.entryDetailsOverlay.rootNodes != null) {
					vm.entryDetailsOverlay.rootNodes = model.rootNodes;
				}
			},
			close: function (model) {
				if (model.rootNodes != null && vm.entryDetailsOverlay.rootNodes != null) {
					vm.entryDetailsOverlay.rootNodes = model.rootNodes;
				}

				editorService.close();
			}
		}
		//#endregion

		//#region Redirect functions 

		vm.redirects.create = function () {
			vm.entryDetailsOverlay.entry = null;
			editorService.open(vm.entryDetailsOverlay);
		}

		vm.redirects.clickItem = function (item) {
			if (item.RedirectHttpCode != 410) {
				vm.entryDetailsOverlay.entry = Object.assign({}, item);
				editorService.open(vm.entryDetailsOverlay);
			}
		}

		vm.redirects.goToPage = function (pageNumber) {
			if (vm.redirects.pageNumber == pageNumber)
				return;

			vm.redirects.pagination.pageNumber = pageNumber;
			GetRedirects(vm.redirects);
		}

		vm.redirects.nextPage = function (pageNumber) {
			vm.redirects.goToPage(pageNumber);
		}

		vm.redirects.prevPage = function (pageNumber) {
			vm.redirects.goToPage(pageNumber);
		}

		vm.redirects.deleteItem = function (item) {
			event.stopPropagation();

			urlTrackerEntryService.deleteEntry(item.Id).then(function () {
				GetRedirects(vm.redirects);

				notificationsService.success("Deleted", "Redirect succesfully deleted");
				UpdateDashboard();
			});
		}

		vm.redirects.pageSizeChanged = function () {
			GetRedirects(vm.redirects);
		}

		vm.redirects.selectAll = function () {
			if (vm.redirects.allItemsSelected) {
				vm.redirects.allItemsSelected = false;
				vm.redirects.selectedItems = [];
			}
			else {
				vm.redirects.allItemsSelected = true;
				vm.redirects.selectedItems = angular.copy(vm.redirects.items);
			}
		}

		vm.redirects.toggleSelectItem = function (event, item) {
			event.stopPropagation();
			var isAlreadySelected = vm.redirects.selectedItems.findIndex(function (i) {
				return i.Id == item.Id;
			});

			if (isAlreadySelected != -1) {
				vm.redirects.selectedItems.splice(isAlreadySelected, 1);
			}
			else {
				vm.redirects.selectedItems.push(item);
			}
		}

		vm.redirects.deleteSelected = function () {
			var promises = [];

			vm.redirects.selectedItems.forEach(item => {
				promises.push(urlTrackerEntryService.deleteEntry(item.Id));
			});

			Promise.all(promises).then(() => {
				vm.redirects.selectedItems = [];
				vm.redirects.allItemsSelected = false;

				GetRedirects(vm.redirects);
				UpdateDashboard();

				notificationsService.success("Deleted", "Redirects succesfully deleted");
			});
		}

		vm.redirects.clearSelection = function () {
			vm.redirects.allItemsSelected = false;
			vm.redirects.selectedItems = [];

			var checkboxes = document.getElementsByClassName('select-redirect');
			for (var i = 0; i < checkboxes.length; i++) {
				if (checkboxes[i].type == 'checkbox')
					checkboxes[i].checked = false;
			}
		}

		vm.redirects.search = function () {
			vm.redirects.pagination.pageNumber = 1;
			GetRedirects(vm.redirects);
		}

		vm.redirects.setFilter = function (column) {
			if (vm.redirects.filter) {
				var currentColumn = vm.redirects.filter.replace('Asc', '').replace('Desc', '');

				if (currentColumn == column) {
					var currentIsDesc = vm.redirects.filter.includes('Desc');

					if (currentIsDesc) {
						vm.redirects.filter = `${column}Asc`;
						return GetRedirects(vm.redirects);
					}
				}
			} 

			vm.redirects.filter = `${column}Desc`;
			GetRedirects(vm.redirects);
		}

		vm.redirects.importExport = function() {
			overlayService.open({
				name: 'urltracker-import-export-overlay',
				view: '/App_Plugins/UrlTracker/Views/UrlTrackerImportExportOverlay.html',
				class: 'testest',
				hideSubmitButton: true,
				position: 'center',
				style: 'width:400px',
				refreshRedirects: function () {
					UpdateDashboard();
					GetRedirects(vm.redirects);
				},
				close: function () {
					overlayService.close();
				}
			});
		};

		//#endregion

		//#region Not founds

		vm.notFounds.createRedirect = function (item) {
			vm.entryDetailsOverlay.entry = Object.assign({}, item);
			editorService.open(vm.entryDetailsOverlay);
		}

		vm.notFounds.goToPage = function (pageNumber) {
			if (vm.notFounds.pageNumber == pageNumber)
				return;

			vm.notFounds.pagination.pageNumber = pageNumber;
			GetNotFounds(vm.notFounds);
		}

		vm.notFounds.nextPage = function (pageNumber) {
			vm.notFounds.goToPage(pageNumber);
		}

		vm.notFounds.prevPage = function (pageNumber) {
			vm.notFounds.goToPage(pageNumber);
		}

		vm.notFounds.editItem = function (item) {
			vm.openPanel(entry);
		}

		vm.notFounds.deleteItem = function (item) {
			event.stopPropagation();

			urlTrackerEntryService.deleteEntry(item.Id, true).then(function () {
				notificationsService.success("Deleted", "Not found succesfully deleted");
				GetNotFounds(vm.notFounds);
				UpdateDashboard();
			});
		}

		vm.notFounds.pageSizeChanged = function () {
			GetNotFounds(vm.notFounds);
		}

		vm.notFounds.selectAll = function () {
			if (vm.notFounds.allItemsSelected) {
				vm.notFounds.allItemsSelected = false;
				vm.notFounds.selectedItems = [];
			}
			else {
				vm.notFounds.allItemsSelected = true;
				vm.notFounds.selectedItems = angular.copy(vm.notFounds.items);
			}
		}

		vm.notFounds.toggleSelectItem = function (event, item) {
			event.stopPropagation();

			var index = vm.notFounds.selectedItems.findIndex(function (i) {
				return i.Id == item.Id;
			});

			if (index != -1) 
				vm.notFounds.selectedItems.splice(index, 1);
			else 
				vm.notFounds.selectedItems.push(item);
		}

		vm.notFounds.deleteSelected = function () {
			var promises = [];

			vm.notFounds.selectedItems.forEach(item => {
				promises.push(urlTrackerEntryService.deleteEntry(item.Id, true));
			});

			Promise.all(promises).then(() => {
				vm.notFounds.selectedItems = [];
				vm.notFounds.allItemsSelected = false;

				GetNotFounds(vm.notFounds);
				UpdateDashboard();

				notificationsService.success("Deleted", "Not founds succesfully deleted");
			});
		}

		vm.notFounds.clearSelection = function () {
			vm.notFounds.allItemsSelected = false;
			vm.notFounds.selectedItems = [];

			var checkboxes = document.getElementsByClassName('select-notfound');
			for (var i = 0; i < checkboxes.length; i++) {
				if (checkboxes[i].type == 'checkbox')
					checkboxes[i].checked = false;
			}
		}

		vm.notFounds.search = function () {
			vm.notFounds.pagination.pageNumber = 1;
			GetNotFounds(vm.notFounds);
		}

		vm.notFounds.addIgnore = function (id) {
			overlayService.open({
				content: 'Are you sure you want to permanently ignore this not found?',
				submit: function () {
					urlTrackerEntryService.addIgnore404(id).then(() => {
						GetNotFounds(vm.notFounds);

						notificationsService.success("Ignored", "Not found succesfully added to ignore list");
						UpdateDashboard();
					});

					overlayService.close();
				},
				close: function () {
					overlayService.close();
				}
			});
		}

		vm.notFounds.setFilter = function (column) {
			if (vm.notFounds.filter) {
				var currentColumn = vm.notFounds.filter.replace('Asc', '').replace('Desc', '');

				if (currentColumn == column) {
					var currentIsDesc = vm.notFounds.filter.includes('Desc');

					if (currentIsDesc) {
						vm.notFounds.filter = `${column}Asc`;
						return GetNotFounds(vm.notFounds);
					}
				}
			}

			vm.notFounds.filter = `${column}Desc`;
			GetNotFounds(vm.notFounds);
		}
		//#endregion

		//#region Watchers

		$scope.$watch('vm.redirects.numberOfItems', function () {
			vm.tabs.find(x => x.alias == "redirects").label = "Redirects" + " (" + vm.redirects.numberOfItems + ")";
		});

		$scope.$watch('vm.notFounds.numberOfItems', function () {
			vm.tabs.find(x => x.alias == "notFounds").label = "Not founds " + " (" + vm.notFounds.numberOfItems + ")";
		});

		//#endregion

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