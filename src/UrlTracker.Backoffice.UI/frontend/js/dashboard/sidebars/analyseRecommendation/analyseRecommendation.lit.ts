import { ILocalizationService, localizationServiceContext } from '@/context/localizationservice.context';
import {
  IRecommendationsAnalysisService,
  recommendationsAnalysisServiceContext,
} from '@/context/recommendationsanalysis.context';
import { scopeContext } from '@/context/scope.context';
import { IRecommendationResponse } from '@/services/recommendation.service';
import {
  IRecommendationHistoryResponse,
  IRecommendationReferrerResponse,
} from '@/services/recommendationanalysis.service';
import { ensureExists, ensureServiceExists } from '@/util/tools/existancecheck';
import { consume } from '@lit/context';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import recommendationTypeStrategyResolver from '../../tabs/recommendations/recommendationType/recommendation.strategy';

import { cardWithClickableHeader } from '@/dashboard/tabs/styles';
import { Task } from '@lit/task';
import { ifDefined } from 'lit/directives/if-defined.js';
import './historyChart.lit';
import './referrersChart.lit';
import { AnalyseRecommendationScope } from './scope';

export const ContentElementTag = 'urltracker-sidebar-analyse-recommendation';

type Translations = {
  close: string;
  lastTwentyDays: string;
  referrers: string;
  noData: string;
};

@customElement(ContentElementTag)
export class UrlTrackerSidebarAnalyseRecommendation extends LitElement {
  private recommendationTypeStrategy = recommendationTypeStrategyResolver;

  @consume({ context: recommendationsAnalysisServiceContext })
  private recommendationsAnalysisService?: IRecommendationsAnalysisService;

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @consume({ context: scopeContext })
  private $scope?: AnalyseRecommendationScope;

  @property({ attribute: false })
  get scope() {
    ensureExists(this.$scope, 'scope');
    return this.$scope;
  }

  @state()
  private data!: IRecommendationResponse;

  @state()
  private _subText = '';

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

  @state()
  private translationTaskKey: number = 0;

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

    ensureServiceExists(this.recommendationsAnalysisService, 'recommendationsAnalysisService');
    ensureServiceExists(this.localizationService, 'localizationService');

    this.data = this.scope.model.recommendation;
    this._subText = this.scope.model.recommendation.url ?? '';

    const referrersPromise = this.recommendationsAnalysisService.getReferrers(this.data.id);
    const historyPromise = this.recommendationsAnalysisService.getHistory(this.data.id, {});

    const [referrers, history] = await Promise.all([referrersPromise, historyPromise]).catch((error) => {
      throw new Error(`Failed to fetch referrers and history for recommendation ${this.data.id}: ${error}`);
    });

    this.referrers = referrers;
    this.history = history;

    const sourceStrategy = recommendationTypeStrategyResolver.getStrategy({ recommendation: this.data, element: this });
    if (sourceStrategy) {
      this.recommendationTypeText = await sourceStrategy.getTitle();
      this.recommendationTypeDescription = await sourceStrategy.getDescription();
      this.recommendationTypeIsError = false;
    } else {
      this.recommendationTypeText = await this.localizationService.localize('urlTrackerRecommendationType_unknown');
      this.recommendationTypeIsError = true;
    }
  }

  close() {
    this.scope.model.close();
  }

  protected renderHistoryChart(noDataText: string) {
    if (!this.history?.dailyOccurances?.length) return html`<i>${noDataText}</i>`;
    return html` <urltracker-history-chart .history=${this.history}></urltracker-history-chart> `;
  }

  protected renderReferrersChart(noDataText: string) {
    if (!this.referrers?.length) return html`<i>${noDataText}</i>`;
    return html` <urltracker-referrers-chart .referrers=${this.referrers}></urltracker-referrers-chart> `;
  }

  private _translationTask = new Task(this, {
    task: async (): Promise<Partial<Translations>> => {
      const [close, lastTwentyDays, referrers, noData] = await Promise.all([
        this.localizationService?.localize('urlTrackerGeneral_close'),
        this.localizationService?.localize('urlTrackerAnalyseRecommendation_history'),
        this.localizationService?.localize('urlTrackerAnalyseRecommendation_referrers'),
        this.localizationService?.localize('urlTrackerGeneral_no-data'),
      ]);
      return {
        close,
        lastTwentyDays,
        referrers,
        noData,
      };
    },
    args: () => [this.translationTaskKey],
  });

  protected render() {
    return this._translationTask.render({
      complete: (translations: Partial<Translations>) => {
        return html`
          <div class="header">
            <h2>${this.renderRecommendationType()}</h2>
            <span>${this._subText}</span>
          </div>
          <div class="main">
            <uui-box>
              <p>${this.recommendationTypeDescription}</p>
              <h6>${translations.lastTwentyDays}</h6>
              ${this.renderHistoryChart(translations.noData ?? 'No Data')}
              <h6>${translations.referrers}</h6>
              ${this.renderReferrersChart(translations.noData ?? 'No Data')}
            </uui-box>
          </div>
          <div class="footer">
            <uui-button look="default" color="default" @click=${this.close}>${translations.close}</uui-button>
          </div>
        `;
      },
    });
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
