import { IScope } from "@/models/scope.model";
import { createContext } from "@lit/context";

export const scopeContextKey = "scope";
export const scopeContext = createContext<IScope>(scopeContextKey);
