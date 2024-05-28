import variableResource, { IVariableResource } from "./variableresource.service";

export interface IUrlResource {

    getController(controller: string): IControllerUrlResource;
}

interface IControllerDefinition {

    base: string;
    [key: string]: string;
}

type UriComponentValue = string | number | boolean;

type EndpointTemplateFactory = (args?: Record<string, UriComponentValue>) => string;

export interface IControllerUrlResource {

    getUrl(endpoint: string, args?: Record<string, UriComponentValue>): string;
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

    private endpointFactoryCache: Record<string, EndpointTemplateFactory | string> = {};
    static readonly templateExpression = /\{(?<name>\w+)\}/;

    constructor(private controller: IControllerDefinition) { }

    public getUrl(endpoint: string, args?: Record<string, UriComponentValue>): string {

        if (!(endpoint in this.endpointFactoryCache)) {
            const factory = this.tryCreateFactory(endpoint);
            if (!factory) throw new Error(`Could not produce an endpoint URL for endpoint '${endpoint}'`);
    
            this.endpointFactoryCache[endpoint] = factory;
        }
        
        const result = this.endpointFactoryCache[endpoint];
        if (typeof result === 'string' || result instanceof String) return result as string;
        return result(args);
    }

    private tryCreateFactory(endpoint: string): EndpointTemplateFactory | string | undefined {

        if (!(endpoint in this.controller)) return undefined

        // Since we're reading from a public variable, we need to make sure that the endpoint value conforms to our expectations
        const rawEndpoint = this.controller[endpoint];
        this.ensureValidRawEndpoint(rawEndpoint);

        // If an endpoint doesn't have any template variables, then the factory simply returns the raw value
        const hasMatches = ControllerUrlResource.templateExpression.test(rawEndpoint);
        if (!hasMatches) return rawEndpoint;

        // If and endpoint DOES have template variables, the result becomes the aggregate of several operations
        // A template is always paired with a constant prefix, and optionally ends with a constant suffix
        // For example: /lorem/{id}/ipsum -> (/lorem/) ({id}) (/ipsum).
        let components = this.dissectRawEndpoint(rawEndpoint);
        if (!components) return undefined;

        return this.createFactoryFromComponents(components);
    }

    private ensureValidRawEndpoint(rawEndpoint: string): void {

        /* BUSINESS RULES:
         * A raw endpoint conforms to these rules:
         * - may contain opening and closing curly braces: { }
         * - may contain letters and numbers
         * - may contain forward slash
         * - starts with a forward slash
         * - Does not contain any other characters
         */
        if (!rawEndpoint.startsWith('/')) throw new Error('The endpoint does not start with a forward slash');
        if (!/^\/[\w\{\}\/]+$/.test(rawEndpoint)) throw new Error('The endpoint does not conform to the expected pattern');
    }

    private createFactoryFromComponents(components: (string | EndpointTemplateFactory)[]): (args?: Record<string, UriComponentValue>) => string {

        return (args?: Record<string, UriComponentValue>): string => {

            let result = '';
            for (let index = 0; index < components.length; index++) {

                const component = components[index];
                if (typeof component === 'string' || component instanceof String) {
                    result += component;
                }
                else {
                    result += component(args);
                }
            }

            return result;
        }
    }

    private dissectRawEndpoint(rawEndpoint: string): (string | EndpointTemplateFactory)[] | undefined {

        let index = 0;
        let match: RegExpExecArray | null;
        let components: (string | EndpointTemplateFactory)[] = [];
        const expr = new RegExp(ControllerUrlResource.templateExpression, 'g');
        while ((match = expr.exec(rawEndpoint)) != null) {

            if (index !== match.index) {

                components.push(rawEndpoint.substring(index, match.index))
            }

            const variableName = match.groups?.name;
            if (!variableName) return undefined;

            components.push((args?: Record<string, UriComponentValue>) => {

                if (!args) throw new Error('No template parameters provided');

                const result = args[variableName]
                if (!result) throw new Error('template parameter not present: ' + variableName);

                return encodeURIComponent(result);
            });

            index = match.index + variableName.length + 2;
        }

        if (index < rawEndpoint.length) {
            components.push(rawEndpoint.substring(index));
        }

        return components;
    }
}

export default new UrlResource(variableResource);