interface IUmbracoGlobals {

    Sys: IUmbracoGlobalsSys;
}

interface IUmbracoGlobalsSys {

    ServerVariables: IUmbracoServerVariables;
}

interface IUmbracoServerVariables {

    urlTracker: IUrlTrackerServerVariables;
}

interface IUrlTrackerServerVariables {
    
    [key: string]: unknown
}

declare const Umbraco: IUmbracoGlobals;

export interface IVariableResource {
    get<T>(key: string, validator?: (obj: unknown) => obj is T): T
}

class VariableResource implements IVariableResource {
    
    get<T>(key: string, validator?: (obj: unknown) => obj is T): T {

        const result = Umbraco.Sys.ServerVariables.urlTracker[key];

        if (validator && !validator(result)) throw new Error("variable is not of the right type");

        return result as T;
    }
}

export default new VariableResource();