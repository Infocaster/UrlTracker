import { LitElement } from 'lit';
import type { RedirectResponse } from '../../../../../../api-client/types.gen';
import { UnsafeStrategyResolver } from '../../../../util/tools/strategy/strategyresolver';

export interface IRedirectSourceStrategyFactoryParameters {
  redirect: RedirectResponse;
  element: LitElement;
}

export interface IRedirectSourceStrategy {
  getTitle(): string;
}

export interface IRedirectSourceStrategyFactory {
  getStrategy(parameters: IRedirectSourceStrategyFactoryParameters): IRedirectSourceStrategy | undefined;
}

export const RedirectSourceStrategyResolver = UnsafeStrategyResolver<
  IRedirectSourceStrategyFactoryParameters,
  IRedirectSourceStrategy
>;

export default new RedirectSourceStrategyResolver();
