declare namespace urltracker.services {

    interface IUrlResource {
        getUrl(key: string): string;
        verify<T>(promise: angular.IHttpPromise<T>, errorMessage: string): angular.IPromise<T>;
        download(url: string): angular.IPromise<undefined>;
    }

    interface IEntryService {
        addRedirect(entry: any): angular.IPromise<any>;
        addIgnore404(id: number): angular.IPromise<any>;
        getRedirects(skip: number, amount: number, query: string, sortType?: string): angular.IPromise<any>;
        getNotFounds(skip: number, amount: number, query: string, sortType?: string): angular.IPromise<any>;
        updateRedirect(entry: any): angular.IPromise<any>;
        deleteEntry(entryId: number, is404?: boolean): angular.IPromise<any>;
        getLanguagesOutNodeDomains(nodeId: number): angular.IPromise<any>;
        countNotFoundsThisWeek(): angular.IPromise<any>;
        getSettings(): angular.IPromise<any>;
        importRedirects(file: any): angular.IPromise<any>;
        exportRedirects(): angular.IPromise<any>;
    }
}