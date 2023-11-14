import { LitElement, css } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerRecommendationType } from "./recommendationTypeBase.mixin";

const baseType = UrlTrackerRecommendationType(
  LitElement,
  "urlTrackerRecommendationType_unknown"
);

@customElement("urltracker-recommendation-type-unknown")
export class UrlTrackerUnknownRecommendationType extends baseType {
  static styles = [
    ...baseType.styles,
    css`
      :host {
        font-style: italic;
        color: var(--uui-color-danger);
      }
    `,
  ];
}
