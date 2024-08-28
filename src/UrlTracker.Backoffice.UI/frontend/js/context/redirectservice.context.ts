import { createContext } from '@lit/context';
import type { IRedirectService } from '../services/redirect.service';
export type { IRedirectService } from '../services/redirect.service';
export const redirectServiceKey = 'redirectService';
export const redirectServiceContext = createContext<IRedirectService>(redirectServiceKey);
