declare var Umbraco: any; // umbraco's system variable has no particular type assigned to itself

export class UrlResource implements urltracker.services.IUrlResource {

    public static $inject = ["umbRequestHelper"];
    constructor(private umbRequestHelper: any) { }

    public verify<T>(promise: angular.IHttpPromise<T>, errorMessage: string): angular.IPromise<T> {

        this.umbRequestHelper.resourcePromise(promise, errorMessage);
        return promise.then((response) => response.data);
    }

    public getUrl(key: string): string {

        const action = this.urlTrackerSection[key];
        if (!action) throw new Error("no action associated with given key");

        return this.urlBase + action;
    }

    public download(url: string): angular.IPromise<undefined> {

        return this.umbRequestHelper.downloadFile(url);
    }

    private get urlBase(): string { return this.urlTrackerSection.base; }

    get urlTrackerSection(): urltracker.system.ISystemSection { return Umbraco.Sys.ServerVariables["urlTracker"] as urltracker.system.ISystemSection; }
}