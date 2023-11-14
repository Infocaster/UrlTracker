import { html } from "lit";
import { IVariableResource } from "../../../../util/tools/variableresource.service";
import { IRecommendationResponse } from "../../../../services/recommendation.service";
import {
  IRecommendationTypeStrategy,
  IRecommendationTypeStrategyFactory,
} from "./recommendation.strategy";
import { IRecommendationTypeStrategies } from "./recommendationType.constant";
import "./fileRecommendation.lit";

export class FileRecommendationTypeStrategyFactory
  implements IRecommendationTypeStrategyFactory
{
  constructor(private variableResource: IVariableResource) {}

  getStrategy(
    recommendation: IRecommendationResponse
  ): IRecommendationTypeStrategy | undefined {
    const key = this.variableResource.get<IRecommendationTypeStrategies>(
      "recommendationTypeStrategies"
    ).file;
    if (recommendation.strategy === key) {
      return {
        getTemplate() {
          return html`<urltracker-recommendation-type-file></urltracker-recommendation-type-file>`;
        },
      };
    }
  }
}
