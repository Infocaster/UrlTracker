import { createContext } from "@lit-labs/context";
import type { IEditorService } from "../umbraco/editor.service";
export type { IEditorService } from "../umbraco/editor.service";
export const editorServiceKey = "editorService";
export const editorServiceContext = createContext<IEditorService>(editorServiceKey);