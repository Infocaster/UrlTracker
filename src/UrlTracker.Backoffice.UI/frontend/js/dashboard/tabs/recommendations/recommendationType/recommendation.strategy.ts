import { LitElement } from 'lit';
import { RecommendationResponse } from '../../../../api-client';
import { UnsafeStrategyResolver } from '../../../../util/tools/strategy/strategyresolver';

export interface IRecommendationTypeStrategy {
  getTitle(): string;
  getDescription(): string;
  typeKey: string;
}

export interface IRecommendationTypeStrategyFactoryParameters {
  recommendation: RecommendationResponse;
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
