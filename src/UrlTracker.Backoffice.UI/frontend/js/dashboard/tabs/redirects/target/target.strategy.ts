import { IRedirectResponse } from "../../../../services/redirect.service";
import { StrategyResolver } from "../../../../util/tools/strategy/strategyresolver";
import { UnknownTargetStrategyFactory } from "./implementations/fallbacktarget";

export interface IRedirectTargetStrategy {
    
    getTemplate(): unknown;
}

export interface IRedirectTargetStrategyFactory {

    getStrategy(redirect: IRedirectResponse): IRedirectTargetStrategy | undefined;
}

export const RedirectTargetStrategyResolver = StrategyResolver<IRedirectResponse, IRedirectTargetStrategy>;

export default new RedirectTargetStrategyResolver(new UnknownTargetStrategyFactory());