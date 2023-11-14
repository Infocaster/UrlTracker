import { IRecommendationResponse } from "../../../../services/recommendation.service";
import { StrategyResolver } from "../../../../util/tools/strategy/strategyresolver";
import { UnknownRecommendationTypeFactory } from "./fallbackRecommendationType";

export interface IRecommendationTypeStrategy {
  getTemplate(): unknown;
}

export interface IRecommendationTypeStrategyFactory {
  getStrategy(
    recommendation: IRecommendationResponse
  ): IRecommendationTypeStrategy | undefined;
}

export const RecommendationTypeStrategyResolver = StrategyResolver<
  IRecommendationResponse,
  IRecommendationTypeStrategy,
  IRecommendationTypeStrategyFactory
>;

export default new RecommendationTypeStrategyResolver(
  new UnknownRecommendationTypeFactory()
);
