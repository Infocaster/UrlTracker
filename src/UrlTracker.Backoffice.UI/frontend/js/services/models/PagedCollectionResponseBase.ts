import { ICollectionResponseBase } from "./CollectionResponseBase";

export interface IPagedCollectionResponseBase<T> extends ICollectionResponseBase<T> {

    total: number;
}