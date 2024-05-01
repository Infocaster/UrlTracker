import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./explainRecommendations/explainRecommendations.lit";

@customElement("urltracker-inspect-recommendations-sidebar")
export class InspectRecommendationsSidebar extends AngularBridgeMixin(
  LitElement,
  html`<urltracker-sidebar-inspect-recommendations></urltracker-sidebar-inspect-recommendations>`
) {}
