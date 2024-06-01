import { IVariableResource } from '../../../../util/tools/variableresource.service';
import {
  IRecommendationTypeStrategy,
  IRecommendationTypeStrategyFactory,
  IRecommendationTypeStrategyFactoryParameters,
} from './recommendation.strategy';
import { IRecommendationTypeStrategies } from './recommendationType.constant';
import { UrlTrackerRecommendationType } from './recommendationTypeBase.mixin';

export class TechnicalFileRecommendationTypeStrategyFactory implements IRecommendationTypeStrategyFactory {
  constructor(private variableResource: IVariableResource) {}

  getStrategy(parameters: IRecommendationTypeStrategyFactoryParameters): IRecommendationTypeStrategy | undefined {
    const key = this.variableResource.get<IRecommendationTypeStrategies>('recommendationTypeStrategies').technicalFile;

    if (parameters.recommendation.strategy === key) {
      return new UrlTrackerRecommendationType(
        parameters.element,
        'urlTrackerRecommendationType_technicalFile',
        'urlTrackerRecommendationType_technicalFiledescription',
      );
    }
  }
}
