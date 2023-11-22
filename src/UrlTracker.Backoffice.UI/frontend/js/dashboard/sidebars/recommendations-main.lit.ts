import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./recommendations/recommendation.lit";

@customElement("urltracker-recommendations-sidebar")
export class RecommendationsSidebar extends AngularBridgeMixin(
  LitElement,
  html`<urltracker-sidebar-recommendations></urltracker-sidebar-recommendations>`
) {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }
}
