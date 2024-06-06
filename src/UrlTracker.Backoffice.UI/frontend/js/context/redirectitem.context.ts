import { createContext } from '@lit/context';
import type { RedirectResponse } from '@/api';
export type { RedirectResponse } from '@/api';
export const redirectKey = 'redirect';
export const redirectContext = createContext<RedirectResponse>(redirectKey);
