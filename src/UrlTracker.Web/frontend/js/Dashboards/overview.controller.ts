export class UrlTrackerOverviewController {
    private dashboard: any;
    private redirects: any;
    private notFounds: any;
    private tabs: any[];
    private pageSizeOptions: any[];
    private allRootNodes: any;
    private entryDetailsOverlay: any;
    public settings: any;

    public static $inject = ["$scope", "urlTrackerEntryService", "editorService", "notificationsService", "entityResource", "overlayService"]
    constructor(private $scope: any, private urlTrackerEntryService: urltracker.services.IEntryService, private editorService: any, private notificationsService: any, private entityResource: any, private overlayService: any) {

        const $this = this;
        urlTrackerEntryService.getSettings().then((response) => {
            this.settings = response;
        });

        this.dashboard = {
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

        this.redirects = {
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

        this.notFounds = {
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
            $this.allRootNodes = rootNodes;
        });

        this.UpdateDashboard();
        this.GetRedirects(this.redirects);
        this.GetNotFounds(this.notFounds);

        this.tabs = [
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

        this.pageSizeOptions = [
            { "value": 10 },
            { "value": 20 },
            { "value": 30 },
            { "value": 50 },
            { "value": 100 },
            { "value": 200 }
        ];

        this.entryDetailsOverlay = {
            view: "/App_Plugins/UrlTracker/Panels/details.html",
            size: "small",
            entry: null,
            rootNodes: null,
            submit: function (model) {
                var promise;
                if (model.isNewEntry || model.entry.Is404) {
                    promise = urlTrackerEntryService.addRedirect(model.entry);
                } else {
                    promise = urlTrackerEntryService.updateRedirect(model.entry);
                }

                promise.then(function () {
                    if (model.isNewEntry || model.entry.Is404) {
                        notificationsService.success("Created", "Succesfully created a new redirect");
                    } else {
                        notificationsService.success("Edited", "Succesfully edited redirect");
                    }

                    editorService.close();
                    $this.GetRedirects($this.redirects);
                    $this.GetNotFounds($this.notFounds);
                    $this.UpdateDashboard();
                }).catch(function (message) {
                    notificationsService.error("Error", `An error occurred: ${message}`);
                });

                if (model.rootNodes != null && $this.entryDetailsOverlay.rootNodes != null) {
                    $this.entryDetailsOverlay.rootNodes = model.rootNodes;
                }
            },
            close: function (model) {
                if (model.rootNodes != null && $this.entryDetailsOverlay.rootNodes != null) {
                    $this.entryDetailsOverlay.rootNodes = model.rootNodes;
                }

                editorService.close();
            }
        }

        this.redirects.create = function () {
            $this.entryDetailsOverlay.entry = null;
            editorService.open($this.entryDetailsOverlay);
        }

        this.redirects.clickItem = function (item) {
            if (item.RedirectHttpCode != 410) {
                $this.entryDetailsOverlay.entry = Object.assign({}, item);
                editorService.open($this.entryDetailsOverlay);
            }
        }

        this.redirects.goToPage = function (pageNumber) {
            if ($this.redirects.pageNumber == pageNumber)
                return;

            $this.redirects.pagination.pageNumber = pageNumber;
            $this.GetRedirects($this.redirects);
        }

        this.redirects.nextPage = function (pageNumber) {
            $this.redirects.goToPage(pageNumber);
        }

        this.redirects.prevPage = function (pageNumber) {
            $this.redirects.goToPage(pageNumber);
        }

        this.redirects.deleteItem = function (item) {
            event?.stopPropagation();

            urlTrackerEntryService.deleteEntry(item.Id).then(function () {
                $this.GetRedirects($this.redirects);

                notificationsService.success("Deleted", "Redirect succesfully deleted");
                $this.UpdateDashboard();
            });
        }

        this.redirects.pageSizeChanged = function () {
            $this.GetRedirects($this.redirects);
        }

        this.redirects.selectAll = function () {
            if ($this.redirects.allItemsSelected) {
                $this.redirects.allItemsSelected = false;
                $this.redirects.selectedItems = [];
            }
            else {
                $this.redirects.allItemsSelected = true;
                $this.redirects.selectedItems = angular.copy($this.redirects.items);
            }
        }

        this.redirects.toggleSelectItem = function (event, item) {
            event.stopPropagation();
            var isAlreadySelected = $this.redirects.selectedItems.findIndex(function (i) {
                return i.Id == item.Id;
            });

            if (isAlreadySelected != -1) {
                $this.redirects.selectedItems.splice(isAlreadySelected, 1);
            }
            else {
                $this.redirects.selectedItems.push(item);
            }
        }

        this.redirects.deleteSelected = function () {
            var promises: any[] = [];

            $this.redirects.selectedItems.forEach(item => {
                promises.push(urlTrackerEntryService.deleteEntry(item.Id));
            });

            Promise.all(promises).then(() => {
                $this.redirects.selectedItems = [];
                $this.redirects.allItemsSelected = false;

                $this.GetRedirects($this.redirects);
                $this.UpdateDashboard();

                notificationsService.success("Deleted", "Redirects succesfully deleted");
            });
        }

        this.redirects.clearSelection = function () {
            $this.redirects.allItemsSelected = false;
            $this.redirects.selectedItems = [];

            var checkboxes = document.getElementsByClassName('select-redirect');
            for (var i = 0; i < checkboxes.length; i++) {
                if ((checkboxes[i] as any).type == 'checkbox')
                    (checkboxes[i] as any).checked = false;
            }
        }

        this.redirects.search = function () {
            $this.redirects.pagination.pageNumber = 1;
            $this.GetRedirects($this.redirects);
        }

        this.redirects.setFilter = function (column) {
            if ($this.redirects.filter) {
                var currentColumn = $this.redirects.filter.replace('Asc', '').replace('Desc', '');

                if (currentColumn == column) {
                    var currentIsDesc = $this.redirects.filter.includes('Desc');

                    if (currentIsDesc) {
                        $this.redirects.filter = `${column}Asc`;
                        return $this.GetRedirects($this.redirects);
                    }
                }
            }

            $this.redirects.filter = `${column}Desc`;
            $this.GetRedirects($this.redirects);
        }

        this.redirects.importExport = function () {
            overlayService.open({
                name: 'urltracker-import-export-overlay',
                view: '/App_Plugins/UrlTracker/Overlays/importexport.html',
                class: 'testest',
                hideSubmitButton: true,
                position: 'center',
                style: 'width:400px',
                refreshRedirects: function () {
                    $this.UpdateDashboard();
                    $this.GetRedirects($this.redirects);
                },
                close: function () {
                    overlayService.close();
                }
            });
        };

        //#region Not founds

        this.notFounds.createRedirect = function (item) {
            $this.entryDetailsOverlay.entry = Object.assign({}, item);
            editorService.open($this.entryDetailsOverlay);
        }

        this.notFounds.goToPage = function (pageNumber) {
            if ($this.notFounds.pageNumber == pageNumber)
                return;

            $this.notFounds.pagination.pageNumber = pageNumber;
            $this.GetNotFounds($this.notFounds);
        }

        this.notFounds.nextPage = function (pageNumber) {
            $this.notFounds.goToPage(pageNumber);
        }

        this.notFounds.prevPage = function (pageNumber) {
            $this.notFounds.goToPage(pageNumber);
        }

        this.notFounds.deleteItem = function (item) {
            event?.stopPropagation();

            urlTrackerEntryService.deleteEntry(item.Id, true).then(function () {
                notificationsService.success("Deleted", "Not found succesfully deleted");
                $this.GetNotFounds($this.notFounds);
                $this.UpdateDashboard();
            });
        }

        this.notFounds.pageSizeChanged = function () {
            $this.GetNotFounds($this.notFounds);
        }

        this.notFounds.selectAll = function () {
            if ($this.notFounds.allItemsSelected) {
                $this.notFounds.allItemsSelected = false;
                $this.notFounds.selectedItems = [];
            }
            else {
                $this.notFounds.allItemsSelected = true;
                $this.notFounds.selectedItems = angular.copy($this.notFounds.items);
            }
        }

        this.notFounds.toggleSelectItem = function (event, item) {
            event.stopPropagation();

            var index = $this.notFounds.selectedItems.findIndex(function (i) {
                return i.Id == item.Id;
            });

            if (index != -1)
                $this.notFounds.selectedItems.splice(index, 1);
            else
                $this.notFounds.selectedItems.push(item);
        }

        this.notFounds.deleteSelected = function () {
            var promises: any[] = [];

            $this.notFounds.selectedItems.forEach(item => {
                promises.push(urlTrackerEntryService.deleteEntry(item.Id, true));
            });

            Promise.all(promises).then(() => {
                $this.notFounds.selectedItems = [];
                $this.notFounds.allItemsSelected = false;

                $this.GetNotFounds($this.notFounds);
                $this.UpdateDashboard();

                notificationsService.success("Deleted", "Not founds succesfully deleted");
            });
        }

        this.notFounds.clearSelection = function () {
            $this.notFounds.allItemsSelected = false;
            $this.notFounds.selectedItems = [];

            var checkboxes = document.getElementsByClassName('select-notfound');
            for (var i = 0; i < checkboxes.length; i++) {
                if ((checkboxes[i] as any).type == 'checkbox')
                    (checkboxes[i] as any).checked = false;
            }
        }

        this.notFounds.search = function () {
            $this.notFounds.pagination.pageNumber = 1;
            $this.GetNotFounds($this.notFounds);
        }

        this.notFounds.addIgnore = function (id) {
            overlayService.open({
                content: 'Are you sure you want to permanently ignore this not found?',
                submit: function () {
                    urlTrackerEntryService.addIgnore404(id).then(() => {
                        $this.GetNotFounds($this.notFounds);

                        notificationsService.success("Ignored", "Not found succesfully added to ignore list");
                        $this.UpdateDashboard();
                    });

                    overlayService.close();
                },
                close: function () {
                    overlayService.close();
                }
            });
        }

        this.notFounds.setFilter = function (column) {
            if ($this.notFounds.filter) {
                var currentColumn = $this.notFounds.filter.replace('Asc', '').replace('Desc', '');

                if (currentColumn == column) {
                    var currentIsDesc = $this.notFounds.filter.includes('Desc');

                    if (currentIsDesc) {
                        $this.notFounds.filter = `${column}Asc`;
                        return $this.GetNotFounds($this.notFounds);
                    }
                }
            }

            $this.notFounds.filter = `${column}Desc`;
            $this.GetNotFounds($this.notFounds);
        }
        //#endregion

        //#region Watchers

        $scope.$watch('vm.redirects.numberOfItems', function () {
            $this.tabs.find(x => x.alias == "redirects").label = "Redirects" + " (" + $this.redirects.numberOfItems + ")";
        });

        $scope.$watch('vm.notFounds.numberOfItems', function () {
            $this.tabs.find(x => x.alias == "notFounds").label = "Not founds " + " (" + $this.notFounds.numberOfItems + ")";
        });

        //#endregion
    }

    public changeTab(alias: string) {
        this.tabs.forEach(x => x.active = false);
        this.tabs.find(x => x.alias == alias).active = true;
    }

    public getSiteName(entry: any) {
        if (this.allRootNodes) {
            var rootNode = this.allRootNodes.find(function (item) {
                return item.id == entry.RedirectRootNodeId;
            });

            if (rootNode) {
                return rootNode.name;
            }
        }
    }

    private GetRedirects(scope: any) {
        var skip = 0;

        if (scope.pagination.pageNumber > 1)
            skip = (scope.pagination.pageNumber - 1) * scope.itemsPerPage;

        scope.loading = true;
        this.urlTrackerEntryService.getRedirects(skip, scope.itemsPerPage, scope.searchString, scope.filter).then((response) => {
            scope.items = response.Entries;

            if (scope.pagination != null) {
                scope.numberOfItems = response.NumberOfEntries;
                scope.pagination.totalPages = Math.ceil(response.NumberOfEntries / scope.itemsPerPage);
            }
        }).finally(() => {
            scope.loading = false;
        });
    }

    private GetNotFounds(scope: any) {
        var skip = 0;
        if (scope.pagination.pageNumber > 1)
            skip = (scope.pagination.pageNumber - 1) * scope.itemsPerPage;

        scope.loading = true;
        this.urlTrackerEntryService.getNotFounds(skip, scope.itemsPerPage, scope.searchString, scope.filter).then((response) => {
            scope.items = response.Entries

            if (scope.pagination != null) {
                scope.numberOfItems = response.NumberOfEntries;
                scope.pagination.totalPages = Math.ceil(response.NumberOfEntries / scope.itemsPerPage);
            }
        }).finally(() => {
            scope.loading = false;
        });
    }

    private UpdateDashboard() {
        this.dashboard.redirects.loading = true;
        this.urlTrackerEntryService.getRedirects(0, 10, "", "CreatedDesc").then((response) => {
            this.dashboard.redirects.items = response.Entries;

            if (this.dashboard.redirects.pagination != null) {
                this.dashboard.redirects.numberOfItems = response.NumberOfEntries;
                this.dashboard.redirects.pagination.totalPages = Math.ceil(response.NumberOfEntries / 10);
            }
        }).finally(() => {
            this.dashboard.redirects.loading = false;
        });

        this.dashboard.notFounds.loading = true;
        this.urlTrackerEntryService.getNotFounds(0, 10, "", "OccurrencesDesc").then((response) => {
            this.dashboard.notFounds.items = response.Entries

            if (this.dashboard.notFounds.pagination != null) {
                this.dashboard.notFounds.numberOfItems = response.NumberOfEntries;
                this.dashboard.notFounds.pagination.totalPages = Math.ceil(response.NumberOfEntries / 10);
            }
        }).finally(() => {
            this.dashboard.notFounds.loading = false;
        });
        this.urlTrackerEntryService.countNotFoundsThisWeek().then((response) => {
            this.dashboard.notFoundsThisWeek = response;
        });
    }
}


export function ngEnterDirective(): angular.IDirective {
    return {
        link: (scope: angular.IScope, element: angular.IAugmentedJQuery, attributes: angular.IAttributes): void => {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attributes.ngEnter)
                    });
                    event.preventDefault();
                }
            })
        }
    }
}
