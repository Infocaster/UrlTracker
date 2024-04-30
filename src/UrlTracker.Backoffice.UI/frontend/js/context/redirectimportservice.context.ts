import { createContext } from "@lit/context";
import type { IRedirectImportService } from "../services/redirectimport.service";
export type { IRedirectImportService } from "../services/redirectimport.service";
export const redirectImportServiceKey = "redirectImportService";
export const redirectImportServiceContext =
  createContext<IRedirectImportService>(redirectImportServiceKey);
