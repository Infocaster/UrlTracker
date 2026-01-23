import { createContext } from '@lit/context';
import type { RedirectResponse } from '../../../api-client/types.gen';
export type { RedirectResponse } from '../../../api-client/types.gen';
export const redirectKey = 'redirect';
export const redirectContext = createContext<RedirectResponse>(redirectKey);
