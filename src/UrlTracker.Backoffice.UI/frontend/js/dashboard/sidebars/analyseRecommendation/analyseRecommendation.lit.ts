import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { IRecommendationsAnalysisService, recommendationsAnalysisServiceContext } from "@/context/recommendationsanalysis.context";
import { scopeContext } from "@/context/scope.context";
import { IScope } from "@/models/scope.model";
import { IRecommendationResponse } from "@/services/recommendation.service";
import { IRecommendationHistoryResponse, IRecommendationReferrerResponse } from "@/services/recommendationanalysis.service";
import { ensureExists, ensureServiceExists } from "@/util/tools/existancecheck";
import { consume } from "@lit/context";
import { LitElement, css, html, nothing } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import recommendationTypeStrategyResolver from "../../tabs/recommendations/recommendationType/recommendation.strategy";

import './historyChart.lit';
import './referrersChart.lit';
import { ifDefined } from "lit/directives/if-defined.js";
import { cardWithClickableHeader } from "@/dashboard/tabs/styles";

export const ContentElementTag = "urltracker-sidebar-analyse-recommendation";

@customElement(ContentElementTag)
export class UrlTrackerSidebarAnalyseRecommendation extends LitElement {
  private recommendationTypeStrategy = recommendationTypeStrategyResolver;

  @consume({ context: recommendationsAnalysisServiceContext })
  private recommendationsAnalysisService?: IRecommendationsAnalysisService;

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @consume({ context: scopeContext })
  private $scope?: IScope;

  @property({ attribute: false})
  get scope() { 
    ensureExists(this.$scope, "scope");
    return this.$scope;
  }

  @state()
  private data!: IRecommendationResponse;

  @state()
  private _subText = "";

  @state()
  private referrers: IRecommendationReferrerResponse | null = null;

  @state()
  private history: IRecommendationHistoryResponse | null = null;

  @state()
  private recommendationTypeText?: string;

  @state()
  private recommendationTypeIsError: boolean = false;

  @state()
  private recommendationTypeDescription?: string;

  private renderRecommendationType(): unknown {
    if (!this.recommendationTypeText) return nothing;
    let errorClass: string | undefined;

    if (this.recommendationTypeIsError) {
      errorClass = 'error';
    }

    return html`
      <h3 class="${ifDefined(errorClass)}">${this.recommendationTypeText}</h3>
    `;
  }

  async connectedCallback(): Promise<void> {
      super.connectedCallback();

      ensureServiceExists(this.recommendationsAnalysisService, "recommendationsAnalysisService");
      ensureServiceExists(this.localizationService, "localizationService");

      this.data = this.scope.model.value;
      this._subText = this.scope.model.value.url ?? "";

      const referrersPromise = this.recommendationsAnalysisService.getReferrers(this.data);
      const historyPromise = this.recommendationsAnalysisService.getHistory(this.data);

      const [referrers, history] = await Promise.all([referrersPromise, historyPromise]).catch((error) => {
        throw new Error(`Failed to fetch referrers and history for recommendation ${this.data.id}: ${error}`);
      });
      
      this.referrers = referrers;
      this.history = history;

      const sourceStrategy = recommendationTypeStrategyResolver.getStrategy({recommendation: this.data, element: this});
      if (sourceStrategy) {
        this.recommendationTypeText = await sourceStrategy.getTitle();
        this.recommendationTypeDescription = await sourceStrategy.getDescription();
        this.recommendationTypeIsError = false;
      }
      else {
  
        this.recommendationTypeText = await this.localizationService.localize(
          "urlTrackerRecommendationType_unknown"
        );
        this.recommendationTypeIsError = true;
      }
  }

  close() {
    this.scope.model.close();
  }

  protected renderHistoryChart() {
    if (!this.history?.dailyOccurances?.length) return html`<i>No data available</i>`;
    return html`
     <urltracker-history-chart .history=${this.history}></urltracker-history-chart>
    `;
  }

  protected renderReferrersChart() {
    if (!this.referrers?.length) return html`<i>No data available</i>`;
    return html`
     <urltracker-referrers-chart .referrers=${this.referrers}></urltracker-referrers-chart>
    `;
  }

  protected render() {
    return html`
      <div class="header">
        <h2>${this.renderRecommendationType()}</h2>
        <span>${this._subText}</span>
      </div>
      <div class="main">
        <uui-box>
          <p>
          ${this.recommendationTypeDescription}
          </p>
          <h6>History (last 20 days)</h6>
          ${this.renderHistoryChart()}
          <h6>Most common referrers</h6>
          ${this.renderReferrersChart()}
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}
          >Close</uui-button
        >
      </div>
    `;
  }

  static styles = [
    cardWithClickableHeader,
    css`
  :host {
    display: flex;
    flex-direction: column;
    height: 100vh;
  }

  .header {
    padding: 10px 20px;
    background-color: white;
    box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
  }

  h2 {
    font-size: 16px;
    font-weight: 700;
    line-height: 20px;
    margin: 0;
    display: block;
  }

  h6 {
    font-size: 15px;
    font-weight: 700;
    line-height: 20px;
    margin: 0;
    display: block;
  }

  span {
    font-size: 12px;
    font-weight: 400;
    line-height: 15px;
    color: #68676B;
  }

  .main {
    flex: 1;
    padding: 16px 20px;
  }

  uui-box {
    margin-bottom: 1rem;
    font-family: lato, sans-serif;
    font-weight: 400;
    font-size: 15px;
    line-height: 1.25;
  }

  ul, p {
    margin-top: 0;
  }

  uui-box uui-button {
    width: 100%;
  }

  .footer {
    display: flex;
    justify-content: flex-end;
    background-color: white;
    padding: 10px 20px;
    box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
  }
`];
}
