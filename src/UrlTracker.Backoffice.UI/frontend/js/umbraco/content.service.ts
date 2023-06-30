export interface IContentItem {

}

export interface IContentResource {

    getById(id: number, type: string): angular.IPromise<IContentItem>;
}