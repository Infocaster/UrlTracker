import { LitElement } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerRecommendationType } from "./recommendationTypeBase.mixin";

@customElement("urltracker-recommendation-type-technical-file")
export class UrlTrackerTechnicalFileRecommendationType extends UrlTrackerRecommendationType(
  LitElement,
  "urlTrackerRecommendationType_technicalFile"
) {}
