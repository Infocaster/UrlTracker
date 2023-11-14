import { html } from "lit";
import { IVariableResource } from "../../../../util/tools/variableresource.service";
import { IRecommendationResponse } from "../../../../services/recommendation.service";
import {
  IRecommendationTypeStrategy,
  IRecommendationTypeStrategyFactory,
} from "./recommendation.strategy";
import { IRecommendationTypeStrategies } from "./recommendationType.constant";
import "./technicalFileRecommendation.lit";

export class TechnicalFileRecommendationTypeStrategyFactory
  implements IRecommendationTypeStrategyFactory
{
  constructor(private variableResource: IVariableResource) {}

  getStrategy(
    recommendation: IRecommendationResponse
  ): IRecommendationTypeStrategy | undefined {
    const key = this.variableResource.get<IRecommendationTypeStrategies>(
      "recommendationTypeStrategies"
    ).technicalFile;
    if (recommendation.strategy === key) {
      return {
        getTemplate() {
          return html`<urltracker-recommendation-type-technical-file></urltracker-recommendation-type-technical-file>`;
        },
      };
    }
  }
}
