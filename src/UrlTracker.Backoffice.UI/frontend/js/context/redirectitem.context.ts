import { createContext } from "@lit-labs/context";
import type { IRedirectResponse } from "../services/redirect.service";
export type { IRedirectResponse } from "../services/redirect.service";
export const redirectKey = 'redirect';
export const redirectContext = createContext<IRedirectResponse>(redirectKey);