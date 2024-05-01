import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./analyseRecommendation/analyseRecommendation.lit";

@customElement("urltracker-analyse-recommendation-sidebar")
export class AnalyseRecommendationSidebar extends AngularBridgeMixin(
  LitElement,
  html`<urltracker-sidebar-analyse-recommendation></urltracker-sidebar-analyse-recommendation>`
) {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }
}
