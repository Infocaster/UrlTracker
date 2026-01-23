import { css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import recommendationTypeStrategyResolver from '../../tabs/recommendations/recommendationType/recommendation.strategy';

import { cardWithClickableHeader } from '@/dashboard/tabs/styles';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalContext, UmbModalExtensionElement } from '@umbraco-cms/backoffice/modal';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { ifDefined } from 'lit/directives/if-defined.js';
import type { Client } from '../../../../../api-client/client/types.gen';
import {
  getUmbracoManagementApiV1UrlTrackerRecommendationAnalysisByRecommendationIdHistory,
  getUmbracoManagementApiV1UrlTrackerRecommendationAnalysisByRecommendationIdReferrers,
} from '../../../../../api-client/sdk.gen';
import { RecommendationHistory, RecommendationResponse, ReferrerResponse } from '../../../../../api-client/types.gen';
import {
  UrlTrackerAnalyseRecommendationModalData,
  UrlTrackerAnalyseRecommendationModalValue,
} from '../analyseRecommendation-modal.token';
import './historyChart.lit';
import './referrersChart.lit';

export const ContentElementTag = 'urltracker-sidebar-analyse-recommendation';

@customElement(ContentElementTag)
export class UrlTrackerSidebarAnalyseRecommendation
  extends UmbLitElement
  implements
    UmbModalExtensionElement<UrlTrackerAnalyseRecommendationModalData, UrlTrackerAnalyseRecommendationModalValue>
{
  private recommendationTypeStrategy = recommendationTypeStrategyResolver;

  @property({ attribute: false })
  modalContext?: UmbModalContext<UrlTrackerAnalyseRecommendationModalData, UrlTrackerAnalyseRecommendationModalValue>;

  @property({ attribute: false })
  data?: UrlTrackerAnalyseRecommendationModalData;

  @state()
  private _subText = '';

  @state()
  private referrers: ReferrerResponse[] | undefined = undefined;

  @state()
  private history: RecommendationHistory | undefined = undefined;

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

    const referrersPromise = tryExecute(
      this,
      getUmbracoManagementApiV1UrlTrackerRecommendationAnalysisByRecommendationIdReferrers({
        client: umbHttpClient as unknown as Client,
        path: {
          recommendationId: this.data?.recommendation?.id!,
        },
      }),
    );

    const historyPromise = tryExecute(
      this,
      getUmbracoManagementApiV1UrlTrackerRecommendationAnalysisByRecommendationIdHistory({
        client: umbHttpClient as unknown as Client,
        path: {
          recommendationId: this.data?.recommendation?.id!,
        },
      }),
    );

    //

    const [referrers, history] = await Promise.all([referrersPromise, historyPromise]).catch((error) => {
      throw new Error(
        `Failed to fetch referrers and history for recommendation ${this.data?.recommendation?.id}: ${error}`,
      );
    });

    this.referrers = referrers.data;
    this.history = history.data;

    const sourceStrategy = recommendationTypeStrategyResolver.getStrategy({
      recommendation: this.data?.recommendation! as unknown as RecommendationResponse,
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
    this.modalContext?.reject();
  }

  protected renderHistoryChart() {
    if (!this.history?.dailyOccurances?.length)
      return html`<i><umb-localize key="urlTrackerGeneral_no-data">No data</umb-localize></i>`;
    return html` <urltracker-history-chart .history=${this.history}></urltracker-history-chart> `;
  }

  protected renderReferrersChart() {
    if (!this.referrers?.length)
      return html`<i><umb-localize key="urlTrackerGeneral_no-data">No data</umb-localize></i>`;
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
          <h6>
            <umb-localize key="urlTrackerGeneral_history"> Last 20 days </umb-localize>
          </h6>

          ${this.renderHistoryChart()}
          <h6>
            <umb-localize key="urlTrackerAnalyseRecommendation_referrers">Referrers</umb-localize>
          </h6>
          ${this.renderReferrersChart()}
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>
          <umb-localize key="urlTrackerGeneral_close">Close</umb-localize>
        </uui-button>
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

export const element = UrlTrackerSidebarAnalyseRecommendation;
