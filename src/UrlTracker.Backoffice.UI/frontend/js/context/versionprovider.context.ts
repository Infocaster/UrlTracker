import { createContext } from "@lit-labs/context";
import type { IVersionProvider } from "../util/tools/versionprovider.service";
export type { IVersionProvider } from "../util/tools/versionprovider.service";
export const versionProviderKey = "versionProvider";
export const versionProviderContext = createContext<IVersionProvider>(versionProviderKey);