declare const Umbraco: any;

export interface IVariableResource {
    get<T>(key: string, validator?: (obj: any) => obj is T): T
}

class VariableResource implements IVariableResource {
    
    get<T>(key: string, validator?: (obj: any) => obj is T): T {

        let result = Umbraco.Sys.ServerVariables.urlTracker[key];

        if (validator && !validator(result)) throw new Error("variable is not of the right type");

        return result;
    }
}

export default new VariableResource();