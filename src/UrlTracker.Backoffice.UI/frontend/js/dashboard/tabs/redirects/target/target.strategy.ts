import { RedirectResponse } from '@/api';
import { StrategyResolver } from '../../../../util/tools/strategy/strategyresolver';
import { UnknownTargetStrategyFactory } from './implementations/fallbacktarget';

export interface IRedirectTargetStrategy {
  getTemplate(): unknown;
}

export interface IRedirectTargetStrategyFactory {
  getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy | undefined;
}

export const RedirectTargetStrategyResolver = StrategyResolver<RedirectResponse, IRedirectTargetStrategy>;

export default new RedirectTargetStrategyResolver(new UnknownTargetStrategyFactory());
