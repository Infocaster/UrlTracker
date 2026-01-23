import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, PropertyValueMap, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import type { EntityWithIdRequest1, RecommendationResponse } from '../../../../api-client/types.gen';
import { UrlTrackerPagination } from '../../util/elements/inputs/pagination.lit';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';

import { LoadingStatus } from '@/types/loadingStatus';
import variableresourceService from '@/util/tools/variableresource.service';
import { provide } from '@lit/context';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { ifDefined } from 'lit/directives/if-defined.js';
import { repeat } from 'lit/directives/repeat.js';
import type { Client } from '../../../../api-client/client/types.gen';
import {
  getUmbracoManagementApiV1UrlTrackerRecommendations,
  postUmbracoManagementApiV1UrlTrackerRecommendationsByRecommendationId,
  postUmbracoManagementApiV1UrlTrackerRecommendationsUpdatebulk,
} from '../../../../api-client/sdk.gen';
import type { RedirectRequest } from '../../../../api-client/types.gen';
import { RecommendationCollectionResponse } from '../../../../api-client/types.gen';
import { IChangeManager, changeManagerContext } from '../../context/changemanager.context';
import { RECOMMENDATION_SORT_TYPE, RecommendationSortType } from '../../enums/sortType';
import { DropdownChangeEvent, IDropdownValue } from '../../util/elements/inputs/dropdown.lit';
import { URL_TRACKER_ANALYSE_RECOMMENDATION_MODAL } from '../sidebars/analyseRecommendation-modal.token';
import { URL_TRACKER_EXPLAIN_RECOMMENDATION_MODAL } from '../sidebars/explainRecommendation-modal.token';
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from '../sidebars/explainRecommendations/explainRecommendations.lit';
import { URL_TRACKER_SIMPLE_REDIRECT_MODAL } from '../sidebars/simpleRedirect-modal.token';
import './recommendations/recommendationSearch.lit';
import './recommendations/recommendationitem.lit';
import './recommendations/recommendationitemSkeleton.lit';
import { ISourceStrategies } from './redirects/source/source.constants';
import { ITargetStrategies } from './redirects/target/target.constants';

@customElement('urltracker-recommendations-tab')
export class UrlTrackerRecommendationsTab extends UrlTrackerNotificationWrapper(
  UmbElementMixin(LitElement),
  'recommendations',
) {
  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @state()
  private recommendationCollection?: RecommendationCollectionResponse;

  @state()
  private loading: number = 0;

  @state()
  private selectedItems: number[] = [];

  @state()
  private ignoreRecommendationLoadingIds: Array<number> = [];

  @state()
  private ignoreSelectionState: LoadingStatus = undefined;

  private query = '';
  private selectedType: RecommendationSortType = RECOMMENDATION_SORT_TYPE.IMPORTANCE;
  private paginationRef: Ref<UrlTrackerPagination> = createRef();

  #notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });
  }

  private get _sortOptions(): IDropdownValue[] {
    return [
      {
        display: this.localize.term('urlTrackerGeneral_importance') || 'Importance',
        value: RECOMMENDATION_SORT_TYPE.IMPORTANCE,
        key: RECOMMENDATION_SORT_TYPE.IMPORTANCE.toString(),
      },
      {
        display: this.localize.term('urlTrackerGeneral_most-recently-updated') || 'Most recently updated',
        value: RECOMMENDATION_SORT_TYPE.MOST_RECENTLY_UPDATED,
        key: RECOMMENDATION_SORT_TYPE.MOST_RECENTLY_UPDATED.toString(),
      },
      {
        display: this.localize.term('urlTrackerGeneral_url') || 'Url',
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
    await this.search();
  }

  private async search() {
    this.recommendationCollection = undefined;

    const page = {
      page: this.paginationRef.value!.value.page + 1,
      pageSize: this.paginationRef.value!.value.pageSize,
    };
    const type = this.selectedType;
    const query = this.query;

    this.loading++;
    try {
      const { data } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerRecommendations({
          client: umbHttpClient as unknown as Client,
          query: {
            Page: page.page,
            PageSize: page.pageSize,
            Query: query,
            OrderBy: type,
            Desc: this._sortDirectionMap[type],
          },
        }),
      );

      this.recommendationCollection = data;
    } catch (error: any) {
      console.error(error);
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
      .then(() => {
        this.submitNewRedirectPanel();
      })
      .catch(() => this.closePanel());
  }

  private submitNewRedirectPanel = () => {
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

  private onSearch = ({ detail: { query } = {} }: CustomEvent) => {
    this.query = query;
    this.search();
  };

  private onSortChange = ({ data }: DropdownChangeEvent) => {
    this.selectedType = data.value as RecommendationSortType;
    this.search();
  };

  private onExplain = (e: CustomEvent<RecommendationResponse>) => {
    this.openExplanationPanel(e.detail);
  };

  private onAnalyse = (e: CustomEvent<RecommendationResponse>) => {
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
      const bulkToUpdate: EntityWithIdRequest1[] = selectedRecommendations.map((r) => {
        return {
          id: r.id,
          data: {
            recommendationStrategy: r.strategy,
            ignore: true,
          },
        };
      });

      await tryExecute(
        this,
        postUmbracoManagementApiV1UrlTrackerRecommendationsUpdatebulk({
          client: umbHttpClient as unknown as Client,
          body: bulkToUpdate,
        }),
      );

      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_recommendations-ignored') || 'Recommendations ignored',
          message:
            this.localize.term('urlTrackerGeneral_recommendations-ignored-message') ||
            'All selected recommendations have been removed from the overview',
        },
      });

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
          <umb-localize key="urlTrackerGeneral_ignore"></umb-localize>
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
          label="${this.localize.term('urlTrackerGeneral_order-by') || 'Order by'}"
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
          <urltracker-result-list
            .loading=${!!this.loading}
            .header=${`${this.localize.term('urlTrackerGeneral_results') || 'Results'} (${this.recommendationCollection ? this.recommendationCollection.total : 0})`}
          >
            ${this.renderRecommendations()}
          </urltracker-result-list>
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
