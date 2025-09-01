import { Task } from '@lit/task';
import { LitElement, PropertyValueMap, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import recommendationService, {
  IRecommendationCollection,
  IRecommendationResponse,
  IRecommendationUpdateBulkRequest,
  IRecommendationsService,
} from '../../services/recommendation.service';
import { UrlTrackerPagination } from '../../util/elements/inputs/pagination.lit';
import { ensureExists, ensureServiceExists } from '../../util/tools/existancecheck';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';

import { IEditorService, editorServiceContext } from '@/context/editorservice.context';
import {
  IUmbracoNotificationsService,
  umbracoNotificationsServiceContext,
} from '@/context/notificationsservice.context';

import { redirectServiceContext } from '@/context/redirectservice.context';
import { IRedirectData, IRedirectResponse, IRedirectService } from '@/services/redirect.service';
import { LoadingStatus } from '@/types/loadingStatus';
import variableresourceService from '@/util/tools/variableresource.service';
import { consume, provide } from '@lit/context';
import { ifDefined } from 'lit/directives/if-defined.js';
import { repeat } from 'lit/directives/repeat.js';
import { IChangeManager, changeManagerContext } from '../../context/changemanager.context';
import { recommendationServiceContext } from '../../context/recommendationservice.context';
import { RECOMMENDATION_SORT_TYPE, RecommendationSortType } from '../../enums/sortType';
import { DropdownChangeEvent, IDropdownValue } from '../../util/elements/inputs/dropdown.lit';
import { createAnalyseRecommendationEditor } from '../sidebars/analyseRecommendation/analyserecommendation';
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from '../sidebars/explainRecommendations/explainRecommendations.lit';
import { createExplainRecommendationsEditor } from '../sidebars/explainRecommendations/explainrecommendations';
import { createNewRedirectOptions } from '../sidebars/simpleRedirect/manageredirect';
import './recommendations/recommendationSearch.lit';
import './recommendations/recommendationitem.lit';
import './recommendations/recommendationitemSkeleton.lit';
import { ISourceStrategies } from './redirects/source/source.constants';
import { ITargetStrategies } from './redirects/target/target.constants';

type Translations = {
  importance: string;
  mostRecentlyUpdated: string;
  url: string;
  newRedirect: string;
  recommendationIgnored: string;
  recommendationIgnoredMessage: string;
  editRedirect: string;
  ignore: string;
  orderBy: string;
  results: string;
  recommendationsIgnored: string;
  recommendationsIgnoredMessage: string;
};

@customElement('urltracker-recommendations-tab')
export class UrlTrackerRecommendationsTab extends UrlTrackerNotificationWrapper(LitElement, 'recommendations') {
  @consume({ context: recommendationServiceContext })
  private _recommendationsService?: IRecommendationsService;

  @consume({ context: redirectServiceContext })
  private _redirectService?: IRedirectService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: umbracoNotificationsServiceContext })
  private _notificationsService?: IUmbracoNotificationsService | undefined;
  public get notificationsService(): IUmbracoNotificationsService {
    console.log('notification service');
    ensureServiceExists(this._notificationsService, 'notificationsService');
    return this._notificationsService;
  }
  public set notificationsService(value: IUmbracoNotificationsService | undefined) {
    this._notificationsService = value;
  }

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @state()
  private recommendationCollection?: IRecommendationCollection;

  @state()
  private loading: number = 0;

  @state()
  private selectedItems: number[] = [];

  @state()
  private ignoreRecommendationLoadingIds: Array<number> = [];

  @state()
  private ignoreSelectionState: LoadingStatus = undefined;

  @state()
  private translationTaskKey: number = 0;

  @state()
  private translations: Partial<Translations> = {};

  private query = '';
  private selectedType: RecommendationSortType = RECOMMENDATION_SORT_TYPE.IMPORTANCE;
  private paginationRef: Ref<UrlTrackerPagination> = createRef();

  private _translationTask = new Task(this, {
    task: async (): Promise<Partial<Translations>> => {
      const [
        importance,
        mostRecentlyUpdated,
        url,
        newRedirect,
        recommendationIgnored,
        recommendationIgnoredMessage,
        editRedirect,
        ignore,
        orderBy,
        results,
        recommendationsIgnored,
        recommendationsIgnoredMessage,
      ] = await Promise.all([
        this.localizationService?.localize('urlTrackerGeneral_importance'),
        this.localizationService?.localize('urlTrackerGeneral_most-recently-updated'),
        this.localizationService?.localize('urlTrackerGeneral_url'),
        this.localizationService?.localize('urlTrackerGeneral_new-redirect'),
        this.localizationService?.localize('urlTrackerGeneral_recommendation-ignored'),
        this.localizationService?.localize('urlTrackerGeneral_recommendation-ignored-message'),
        this.localizationService?.localize('urlTrackerGeneral_edit-redirect'),
        this.localizationService?.localize('urlTrackerGeneral_ignore'),
        this.localizationService?.localize('urlTrackerGeneral_order-by'),
        this.localizationService?.localize('urlTrackerGeneral_results'),
        this.localizationService?.localize('urlTrackerGeneral_recommendations-ignored'),
        this.localizationService?.localize('urlTrackerGeneral_recommendations-ignored-message'),
      ]);

      const translations: Partial<Translations> = {
        importance,
        mostRecentlyUpdated,
        url,
        newRedirect,
        recommendationIgnored,
        recommendationIgnoredMessage,
        editRedirect,
        ignore,
        orderBy,
        results,
        recommendationsIgnored,
        recommendationsIgnoredMessage,
      };

      this.translations = translations;

      return translations;
    },
    args: () => [this.translationTaskKey],
  });

  private get _sortOptions(): IDropdownValue[] {
    return [
      {
        display: this.translations.importance || 'Importance',
        value: RECOMMENDATION_SORT_TYPE.IMPORTANCE,
        key: RECOMMENDATION_SORT_TYPE.IMPORTANCE.toString(),
      },
      {
        display: this.translations.mostRecentlyUpdated || 'Most recently updated',
        value: RECOMMENDATION_SORT_TYPE.MOST_RECENTLY_UPDATED,
        key: RECOMMENDATION_SORT_TYPE.MOST_RECENTLY_UPDATED.toString(),
      },
      {
        display: this.translations.url || 'Url',
        value: RECOMMENDATION_SORT_TYPE.URL,
        key: RECOMMENDATION_SORT_TYPE.URL.toString(),
      },
    ];
  }

  private _sortDirectionMap: Record<RecommendationSortType, boolean> = {
    [RECOMMENDATION_SORT_TYPE.IMPORTANCE]: true,
    [RECOMMENDATION_SORT_TYPE.MOST_RECENTLY_UPDATED]: true,
    [RECOMMENDATION_SORT_TYPE.URL]: false,
  };

  protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    console.log('init');
    ensureServiceExists(this._recommendationsService, 'recommendations service');
    ensureServiceExists(this._redirectService, 'redirect service');
    ensureServiceExists(this._recommendationsService, 'recommendations service');
    ensureServiceExists(this.editorService, 'editor service');

    await this.search();
  }

  private async search() {
    this.recommendationCollection = undefined;
    console.log('search');
    ensureExists(this.paginationRef.value);

    const page = {
      page: this.paginationRef.value!.value.page + 1,
      pageSize: this.paginationRef.value!.value.pageSize,
    };
    const type = this.selectedType;
    const query = this.query;

    this.loading++;
    try {
      this.recommendationCollection = await this._recommendationsService?.list({
        ...page,
        query,
        OrderBy: type,
        Desc: this._sortDirectionMap[type],
      });
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
      advanced: false,
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
      advanced: false,
    };

    this.openNewRedirectPanel(redirect, event.detail.id);
  };

  private handleIgnore = async (event: CustomEvent<IRecommendationResponse>) => {
    this.ignoreRecommendationLoadingIds.push(event.detail.id);
    this.requestUpdate();
    try {
      await this._recommendationsService!.update(event.detail.id, {
        recommendationStrategy: event.detail.strategy,
        ignore: true,
      });

      this.notificationsService.success(
        this.translations.recommendationIgnored || 'Recommendation ignored',
        this.translations.recommendationIgnoredMessage || 'The recommendation has been removed from the overview',
      );

      await this.search();
    } finally {
      this.ignoreRecommendationLoadingIds = this.ignoreRecommendationLoadingIds.filter((id) => id !== event.detail.id);
      this.requestUpdate();
    }
  };

  private openNewRedirectPanel(data: IRedirectData, solvedRecommendation?: number) {
    const options = createNewRedirectOptions({
      title: this.translations.newRedirect || 'New redirect',
      submit: this.submitNewRedirectPanel,
      close: this.closePanel,
      data: data,
      advanced: false,
      sourceEditable: false,
      solvedRecommendation: solvedRecommendation,
    });

    this.editorService!.open(options);
  }

  private submitNewRedirectPanel = (_: IRedirectResponse) => {
    this.closePanel();
    this.search();
  };

  private openExplanationPanel(data: IRecommendationResponse) {
    const options = createExplainRecommendationsEditor({
      recommendation: data,
      submit: (action) => this.submitExplanationPanel(data, action),
      close: this.closePanel,
    });

    this.editorService!.open(options);
  }

  private submitExplanationPanel = (recommendation: IRecommendationResponse, action: IRecommendationAction) => {
    this.editorService!.close();
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

  private openAnalysePanel(data: IRecommendationResponse) {
    const options = createAnalyseRecommendationEditor({
      close: this.closePanel,
      recommendation: data,
    });
    this.editorService!.open(options);
  }

  closePanel = () => {
    this.editorService!.close();
  };

  private onSearch = ({ detail: { query } = {} }: CustomEvent) => {
    this.query = query;
    this.search();
  };

  private onSortChange = ({ data }: DropdownChangeEvent) => {
    this.selectedType = data.value as RecommendationSortType;
    this.search();
  };

  private onExplain = (e: CustomEvent<IRecommendationResponse>) => {
    this.openExplanationPanel(e.detail);
  };

  private onAnalyse = (e: CustomEvent<IRecommendationResponse>) => {
    this.openAnalysePanel(e.detail);
  };

  private onFilterChange = (_: Event) => {
    this.search();
  };

  private onSelectItem = (e: any) => {
    this.selectedItems.push(e.item.id);
    this.requestUpdate();
  };

  private onDeselectItem = (e: any) => {
    this.selectedItems = this.selectedItems.filter((i) => i !== e.item.id);
  };

  private onSelectAll = (_: any) => {
    if (this.selectedItems.length === this.recommendationCollection?.total) {
      this.selectedItems = [];
    } else {
      this.selectedItems = this.recommendationCollection?.results.map((r) => r.id) || [];
    }
  };

  private onClearSelection = (_: any) => {
    this.selectedItems = [];
  };

  private onIgnoreSelection = async (_: any) => {
    this.ignoreSelectionState = 'waiting';
    try {
      const selectedRecommendations =
        this.recommendationCollection?.results.filter((r) => this.selectedItems.some((i) => i === r.id)) || [];
      const bulkToUpdate: IRecommendationUpdateBulkRequest = selectedRecommendations.map((r) => {
        return {
          id: r.id,
          data: {
            recommendationStrategy: r.strategy,
            ignore: true,
          },
        };
      });
      await recommendationService.updateBulk(bulkToUpdate);
      this.notificationsService.success(
        this.translations.recommendationsIgnored || 'Recommendations ignored',
        this.translations.recommendationsIgnoredMessage ||
          'All selected recommendations have been removed from the overview',
      );
      this.selectedItems = [];
      this.search();

      this.ignoreSelectionState = undefined;
    } catch {
      this.ignoreSelectionState = 'failed';
    }
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
          .isSelected=${this.selectedItems.some((i) => i === r.id)}
          .ignoreRecommendationLoading=${this.ignoreRecommendationLoadingIds.includes(r.id)}
          @selected=${this.onSelectItem}
          @deselected=${this.onDeselectItem}
          @explain=${this.onExplain}
          @analyse=${this.onAnalyse}
          @createPermanent=${this.handleCreatePermanentRedirect}
          @createTemporary=${this.handleCreateTemporaryRedirect}
          @ignore=${this.handleIgnore}
        ></urltracker-recommendation-item>`,
    );
  }

  private renderPagination(): unknown {
    return html` <urltracker-pagination
      ${ref(this.paginationRef)}
      class="pagination"
      total="${ifDefined(this.recommendationCollection?.total)}"
      @change=${this.onFilterChange}
    ></urltracker-pagination>`;
  }

  private renderBulkActions(): unknown {
    if (!this.selectedItems.length) return nothing;
    return html`
      <urltracker-bulk-actions
        class="bulk"
        .selectedCount=${this.selectedItems.length}
        .total=${this.recommendationCollection ? this.recommendationCollection.total : 0}
        @select-all=${this.onSelectAll}
        @clear-selection=${this.onClearSelection}
      >
        <uui-button look="secondary" .state="${this.ignoreSelectionState}" @click=${this.onIgnoreSelection}>
          <uui-icon name="delete"></uui-icon>
          ${this.translations.ignore}
        </uui-button>
      </urltracker-bulk-actions>
    `;
  }

  protected renderFilters(): unknown {
    if (this.selectedItems.length > 0) return nothing;
    return html`
      <div class="filters">
        <urltracker-recommendation-search @search=${this.onSearch}></urltracker-recommendation-search>
        <urltracker-dropdown
          label="${this.translations.orderBy || 'Order by'}"
          .options=${this._sortOptions}
          @change=${this.onSortChange}
        ></urltracker-dropdown>
      </div>
    `;
  }

  protected renderInternal(): unknown {
    return html`
      <div class="grid-root">
        ${this.renderFilters()} ${this.renderBulkActions()}
        <div class="results">
          ${this._translationTask.render({
            complete: () => html`
              <urltracker-result-list
                .loading=${!!this.loading}
                .header=${`${this.translations.results || 'Results'} (${this.recommendationCollection ? this.recommendationCollection.total : 0})`}
              >
                ${this.renderRecommendations()}
              </urltracker-result-list>
            `,
          })}
          ${this.renderPagination()}
        </div>
      </div>
    `;
  }

  static styles = css`
    .grid-root {
      display: grid;
      gap: 1rem;
    }

    .filters {
      grid-column: 1 / span 2;
      grid-row: 1;
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem 0;
    }

    .filters urltracker-recommendation-search {
      flex: 0 1 30%;
    }

    .bulk {
      grid-column: 1 / span 2;
      grid-row: 1;
    }

    .results {
      grid-column: 1 / span 2;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
  `;
}
