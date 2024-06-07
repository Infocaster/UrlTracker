import { ensureExists } from '@/util/tools/existancecheck';
import { css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { customElement, property, state } from 'lit/decorators.js';
import recommendationTypeStrategyResolver from '../../tabs/recommendations/recommendationType/recommendation.strategy';

import './historyChart.lit';
import './referrersChart.lit';
import { ifDefined } from 'lit/directives/if-defined.js';
import { cardWithClickableHeader } from '@/dashboard/tabs/styles';
import { IAnalyseRecommendationModel } from './analyserecommendation';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import {
  GetApiV1UrlTrackerRecommendationAnalysisByRecommendationIdHistoryResponse,
  GetApiV1UrlTrackerRecommendationAnalysisByRecommendationIdReferrersResponse,
  getApiV1UrlTrackerRecommendationAnalysisByRecommendationIdHistory,
  getApiV1UrlTrackerRecommendationAnalysisByRecommendationIdReferrers,
} from '@/api';

export const ContentElementTag = 'urltracker-sidebar-analyse-recommendation';

@customElement(ContentElementTag)
export default class UrlTrackerSidebarAnalyseRecommendation extends UmbModalBaseElement<
  IAnalyseRecommendationModel,
  void
> {
  private recommendationTypeStrategy = recommendationTypeStrategyResolver;

  @state()
  private _subText = '';

  @state()
  private referrers: GetApiV1UrlTrackerRecommendationAnalysisByRecommendationIdReferrersResponse | null = null;

  @state()
  private history: GetApiV1UrlTrackerRecommendationAnalysisByRecommendationIdHistoryResponse | null = null;

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

    return html` <h3 class="${ifDefined(errorClass)}">${this.recommendationTypeText}</h3> `;
  }

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    ensureExists(this.data, 'Expected contextual data, but none was found');

    this._subText = this.data.recommendation.url ?? '';

    const referrersPromise = getApiV1UrlTrackerRecommendationAnalysisByRecommendationIdReferrers({
      recommendationId: this.data.recommendation.id,
    });
    const historyPromise = getApiV1UrlTrackerRecommendationAnalysisByRecommendationIdHistory({
      recommendationId: this.data.recommendation.id,
    });

    const [referrers, history] = await Promise.all([referrersPromise, historyPromise]).catch((error) => {
      throw new Error(
        `Failed to fetch referrers and history for recommendation ${this.data?.recommendation.id}: ${error}`,
      );
    });

    this.referrers = referrers;
    this.history = history;

    const sourceStrategy = recommendationTypeStrategyResolver.getStrategy({
      recommendation: this.data?.recommendation,
      element: this,
    });
    if (sourceStrategy) {
      this.recommendationTypeText = await sourceStrategy.getTitle();
      this.recommendationTypeDescription = await sourceStrategy.getDescription();
      this.recommendationTypeIsError = false;
    } else {
      this.recommendationTypeText = this.localize.term('urlTrackerRecommendationType_unknown');
      this.recommendationTypeIsError = true;
    }
  }

  close() {
    this.modalContext?.submit();
  }

  protected renderHistoryChart() {
    if (!this.history?.dailyOccurances?.length) return html`<i>No data available</i>`;
    return html` <urltracker-history-chart .history=${this.history}></urltracker-history-chart> `;
  }

  protected renderReferrersChart() {
    if (!this.referrers?.length) return html`<i>No data available</i>`;
    return html` <urltracker-referrers-chart .referrers=${this.referrers}></urltracker-referrers-chart> `;
  }

  protected render() {
    return html`
      <div class="header">
        <h2>${this.renderRecommendationType()}</h2>
        <span>${this._subText}</span>
      </div>
      <div class="main">
        <uui-box>
          <p>${this.recommendationTypeDescription}</p>
          <h6>History (last 20 days)</h6>
          ${this.renderHistoryChart()}
          <h6>Most common referrers</h6>
          ${this.renderReferrersChart()}
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>Close</uui-button>
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
        color: #68676b;
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

      ul,
      p {
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
    `,
  ];
}
