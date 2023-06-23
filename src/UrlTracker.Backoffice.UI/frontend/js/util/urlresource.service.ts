declare const Umbraco: any;

export interface IUrlResource {

    getController(controller: string): IControllerUrlResource;
}

interface IControllerDefinition {

    base: string;
    [key: string]: string;
}

export interface IControllerUrlResource {

    getUrl(endpoint: string): string;
}

export class UrlResource implements IUrlResource {

    public getController(controller: string): IControllerUrlResource {

        let def = Umbraco.Sys.ServerVariables.urlTracker[controller];

        if (!this.isControllerDefinition(def)) {
            throw Error("Could not find a definition for controller named: " + controller);
        }

        return new ControllerUrlResource(def);
    }

    private isControllerDefinition(obj: any): obj is IControllerDefinition {

        return "base" in obj;
    }
}

class ControllerUrlResource implements IControllerUrlResource {

    constructor(private controller: IControllerDefinition) { }

    public getUrl(endpoint: string): string {

        return this.controller.base + this.controller[endpoint];
    }
}

export default new UrlResource();