import UmbServerVariableContext, { ServerVariables } from '@/context/servervariables.context';

export interface IVariableResource {
  get<T>(key: keyof Partial<ServerVariables>, validator?: (obj: unknown) => obj is T): T;
}

class VariableResource implements IVariableResource {
  get<T>(key: keyof Partial<ServerVariables>, validator?: (obj: unknown) => obj is T): T {
    const serverVariablesContext = new UmbServerVariableContext();

    const value = serverVariablesContext.serverVariables.getValue();

    const result = value[key] as unknown;

    if (validator && !validator(result)) throw new Error('variable is not of the right type');

    return result as T;
  }
}

export default new VariableResource();
