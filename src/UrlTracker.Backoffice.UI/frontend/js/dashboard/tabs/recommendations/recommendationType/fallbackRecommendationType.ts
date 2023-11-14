import { html } from "lit";
import "./fallbackRecommendationType.lit";
import {
  IRecommendationTypeStrategy,
  IRecommendationTypeStrategyFactory,
} from "./recommendation.strategy";

export class UnknownRecommendationTypeFactory
  implements IRecommendationTypeStrategyFactory
{
  getStrategy(): IRecommendationTypeStrategy | undefined {
    return {
      getTemplate() {
        return html`<urltracker-recommendation-type-unknown></urltracker-recommendation-type-unknown>`;
      },
    };
  }
}
