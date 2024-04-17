import { createContext } from "@lit/context";
import { ITab } from "../dashboard/tab";
export const tabs = [] as ITab[];
export const tabContext = createContext<ITab[]>(tabs);
