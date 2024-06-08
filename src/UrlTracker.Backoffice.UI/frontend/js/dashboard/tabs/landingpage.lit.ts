import { ensureServiceExists } from '@/util/tools/existancecheck';
import variableresourceService from '@/util/tools/variableresource.service';
import { LitElement, PropertyValueMap, css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { customElement, state } from 'lit/decorators.js';
import { repeat } from 'lit/directives/repeat.js';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';
import './redirects/redirectitem.lit';
import '@/util/elements/resultlist.lit';
import { ISourceStrategies } from './redirects/source/source.constants';
import { ITargetStrategies } from './redirects/target/target.constants';
import { createNewRedirectOptions } from '../sidebars/simpleRedirect/manageredirect';
import {
  RecommendationOrderBy,
  RecommendationResponse,
  RedirectRequest,
  getApiV1UrlTrackerLandingPageMetric,
  getApiV1UrlTrackerRecommendations,
  postApiV1UrlTrackerRecommendationsByRecommendationId,
} from '@/api';
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from '@umbraco-cms/backoffice/modal';
import { URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL } from '../sidebars/explainRecommendations/manifest';
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from '../sidebars/explainRecommendations/explainrecommendations';
import { URLTRACKER_ANALYSE_RECOMMENDATION_MODAL } from '../sidebars/analyseRecommendation/manifest';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationContext } from '@umbraco-cms/backoffice/notification';
import { URLTRACKER_EDIT_REDIRECT_MODAL } from '../sidebars/simpleRedirect/manifest';
import { ProcessedRecommendationResponse } from '@/context/recommendationitem.context';
import { URLTRACKER_SCORING_CONTEXT } from '@/services/scoring/contexttoken';
import ScoringService from '@/services/scoring/scoring.service';

@customElement('urltracker-landing-tab')
export default class UrlTrackerLandingTab extends UrlTrackerNotificationWrapper(LitElement, 'landingpage') {
  private _modalManager?: UmbModalManagerContext | undefined;
  public get modalManager(): UmbModalManagerContext {
    ensureServiceExists(this._modalManager, 'modalManager');
    return this._modalManager;
  }

  private _notificationContext: UmbNotificationContext | undefined;
  public get notificationContext(): UmbNotificationContext {
    ensureServiceExists(this._notificationContext, 'notificationContext');
    return this._notificationContext;
  }

  private _scoringService: ScoringService | undefined;
  public get scoringService(): ScoringService {
    ensureServiceExists(this._scoringService, 'scoringService');
    return this._scoringService;
  }

  constructor() {
    super();

    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance?: UmbModalManagerContext) => {
      this._modalManager = instance;
    });

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance?: UmbNotificationContext) => {
      this._notificationContext = instance;
    });

    this.consumeContext(URLTRACKER_SCORING_CONTEXT, (instance?: ScoringService) => {
      this._scoringService = instance;
    });
  }

  @state()
  private recommendationCollection?: ProcessedRecommendationResponse[];

  @state()
  private numericMetric: number = 0;

  @state()
  private loading: number = 0;

  protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    await this.search();
  }

  private async search() {
    this.loading++;
    try {
      const { data } = await tryExecuteAndNotify(
        this,
        getApiV1UrlTrackerRecommendations({
          page: 1,
          pageSize: 10,
          orderBy: RecommendationOrderBy.IMPORTANCE,
        }),
      );
      if (data) {
        this.recommendationCollection = await Promise.all(
          data.results.map((r) => this.scoringService.processRecommendation(r)),
        );
      }

      const { data: metricData } = await tryExecuteAndNotify(this, getApiV1UrlTrackerLandingPageMetric());
      if (metricData) {
        this.numericMetric = metricData.value;
      }
    } finally {
      this.loading--;
    }
  }

  private handleCreatePermanentRedirect = async (event: CustomEvent<RecommendationResponse>) => {
    const redirect = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').url,
        value: event.detail.url,
      },
      target: {
        strategy: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
        value: '',
      },
      permanent: true,
      retainQuery: true,
      force: false,
    };

    this.openNewRedirectPanel(redirect, event.detail.id);
  };

  private handleCreateTemporaryRedirect = async (event: CustomEvent<RecommendationResponse>) => {
    const redirect = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').url,
        value: event.detail.url,
      },
      target: {
        strategy: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
        value: '',
      },
      permanent: false,
      retainQuery: true,
      force: false,
    };

    this.openNewRedirectPanel(redirect, event.detail.id);
  };

  private handleIgnore = async (event: CustomEvent<RecommendationResponse>) => {
    const { error } = await tryExecuteAndNotify(
      this,
      postApiV1UrlTrackerRecommendationsByRecommendationId({
        recommendationId: event.detail.id,
        requestBody: {
          recommendationStrategy: event.detail.strategy,
          ignore: true,
        },
      }),
    );

    if (!error) {
      this.notificationContext.peek('positive', {
        data: {
          headline: 'Recommendation ignored',
          message: 'The recommendation has been removed from the overview',
        },
      });
    }

    await this.search();
  };

  private async openNewRedirectPanel(data: RedirectRequest, solvedRecommendation?: number) {
    const options = createNewRedirectOptions({
      title: 'New redirect',
      data: data,
      advanced: false,
      solvedRecommendation: solvedRecommendation,
    });

    const modal = this.modalManager.open(this, URLTRACKER_EDIT_REDIRECT_MODAL, {
      data: options,
    });

    try {
      await modal.onSubmit();
      await this.search();
    } catch {
      /* Nothing to do when the modal is rejected */
    }
  }

  private async openExplanationPanel(data: RecommendationResponse) {
    const modal = this.modalManager.open(this, URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL, {
      data: {
        recommendation: data,
      },
    });

    try {
      const action = await modal.onSubmit();
      this.submitExplanationPanel(data, action);
    } catch {}
  }

  private submitExplanationPanel = (recommendation: RecommendationResponse, action: IRecommendationAction) => {
    switch (action) {
      case RECCOMENDATION_ACTIONS.MAKE_PERMANENT:
        this.handleCreatePermanentRedirect(new CustomEvent('', { detail: recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.MAKE_TEMPORARY:
        this.handleCreateTemporaryRedirect(new CustomEvent('', { detail: recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.IGNORE:
        this.handleIgnore(new CustomEvent('', { detail: recommendation }));
        break;
    }
  };

  private async openAnalysePanel(data: RecommendationResponse) {
    const modal = this.modalManager.open(this, URLTRACKER_ANALYSE_RECOMMENDATION_MODAL, {
      data: {
        recommendation: data,
      },
    });

    try {
      await modal.onSubmit();
    } catch {
      /* We should never come here, because the analysis view never rejects */
    }
  }

  private onExplain = (e: CustomEvent<RecommendationResponse>) => {
    this.openExplanationPanel(e.detail);
  };

  private onAnalyse = (e: CustomEvent<RecommendationResponse>) => {
    this.openAnalysePanel(e.detail);
  };

  private renderRecommendations(): unknown {
    if (!this.recommendationCollection) return nothing;
    return repeat(
      this.recommendationCollection,
      (recommendation) => recommendation.id,
      (r) =>
        html`<urltracker-recommendation-item
          .item=${r}
          .selectable=${false}
          @explain=${this.onExplain}
          @analyse=${this.onAnalyse}
          @createPermanent=${this.handleCreatePermanentRedirect}
          @createTemporary=${this.handleCreateTemporaryRedirect}
          @ignore=${this.handleIgnore}
        ></urltracker-recommendation-item>`,
    );
  }

  protected renderInternal(): unknown {
    return html`
      <div class="grid-root">
        <div class="results">
          <urltracker-result-list .loading=${!!this.loading} header="Top 10 recommendations">
            ${this.renderRecommendations()}
          </urltracker-result-list>
        </div>

        <uui-box>
          <div class="total">
            <span>${this.numericMetric}</span>
            <p>Pages were not found last week</p>
          </div>
        </uui-box>
      </div>
    `;
  }

  static styles = css`
    .grid-root {
      display: grid;
      grid-template-columns: 1fr 360px;
      gap: 1rem;
    }

    .results {
      grid-column: 1;
      display: flex;
      flex-direction: column;
      gap: 1rem;
      min-width: 0;
    }

    urltracker-result-list {
      flex: 1 1 32rem;
    }

    uui-box {
      height: fit-content;
    }

    uui-box .total {
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    uui-box span {
      color: var(--uui-palette-space-cadet);
      font-family: Lato, sans-serif;
      font-size: 40px;
      font-weight: 800;
      line-height: 45px;
    }

    uui-box p {
      font-family: Lato, sans-serif;
      font-size: 15px;
      font-weight: 400;
      line-height: 20px;
      margin: 0;
    }
  `;
}
