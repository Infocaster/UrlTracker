import { LitElement } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerRecommendationType } from "./recommendationTypeBase.mixin";

@customElement("urltracker-recommendation-type-page")
export class UrlTrackerPageRecommendationType extends UrlTrackerRecommendationType(
  LitElement,
  "urlTrackerRecommendationType_page"
) {}
