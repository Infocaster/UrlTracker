import { createContext } from "@lit-labs/context";
import type { ILocalizationService } from "../umbraco/localizationService";
export type { ILocalizationService } from "../umbraco/localizationService";
export const localizationServiceKey = 'localizationService';
export const localizationServiceContext = createContext<ILocalizationService>(localizationServiceKey);