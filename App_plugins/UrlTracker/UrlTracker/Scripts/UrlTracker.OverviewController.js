(function () {
	"use strict";
	angular.module("umbraco").controller("UrlTracker.OverviewController", ["$scope", "UrlTrackerEntryService", "editorService", "contentResource", function ($scope, UrlTrackerEntryService, editorService, contentResource) {

		//#region General
		var vm = this;

		vm.dashboard = {
			notFounds: {
				items: [] //top 10
			},
			loading: false
		}

		vm.redirects = {
			items: [],
			numberOfItems: 0,
			selectedItems: [],
			allSelected: false,
			itemsPerPage: 20,
			createButtonState: "init",
			searchString: "",
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
			pagination: {
				pageNumber: 1,
				totalPages: 1
			},
			loading: false
		};

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

		contentResource.getChildren(-1).then(function (rootNodes) {
			vm.allRootNodes = rootNodes.items;
		});

		vm.changeTab = function (selectedTab) {
			vm.tabs.forEach(x => x.active = false);
			selectedTab.active = true;

			if (selectedTab.alias == "redirects" && !vm.redirects.items.length) {
				GetRedirects(vm.redirects);
			}
			else if (selectedTab.alias == "notFounds" && !vm.redirects.items.length) {
				console.log("notFounds");
				//SetNotFounds();
			}
		}

		vm.getSiteName = function (entry) {
			return vm.allRootNodes.find(function (item) {
				return item.id == entry.RedirectRootNodeId
			}).name;
		}

		function GetRedirects(scope) {
			var skip = 0;
			if (scope.pagination.pageNumber > 1) {
				skip = (scope.pagination.pageNumber - 1) * scope.itemsPerPage;
			}
			UrlTrackerEntryService.getRedirects(scope, skip, scope.itemsPerPage);
		}

		vm.createEntry = function () {
			vm.openEntryPanel(null);
		}

		vm.openEntryPanel = function (model) {
			vm.entryDetailsOverlay.entry = model;
			editorService.open(vm.entryDetailsOverlay);
		}

		vm.entryDetailsOverlay = {
			view: "/App_Plugins/UrlTracker/Views/UrlTrackerDetails.html",
			size: "small",
			entry: null,
			submit: function (model) {
				editorService.close();

				if (model.isNewEntry) 
					UrlTrackerEntryService.createEntry(vm, model.entry);
				else 
					UrlTrackerEntryService.saveEntry(vm, model.entry);

				//Todo: reload entries
			},
			close: function (oldModel) {
				editorService.close();
			}
		};
		//#endregion

		//#region Redirect functions 

		vm.redirects.clickItem = function (item) {
			vm.entryDetailsOverlay.entry = item;
			editorService.open(vm.entryDetailsOverlay);
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

		vm.redirects.prevPage = function(pageNumber) {
			vm.redirects.goToPage(pageNumber);
		}

		vm.redirects.editItem = function (item) {
			vm.openPanel(entry);
		}

		vm.redirects.deleteItem = function (item) {
			UrlTrackerEntryService.deleteItem(item.Id);
			GetRedirects(vm.redirects);
		}

		vm.redirects.pageSizeChanged = function () {
			console.log('çhange pagesize');
			GetRedirects(vm.redirects);
		}

		vm.redirects.selectAll = function () {
			if (vm.redirects.allItemsSelected) {
				vm.redirects.allItemsSelected = false;
				vm.redirects.selectedItems = [];
			}
			else {
				vm.redirects.allItemsSelected = true;
				vm.redirects.selectedItems = vm.redirects.items;
			}
		}

		vm.redirects.toggleSelectItem = function (event, item) {
			event.stopPropagation(); //Prevent click event on table row
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
			vm.redirects.selectedItems.forEach(item => {
				UrlTrackerEntryService.deleteEntry(item.Id);
			});
			vm.redirects.selectedItems = [];

			GetRedirects(vm.redirects);
		}

		vm.redirects.clearSelection = function () {
			vm.redirects.allItemsSelected = false;
			vm.redirects.selectedItems = [];

			//Todo: fixen
			var checkboxes = document.getElementsByClassName('item-select');
			for (var i = 0; i < checkboxes.length; i++) {
				if (checkboxes[i].type == 'checkbox')
					checkboxes[i].checked = false;
			}
		}

		vm.redirects.search = function () {
			if (vm.redirects.searchString !== "") {
				var skip = 0;
				if (vm.redirects.pagination.pageNumber > 1) {
					skip = (vm.redirects.pagination.pageNumber - 1) * vm.redirects.itemsPerPage;
				}
				UrlTrackerEntryService.searchRedirects(vm.redirects, skip, vm.redirects.itemsPerPage, vm.redirects.searchString);
			}
			else {
				GetRedirects(vm.redirects);
			}
		}

		//#endregion Redirect functions 

		//#region Redirect functions 

		//vm.redirects.clickItem = function (item) {
		//	console.log('click entry');
		//	//vm.redirects.overlay.entry = item;
		//	//editorService.open(vm.overlay);
		//}

		//vm.redirects.goToPage = function (pageNumber) {
		//	if (vm.redirects.pageNumber != pageNumber)
		//		return;

		//	vm.pagination.pageNumber = pageNumber;
		//	GetEntries(vm.redirects);
		//}

		//vm.redirects.editItem = function (item) {
		//	vm.openPanel(entry);
		//}

		//vm.redirects.deleteItem = function (item) {
		//	UrlTrackerEntryService.deleteItem(item.Id);
		//	GetEntries(vm.redirects);
		//}

		//vm.redirects.createRedirect = function () { //Make global to reuse it on creating redirect out of 404?
		//	vm.redirects.openPanel();
		//}

		//vm.redirects.pageSizeChanged = function () {
		//	GetEntries(vm.redirects);
		//}

		//vm.redirects.selectAll = function () {
		//	if (vm.redirects.allItemsSelected) {
		//		vm.redirects.allItemsSelected = false;
		//		vm.redirects.selectedItems = [];
		//	}
		//	else {
		//		vm.redirects.allItemsSelected = true;
		//		vm.redirects.selectedItems = vm.items;
		//	}
		//}

		//vm.redirects.toggleSelectItem = function (event, item) {
		//	event.stopPropagation(); //Prevent click event on table row

		//	var isAlreadySelected = vm.redirects.selectedItems.findIndex(function (i) {
		//		return i.Id == item.Id;
		//	});

		//	if (isAlreadySelected != -1) {
		//		vm.selectedItems.splice(isAlreadySelected, 1);
		//	}
		//	else {
		//		vm.selectedItems.push(item);
		//	}
		//}

		//vm.redirects.deleteSelected = function () {
		//	vm.redirects.selectedItems.forEach(item => {
		//		UrlTrackerEntryService.deleteEntry(item.Id);
		//	});
		//	vm.redirects.selectedItems = [];
		//	GetEntries(vm.redirects);
		//}

		//vm.redirects.clearSelection = function () {
		//	vm.redirects.allItemsSelected = false;
		//	vm.redirects.selectedItems = [];

		//	//Todo: fixen
		//	var checkboxes = document.getElementsByClassName('item-select');
		//	for (var i = 0; i < checkboxes.length; i++) {
		//		if (checkboxes[i].type == 'checkbox')
		//			checkboxes[i].checked = false;
		//	}
		//}



		//vm.redirects.detailsOverlay = {
		//	view: "/App_Plugins/UrlTracker/Views/UrlTrackerDetails.html",
		//	entry: null,
		//	size: "small",
		//	submit: function (model) {
		//		console.log(model);
		//		editorService.close();
		//		if (model.isNewEntry) {
		//			UrlTrackerEntryService.createEntry(vm, model.entry);
		//		}
		//		else {
		//			UrlTrackerEntryService.saveEntry(vm, model.entry);
		//		}
		//		GetEntries(vm.redirects);
		//	},
		//	close: function () {
		//		editorService.close();
		//	}
		//};



		//vm.saveentry = function (entry) {
		//	urltrackerentryservice.saveentry(entry);
		//}

		//vm.redirects.search = function () {
		//	if (vm.redirects.searchString !== "") {
		//		var skip = 0;
		//		if (vm.redirects.pagination.pageNumber > 1) {
		//			skip = (vm.redirects.pagination.pageNumber - 1) * vm.redirects.itemsPerPage;
		//		}
		//		UrlTrackerEntryService.search(vm.redirects, vm.redirects.searchString, skip, vm.redirects.itemsPerPage);
		//	}
		//	else {
		//		GetEntries(vm.redirects);
		//	}
		//}

		//#endregion Redirect functions 
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