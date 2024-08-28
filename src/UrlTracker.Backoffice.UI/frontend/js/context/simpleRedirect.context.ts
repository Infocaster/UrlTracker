import { createContext } from '@lit/context';
import type { ITypeButton } from '../util/elements/redirects/simpleRedirect/simpleRedirectTypeProvider';
export type { ITypeButton } from '../util/elements/redirects/simpleRedirect/simpleRedirectTypeProvider';
export const simpleRedirectContext = createContext<ITypeButton[]>('simpleRedirect');
