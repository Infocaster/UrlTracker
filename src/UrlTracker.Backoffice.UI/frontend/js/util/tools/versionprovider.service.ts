import variableResource, { IVariableResource } from './variableresource.service';

export interface IVersionProvider {
  version: string;
}

export class VersionProvider implements IVersionProvider {
  constructor(private variableResource: IVariableResource) {}

  get version(): string {
    // Return a placeholder version - in a real implementation this would be fetched from the server
    return '16.0.0';
  }
}

export default new VersionProvider(variableResource);
