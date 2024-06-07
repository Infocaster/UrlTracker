import { UnsafeStrategyResolver } from '../../../../util/tools/strategy/strategyresolver';
import { RedirectResponse } from '@/api';
import { UmbElement } from '@umbraco-cms/backoffice/element-api';
import { LitElement } from '@umbraco-cms/backoffice/external/lit';

export interface IRedirectSourceStrategyFactoryParameters {
  redirect: RedirectResponse;
  element: UmbElement & LitElement;
}

export interface IRedirectSourceStrategy {
  getTitle(): Promise<string>;
}

export interface IRedirectSourceStrategyFactory {
  getStrategy(parameters: IRedirectSourceStrategyFactoryParameters): IRedirectSourceStrategy | undefined;
}

export const RedirectSourceStrategyResolver = UnsafeStrategyResolver<
  IRedirectSourceStrategyFactoryParameters,
  IRedirectSourceStrategy
>;

export default new RedirectSourceStrategyResolver();
