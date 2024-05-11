import { LitElement } from "lit";
import { IRedirectResponse } from "../../../../services/redirect.service";
import { UnsafeStrategyResolver } from "../../../../util/tools/strategy/strategyresolver";

export interface IRedirectSourceStrategyFactoryParameters {
    redirect: IRedirectResponse,
    element: LitElement
}

export interface IRedirectSourceStrategy {

    getTitle(): Promise<string>;
}

export interface IRedirectSourceStrategyFactory {

    getStrategy(parameters: IRedirectSourceStrategyFactoryParameters): IRedirectSourceStrategy | undefined;
}

export const RedirectSourceStrategyResolver = UnsafeStrategyResolver<IRedirectSourceStrategyFactoryParameters, IRedirectSourceStrategy>;

export default new RedirectSourceStrategyResolver();