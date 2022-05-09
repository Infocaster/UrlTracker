export class UrlTrackerDetailsController {

    private isLoaded: boolean;
    public previousCulture: any;
    public isNewEntry: boolean;
    public advancedView: boolean;
    public redirectNode: any;
    public loading: boolean;
    public allowRemove: boolean;
    public allowOpen: boolean;
    public sortable: boolean;
    public nodes: any;
    public entry: any;
    public rootNodes: any;
    public languages: any;
    public title: string | null;

    public static $inject = ["$scope", "$location", "$q", "urlTrackerEntryService", "contentResource", "editorService", "entityResource"];
    constructor(private $scope, private $location: angular.ILocationService, private $q: angular.IQService, private urlTrackerEntryService: urltracker.services.IEntryService, private contentResource, private editorService, private entityResource) {

        this.isLoaded = false;

        this.previousCulture = null;
        this.isNewEntry = false;
        this.advancedView = false;
        this.redirectNode = null;
        this.loading = true;
        this.allowRemove = true;
        this.allowOpen = false;
        this.sortable = false;
        this.nodes = null;
        this.title = null;

        this.LoadDomains().then(() => {
            this.LoadEntry();
        });

        const $this = this;

        $scope.$watch('vm.entry.Culture', function (e) {
            if (!$this.loading) {
                if ($location.search()["mculture"])
                    $this.previousCulture = $location.search()["mculture"];
                $location.search('mculture', e);
            }
        });
    }

    public setRootNode(node) {
        this.entry.RedirectRootNodeId = node;
    }

    public openNodePicker() {
        const $this = this;
        var pickerOptions = {
            startNodeId: this.entry.RedirectRootNodeId,
            multiPicker: false,
            submit: function (model) {
                $this.entry.RedirectNodeName = model.selection[0].name;
                $this.entry.RedirectNodeId = model.selection[0].id;
                $this.entry.RedirectNodeIcon = model.selection[0].icon;

                $this.entityResource.getUrl($this.entry.RedirectNodeId, 'Document', $this.entry.Culture).then(
                    function (url) {
                        $this.entry.RedirectNodeUrl = url;
                    }
                );

                $this.editorService.close();
            },
            close: function (model) {
                $this.editorService.close();
            }
        }
        this.editorService.contentPicker(pickerOptions);
    }

    public removeRedirectNode() {
        this.entry.RedirectNodeName = this.entry.RedirectNodeId = this.entry.RedirectNodeIcon = null;
    }

    public saveChanges() {
        if (this.$scope.model.submit) {
            this.$scope.model.isNewEntry = this.isNewEntry;
            this.$scope.model.entry = this.entry;
            this.$scope.model.rootNodes = this.rootNodes;

            this.$location.search('mculture', this.previousCulture);

            this.$scope.model.submit(this.$scope.model);
        }
    }

    public close() {
        if (this.$scope.model.close) {
            this.$scope.model.rootNodes = this.rootNodes;

            this.$location.search('mculture', this.previousCulture);

            this.$scope.model.close(this.$scope.model);
        }
    }

    public updateLanguages() {
        if (this.entry.RedirectRootNodeId == 0 || this.entry.RedirectRootNodeId == -1 || this.rootNodes == null) {
            this.languages = null;
            this.entry.Culture = null;
        } else {
            var domain = this.rootNodes.find(n => n.id == this.entry.RedirectRootNodeId);

            if (domain != null && domain.domainLanguages != null) {
                this.languages = domain.domainLanguages;

                if (this.entry.Culture == null || !this.languages.find(x => x.IsoCode == this.entry.Culture)) {

                    if (this.languages.length > 0 && this.languages[0] != null) {
                        this.entry.Culture = this.languages[0].IsoCode;
                    }
                }
            } else {
                this.languages = null;
                this.entry.Culture = null;
            }
        }
    }

    private LoadDomains() {
        const $this = this;
        if (this.$scope.model.rootNodes != null) {
            return this.$q((resolve, reject) => {
                this.rootNodes = this.$scope.model.rootNodes;
                resolve();
            });
        } else {
            return this.$q((resolve, reject) => {
                this.entityResource.getChildren(-1, 'Document')
                    .then(function (rootNodes) {
                        var languagesPromise: angular.IPromise<any>[] = [];
                        $this.rootNodes = rootNodes;

                        if ($this.rootNodes != null) {
                            $this.rootNodes.forEach(function (n) {
                                languagesPromise.push($this.urlTrackerEntryService.getLanguagesOutNodeDomains(n.id).then((response) => {
                                    n.domainLanguages = response;
                                }));
                            });
                        }

                        $this.$q.all(languagesPromise).then(() => {
                            resolve();
                        });
                    });
            });
        }
    }

    private LoadEntry() {
        const $this = this;
        if (this.$scope.model.entry != null) {
            this.entry = this.$scope.model.entry;

            if (this.entry.Is404) {
                this.title = "Create redirect";
                this.entry.remove404 = true;
                this.entry.RedirectHttpCode = 301;
                this.entry.Notes = "Redirect created to prevent 404";
            } else {
                this.title = "Edit redirect";
                this.advancedView = true;
            }

            if (this.entry.RedirectNodeId && this.entry.RedirectNodeId != 0) {
                this.entityResource.getUrl(this.entry.RedirectNodeId, 'Document', this.entry.Culture).then(
                    function (url) {
                        $this.entry.RedirectNodeUrl = url;
                    }
                );

                this.entityResource.getById(this.entry.RedirectNodeId, 'Document', this.entry.Culture)
                    .then(function (redirectNode) {
                        $this.entry.RedirectNodeName = redirectNode.name;
                        $this.entry.RedirectNodeIcon = redirectNode.icon;
                    });
            }
        }
        else {
            this.title = "Create redirect";

            this.isNewEntry = true;
            this.advancedView = true;

            this.entry = {
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

            if (this.rootNodes != null) {
                this.entry.RedirectRootNodeId = this.rootNodes[0].id;
            }
        }

        this.updateLanguages();
        this.loading = false;
    }
}