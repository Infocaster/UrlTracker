import { createContext } from "@lit-labs/context";
import type { TabService } from "../services/tabs.service";
export type { TabService } from "../services/tabs.service";
export const tabServiceKey = "tabService";
export const tabServiceContext = createContext<TabService>(tabServiceKey);