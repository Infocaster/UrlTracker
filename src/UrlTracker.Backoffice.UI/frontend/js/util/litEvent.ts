export interface ILitEvent<T> extends angular.IAngularEvent
{
    detail: T;
}