import { createContext } from "@lit/context";
import type { ITypeButton } from "./simpleRedirectTypeProvider";
export type { ITypeButton } from "./simpleRedirectTypeProvider";
export const simpleRedirectContext =
  createContext<ITypeButton[]>("simpleRedirect");
