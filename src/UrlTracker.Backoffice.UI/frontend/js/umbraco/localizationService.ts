export interface ILocalizationService {
    localizeMany(keys: Array<string>) : Promise<Array<string>>;
}