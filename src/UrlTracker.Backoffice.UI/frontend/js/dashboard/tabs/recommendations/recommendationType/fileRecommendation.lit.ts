import { LitElement } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerRecommendationType } from "./recommendationTypeBase.mixin";

@customElement("urltracker-recommendation-type-file")
export class UrlTrackerFileRecommendationType extends UrlTrackerRecommendationType(
  LitElement,
  "urlTrackerRecommendationType_file"
) {}
