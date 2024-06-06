import { UnsafeStrategyResolver } from '../../../../util/tools/strategy/strategyresolver';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { RedirectResponse } from '@/api';

export interface IRedirectSourceStrategyFactoryParameters {
  redirect: RedirectResponse;
  element: UmbLitElement;
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
