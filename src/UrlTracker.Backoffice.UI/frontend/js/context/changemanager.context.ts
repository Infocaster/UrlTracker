import { createContext } from "@lit/context";
import type { IChangeManager } from "../util/tools/changemanager";
export type { IChangeManager } from "../util/tools/changemanager";
export const changeManagerKey = "changeManager";
export const changeManagerContext =
  createContext<IChangeManager>(changeManagerKey);
