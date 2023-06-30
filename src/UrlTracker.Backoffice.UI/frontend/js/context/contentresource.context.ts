import { createContext } from "@lit-labs/context";
import type { IContentResource } from "../umbraco/content.service";
export type { IContentResource } from "../umbraco/content.service";
export const contentResourceKey = 'contentResource';
export const contentResourceContext = createContext<IContentResource>(contentResourceKey);