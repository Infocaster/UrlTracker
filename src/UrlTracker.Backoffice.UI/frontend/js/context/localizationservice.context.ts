import { createContext } from "@lit/context";
import type { ILocalizationService } from "../umbraco/localization.service";
export type { ILocalizationService } from "../umbraco/localization.service";
export const localizationServiceKey = "localizationService";
export const localizationServiceContext = createContext<ILocalizationService>(
  localizationServiceKey
);
