import {
  getUmbracoManagementApiV1UrlTrackerLandingPageMetric,
  getUmbracoManagementApiV1UrlTrackerRecommendations,
  postUmbracoManagementApiV1UrlTrackerRecommendationsByRecommendationId,
} from '@/../../api-client';
import type { Client } from '@/../../api-client/client/types.gen';
import { RECOMMENDATION_SORT_TYPE } from '@/enums/sortType';
import variableresourceService from '@/util/tools/variableresource.service';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { LitElement, PropertyValueMap, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { repeat } from 'lit/directives/repeat.js';
import type {
  RecommendationCollectionResponse,
  RecommendationResponse,
  RedirectRequest,
} from '../../../../api-client/types.gen';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';
import { URL_TRACKER_ANALYSE_RECOMMENDATION_MODAL } from '../sidebars/analyseRecommendation-modal.token';
import { URL_TRACKER_EXPLAIN_RECOMMENDATION_MODAL } from '../sidebars/explainRecommendation-modal.token';
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from '../sidebars/explainRecommendations/explainRecommendations.lit';
import { URL_TRACKER_SIMPLE_REDIRECT_MODAL } from '../sidebars/simpleRedirect-modal.token';
import './redirects/redirectitem.lit';
import './redirects/redirectitemSkeleton.lit';
import { ISourceStrategies } from './redirects/source/source.constants';
import { ITargetStrategies } from './redirects/target/target.constants';

@customElement('urltracker-landing-tab')
export class UrlTrackerLandingTab extends UrlTrackerNotificationWrapper(UmbElementMixin(LitElement), 'landingpage') {
  @state()
  private recommendationCollection?: RecommendationCollectionResponse;

  @state()
  private numericMetric: number = 0;

  @state()
  private loading: number = 0;

  @state()
  private statisticLabel?: string;

  @state()
  private topRecommendationsLabel?: string;

  @state()
  private redirectLabel?: string;

  @state()
  private ignoreRecommendationLoadingIds: Array<number> = [];

  #notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;

  constructor() {
    super();
    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });
  }

  protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    this.statisticLabel = this.localize.term('urlTrackerDashboardLanding_statisticLabel');
    this.redirectLabel = this.localize.term('urlTrackerDashboardLanding_recommendationRedirect');

    await this.search();
  }

  private async search() {
    this.loading++;
    try {
      const recommendationTypeStrategies: {
        [key: string]: string;
      } = variableresourceService.get('recommendationTypeStrategies');

      const strategiesToInclude = Object.keys(recommendationTypeStrategies).filter((key) => {
        return key !== 'technicalFile' && key !== 'image';
      });

      const strategiesGuidArray = strategiesToInclude.map((key) => {
        return recommendationTypeStrategies[key];
      });

      const { data } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerRecommendations({
          client: umbHttpClient as unknown as Client,
          query: {
            Page: 1,
            PageSize: 10,
            OrderBy: RECOMMENDATION_SORT_TYPE.IMPORTANCE,
            Types: strategiesGuidArray,
          },
        }),
      );

      this.recommendationCollection = data as unknown as RecommendationCollectionResponse;

      const { data: numericMetricData } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerLandingPageMetric({
          client: umbHttpClient as unknown as Client,
        }),
      );

      this.numericMetric = numericMetricData?.value ?? 0;

      if (this.recommendationCollection?.total && this.recommendationCollection.total > 0) {
        this.topRecommendationsLabel =
          (await this.localize.term('urlTrackerDashboardLanding_topAmount', [
            this.recommendationCollection.results?.length.toString() ?? '0',
          ])) ?? 'Top recommendations';
      } else {
        this.topRecommendationsLabel =
          (await this.localize.term('urlTrackerDashboardLanding_noRecommendations')) ?? 'No recommendations';
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
      advanced: false,
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
      advanced: false,
    };

    this.openNewRedirectPanel(redirect, event.detail.id);
  };

  private handleIgnore = async (event: CustomEvent<RecommendationResponse>) => {
    this.ignoreRecommendationLoadingIds.push(event.detail.id);
    this.requestUpdate();
    try {
      await tryExecute(
        this,
        postUmbracoManagementApiV1UrlTrackerRecommendationsByRecommendationId({
          client: umbHttpClient as unknown as Client,
          path: {
            recommendationId: event.detail.id,
          },
          body: {
            recommendationStrategy: event.detail.strategy,
            ignore: true,
          },
        }),
      );

      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_recommendation-ignored') || 'Recommendation ignored',
          message:
            this.localize.term('urlTrackerGeneral_recommendation-ignored-message') ||
            'The recommendation has been removed from the overview',
        },
      });

      await this.search();
    } finally {
      this.ignoreRecommendationLoadingIds = this.ignoreRecommendationLoadingIds.filter((id) => id !== event.detail.id);
      this.requestUpdate();
    }
  };

  private openNewRedirectPanel(data: RedirectRequest, solvedRecommendation?: number) {
    umbOpenModal(this, URL_TRACKER_SIMPLE_REDIRECT_MODAL, {
      data: {
        title: this.localize.term('urlTrackerGeneral_new-redirect') || 'New redirect',
        data: data,
        advanced: false,
        sourceEditable: false,
        solvedRecommendation: solvedRecommendation,
      },
    })
      .then(() => this.submitNewRedirectPanel())
      .catch(() => this.closePanel());
  }

  submitNewRedirectPanel = () => {
    this.search();
  };

  private openExplanationPanel(data: RecommendationResponse) {
    umbOpenModal(this, URL_TRACKER_EXPLAIN_RECOMMENDATION_MODAL, {
      data: {
        recommendation: data,
      },
    })
      .then((value) => {
        this.submitExplanationPanel(data, value.type);
      })
      .catch(() => this.closePanel());
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

  private openAnalysePanel(data: RecommendationResponse) {
    umbOpenModal(this, URL_TRACKER_ANALYSE_RECOMMENDATION_MODAL, {
      data: {
        recommendation: data,
      },
    })
      .then(() => {
        this.closePanel();
      })
      .catch(() => this.closePanel());
  }

  closePanel = () => {};

  private onExplain = (e: CustomEvent<RecommendationResponse>) => {
    this.openExplanationPanel(e.detail);
  };

  private onAnalyse = (e: CustomEvent<RecommendationResponse>) => {
    this.openAnalysePanel(e.detail);
  };

  private renderRecommendations(): unknown {
    if (this.loading) {
      return html`<urltracker-recommendation-item-skeleton></urltracker-recommendation-item-skeleton>
        <urltracker-recommendation-item-skeleton></urltracker-recommendation-item-skeleton>
        <urltracker-recommendation-item-skeleton></urltracker-recommendation-item-skeleton> `;
    }

    if (!this.recommendationCollection?.results) return nothing;

    return repeat(
      this.recommendationCollection.results,
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

  private renderRedirectButton(): unknown {
    if (!this.recommendationCollection?.results) return nothing;
    return html` <div class="recommendation-redirect">
      <uui-button look="primary" @click=${this.openRecommendations}
        >${this.redirectLabel}
        <uui-icon name="icon-navigation-right" class="icon-white"></uui-icon>
      </uui-button>
    </div>`;
  }

  private openRecommendations() {
    const event = new CustomEvent('url-tracker-open-tab', {
      detail: {
        alias: 'recommendations',
      },
    });

    window.dispatchEvent(event);
  }

  protected renderInternal(): unknown {
    return html`
      <div class="grid-root">
        <div class="results">
          <urltracker-result-list .header=${this.topRecommendationsLabel}>
            ${this.renderRecommendations()}
          </urltracker-result-list>
          ${this.renderRedirectButton()}
        </div>

        <uui-box>
          <div class="total">
            <span>${this.numericMetric}</span>
            <p>${this.statisticLabel}</p>
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

    .recommendation-redirect {
      display: flex;
      justify-content: flex-end;
    }

    .icon-white {
      fill: white;
    }

    urltracker-recommendation-item {
      min-width: 0;
    }

    urltracker-result-list {
      flex: 1 1 0;
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
