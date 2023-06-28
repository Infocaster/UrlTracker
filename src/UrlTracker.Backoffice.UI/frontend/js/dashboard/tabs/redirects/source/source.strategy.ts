import { IRedirectResponse } from "../../../../services/redirect.service";
import { StrategyResolver } from "../../../../util/tools/strategy/strategyresolver";
import { UnknownSourceStrategyFactory } from "./fallbacksource";

export interface IRedirectSourceStrategy {

    getTemplate(): unknown;
}

export interface IRedirectSourceStrategyFactory {

    getStrategy(redirect: IRedirectResponse): IRedirectSourceStrategy | undefined;
}

export const RedirectSourceStrategyResolver = StrategyResolver<IRedirectResponse, IRedirectSourceStrategy, IRedirectSourceStrategyFactory>;

export default new RedirectSourceStrategyResolver(new UnknownSourceStrategyFactory());