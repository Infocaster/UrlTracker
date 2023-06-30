import { createContext } from "@lit-labs/context";
import type { ITargetService } from "../dashboard/tabs/redirects/target/target.service";
export type { ITargetService } from "../dashboard/tabs/redirects/target/target.service";
export const redirectTargetServiceKey = 'redirectTargetService';
export const redirectTargetServiceContext = createContext<ITargetService>(redirectTargetServiceKey);