export interface ILocalizationService {
    localizeMany(keys: Array<string>): angular.IPromise<Array<string>>;
    localize(value: string, tokens?: Array<string>, fallbackValue?: string): angular.IPromise<string>;
    tokenReplace(value: string, tokens: Array<string>): string;
}