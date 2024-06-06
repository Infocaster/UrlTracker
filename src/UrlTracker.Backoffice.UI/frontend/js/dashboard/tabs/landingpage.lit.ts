import { recommendationServiceContext } from '@/context/recommendationservice.context';
import { RECOMMENDATION_SORT_TYPE } from '@/enums/sortType';
import {
  IRecommendationCollection,
  IRecommendationResponse,
  IRecommendationsService,
} from '@/services/recommendation.service';
import { ensureServiceExists } from '@/util/tools/existancecheck';
import variableresourceService from '@/util/tools/variableresource.service';
import { consume } from '@lit/context';
import { LitElement, PropertyValueMap, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { repeat } from 'lit/directives/repeat.js';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';
import './redirects/redirectitem.lit';
import { ISourceStrategies } from './redirects/source/source.constants';
import { ITargetStrategies } from './redirects/target/target.constants';
import { createNewRedirectOptions } from '../sidebars/simpleRedirect/manageredirect';
import { getApiV1UrlTrackerLandingPageMetric } from '@/api';
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from '@umbraco-cms/backoffice/modal';
import { URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL } from '../sidebars/explainRecommendations/manifest';
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from '../sidebars/explainRecommendations/explainrecommendations';
import { URLTRACKER_ANALYSE_RECOMMENDATION_MODAL } from '../sidebars/analyseRecommendation/manifest';

@customElement('urltracker-landing-tab')
export class UrlTrackerLandingTab extends UrlTrackerNotificationWrapper(LitElement, 'landingpage') {
  private _modalManager?: UmbModalManagerContext | undefined;
  public get modalManager(): UmbModalManagerContext {
    ensureServiceExists(this._modalManager, 'modalManager');
    return this._modalManager;
  }

  constructor() {
    super();

    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance?: UmbModalManagerContext) => {
      this._modalManager = instance;
    });
  }

  @consume({ context: recommendationServiceContext })
  private _recommendationsService?: IRecommendationsService;

  @state()
  private recommendationCollection?: IRecommendationCollection;

  @state()
  private numericMetric: number = 0;

  @state()
  private loading: number = 0;

  protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    ensureServiceExists(this._recommendationsService, 'recommendations service');

    await this.search();
  }

  private async search() {
    this.loading++;
    try {
      this.recommendationCollection = await this._recommendationsService!.list({
        page: 1,
        pageSize: 10,
        OrderBy: RECOMMENDATION_SORT_TYPE.IMPORTANCE,
      });
      this.numericMetric = (await getApiV1UrlTrackerLandingPageMetric()).value;
    } finally {
      this.loading--;
    }
  }

  private handleCreatePermanentRedirect = async (event: CustomEvent<IRecommendationResponse>) => {
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

  private handleCreateTemporaryRedirect = async (event: CustomEvent<IRecommendationResponse>) => {
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

  private handleIgnore = async (event: CustomEvent<IRecommendationResponse>) => {
    await this._recommendationsService!.update(event.detail.id, {
      recommendationStrategy: event.detail.strategy,
      ignore: true,
    });
    this.notificationsService.success(
      'Recommendation ignored',
      'The recommendation has been removed from the overview',
    );

    await this.search();
  };

  private openNewRedirectPanel(data: IRedirectData, solvedRecommendation?: number) {
    const options = createNewRedirectOptions({
      title: 'New redirect',
      submit: this.submitNewRedirectPanel,
      close: this.closePanel,
      data: data,
      advanced: false,
      solvedRecommendation: solvedRecommendation,
    });

    this.editorService!.open(options);
  }

  submitNewRedirectPanel = (_: IRedirectResponse) => {
    this.closePanel();
    this.search();
  };

  private async openExplanationPanel(data: IRecommendationResponse) {
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

  private submitExplanationPanel = (recommendation: IRecommendationResponse, action: IRecommendationAction) => {
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

  private async openAnalysePanel(data: IRecommendationResponse) {
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

  private onExplain = (e: CustomEvent<IRecommendationResponse>) => {
    this.openExplanationPanel(e.detail);
  };

  private onAnalyse = (e: CustomEvent<IRecommendationResponse>) => {
    this.openAnalysePanel(e.detail);
  };

  private renderRecommendations(): unknown {
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
