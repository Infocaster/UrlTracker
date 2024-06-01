import { createContext } from '@lit/context';

export interface IRedirectViewContext {
  advanced: boolean;
}

export const redirectViewKey = 'redirectviewContext';
export const redirectViewContext = createContext<IRedirectViewContext>(redirectViewKey);
