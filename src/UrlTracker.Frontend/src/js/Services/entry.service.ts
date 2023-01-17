export class EntryService implements urltracker.services.IEntryService {

    public static $inject = ["$http", "$log", "Upload", "urlTrackerUrlResource"];
    constructor(private $http: angular.IHttpService, private $log: angular.ILogService, private Upload: any, private urlResource: urltracker.services.IUrlResource) { }

    public addRedirect(entry) {

        entry.is404 = false;
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("addRedirect"),
            method: "POST",
            data: entry
        }), "Failed to add redirect");
    }

    public addIgnore404(id) {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("addIgnore404"),
            method: "POST",
            data: {
                id: id
            }
        }), "Failed to ignore 404");
    }

    public getRedirects(skip, amount, query, sortType: string = "CreatedDesc") {

        if (!sortType) sortType = "CreatedDesc";

        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("getRedirects"),
            method: "GET",
            params: {
                skip: skip,
                amount: amount,
                query: query,
                sortType: sortType
            }
        }), "Failed to get redirects");
    }

    public getNotFounds(skip, amount, query, sortType = "LastOccurredDesc") {
        if (!sortType)
            sortType = "LastOccurredDesc";

        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("getNotFounds"),
            method: "GET",
            params: {
                skip: skip,
                amount: amount,
                query: query,
                sortType: sortType
            }
        }), "Failed to get not founds");
    }

    public updateRedirect(entry) {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("updateRedirect"),
            method: "POST",
            data: entry
        }), "Failed to update redirect");
    }

    public deleteEntry(entryId, is404 = false) {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("deleteEntry"),
            method: "POST",
            params: {
                id: entryId,
                is404: is404
            }
        }), "Failed to delete entry");
    }

    public deleteRedirect(id) {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("deleteRedirect") + `/${id}`,
            method: "POST"
        }), "Failed to delete redirect");
    }

    public getLanguagesOutNodeDomains(nodeId) {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("getLanguagesOutNodeDomains"),
            method: "GET",
            params: {
                nodeId: nodeId
            }
        }), "Failed to get languages");
    }

    public getNodesWithDomains() {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("getNodesWithDomains"),
            method: "GET"
        }), "Failed to get nodes with domains");
    }

    public countNotFoundsThisWeek() {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("countNotFoundsThisWeek"),
            method: "GET",
        }), "Failed to get amount of not founds");
    }

    public getSettings() {
        return this.urlResource.verify(this.$http({
            url: this.urlResource.getUrl("getSettings"),
            method: "GET",
        }), "Failed to get settings");
    }

    public importRedirects(file: any) {
        return this.urlResource.verify(this.Upload.upload({
            url: this.urlResource.getUrl("importRedirects"),
            file: file,
            fields: {}
        }), "Failed to import redirects");
    }

    public exportRedirects() {

        return this.urlResource.download(this.urlResource.getUrl("exportRedirects"));
    }
}
