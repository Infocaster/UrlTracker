import { createContext } from "@lit/context";
import type { IIconHelper } from "../umbraco/icon.service";
export type { IIconHelper } from "../umbraco/icon.service";
export const iconHelperKey = "iconHelper";
export const iconHelperContext = createContext<IIconHelper>(iconHelperKey);
