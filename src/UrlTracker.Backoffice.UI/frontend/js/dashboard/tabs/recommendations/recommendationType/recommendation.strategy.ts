import { RecommendationResponse } from '@/api';
import { UnsafeStrategyResolver } from '../../../../util/tools/strategy/strategyresolver';
import { UmbElement } from '@umbraco-cms/backoffice/element-api';

export interface IRecommendationTypeStrategy {
  getTitle(): Promise<string>;
  getDescription(): Promise<string>;
}

export interface IRecommendationTypeStrategyFactoryParameters {
  recommendation: RecommendationResponse;
  element: UmbElement;
}

export interface IRecommendationTypeStrategyFactory {
  getStrategy(parameters: IRecommendationTypeStrategyFactoryParameters): IRecommendationTypeStrategy | undefined;
}

export const RecommendationTypeStrategyResolver = UnsafeStrategyResolver<
  IRecommendationTypeStrategyFactoryParameters,
  IRecommendationTypeStrategy
>;

export default new RecommendationTypeStrategyResolver();
