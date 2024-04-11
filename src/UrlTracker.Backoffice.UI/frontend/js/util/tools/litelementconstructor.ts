import { LitElement } from "lit";

export type LitElementConstructor<T = LitElement> = new (...args: any[]) => T;