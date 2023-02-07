declare var Umbraco: any;

export interface IVersionProvider {
    get version(): string
}

export class VersionProvider implements IVersionProvider {

    public static alias: string = "urlTrackerVersionProvider";

    get version(): string {
        
        return "10.3.1";
        //return Umbraco.Sys.ServerVariables["urlTracker"].version;
    }
}