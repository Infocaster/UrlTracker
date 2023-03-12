export interface IUmbRequestHelper {

    resourcePromise<T>(promise: angular.IHttpPromise<T>, opts: string): angular.IPromise<T>;
}