import { LitElement } from 'lit';
import { IRecommendationResponse } from '../../../../services/recommendation.service';
import { UnsafeStrategyResolver } from '../../../../util/tools/strategy/strategyresolver';

export interface IRecommendationTypeStrategy {
  getTitle(): Promise<string>;
  getDescription(): Promise<string>;
}

export interface IRecommendationTypeStrategyFactoryParameters {
  recommendation: IRecommendationResponse;
  element: LitElement;
}

export interface IRecommendationTypeStrategyFactory {
  getStrategy(parameters: IRecommendationTypeStrategyFactoryParameters): IRecommendationTypeStrategy | undefined;
}

export const RecommendationTypeStrategyResolver = UnsafeStrategyResolver<
  IRecommendationTypeStrategyFactoryParameters,
  IRecommendationTypeStrategy
>;

export default new RecommendationTypeStrategyResolver();
