import variableResource, { IVariableResource } from "./variableresource.service";

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

    constructor(private _variableResource: IVariableResource) { }

    public getController(controller: string): IControllerUrlResource {

        return new ControllerUrlResource(this._variableResource.get(controller, this.isControllerDefinition));
    }

    private isControllerDefinition = (obj: unknown): obj is IControllerDefinition => {

        return obj instanceof Object && "base" in obj;
    }
}

class ControllerUrlResource implements IControllerUrlResource {

    constructor(private controller: IControllerDefinition) { }

    public getUrl(endpoint: string): string {

        return this.controller.base + this.controller[endpoint];
    }
}

export default new UrlResource(variableResource);