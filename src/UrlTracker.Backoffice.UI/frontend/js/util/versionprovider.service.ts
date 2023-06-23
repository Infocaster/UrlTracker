export interface IVersionProvider {
    get version(): string
}

export class VersionProvider implements IVersionProvider {

    get version(): string {
        
        return "10.3.1";
        //return Umbraco.Sys.ServerVariables["urlTracker"].version;
    }
}

export default new VersionProvider();