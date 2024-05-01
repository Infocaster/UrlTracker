import { recommendationsAnalysisServiceContext } from "@/context/recommendationsanalysis.context";
import recommendationsAnalysisService, { RecommendationsAnalysisService } from "@/services/recommendationanalysis.service";
import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { provide } from "@lit/context";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./analyseRecommendation/analyseRecommendation.lit";

@customElement("urltracker-analyse-recommendation-sidebar")
export class AnalyseRecommendationSidebar extends AngularBridgeMixin(
  LitElement,
  html`<urltracker-sidebar-analyse-recommendation></urltracker-sidebar-analyse-recommendation>`
) {
  @provide({ context: recommendationsAnalysisServiceContext })
  recommendationsAnalysisService: RecommendationsAnalysisService = recommendationsAnalysisService;
}
