import { LitElement } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerRecommendationType } from "./recommendationTypeBase.mixin";

@customElement("urltracker-recommendation-type-image")
export class UrlTrackerImageRecommendationType extends UrlTrackerRecommendationType(
  LitElement,
  "urlTrackerRecommendationType_image"
) {}
