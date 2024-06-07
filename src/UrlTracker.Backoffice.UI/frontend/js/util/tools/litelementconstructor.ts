import { LitElement } from '@umbraco-cms/backoffice/external/lit';

export type LitElementConstructor<T = LitElement> = new (...args: any[]) => T;
