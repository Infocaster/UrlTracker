import { LitElement, PropertyValueMap, css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { customElement, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { UrlTrackerPagination } from '../../util/elements/inputs/pagination.lit';
import { ensureExists, ensureServiceExists } from '../../util/tools/existancecheck';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';

import variableresourceService from '@/util/tools/variableresource.service';
import { consume, provide } from '@lit/context';
import { ifDefined } from 'lit/directives/if-defined.js';
import { repeat } from 'lit/directives/repeat.js';
import { IChangeManager, changeManagerContext } from '../../context/changemanager.context';
import { DropdownChangeEvent, IDropdownValue } from '../../util/elements/inputs/dropdown.lit';
import './recommendations/recommendationSearch.lit';
import './recommendations/recommendationitem.lit';
import { ISourceStrategies } from './redirects/source/source.constants';
import { ITargetStrategies } from './redirects/target/target.constants';
import { createNewRedirectOptions } from '../sidebars/simpleRedirect/manageredirect';
import {
  RecommendationCollectionResponse,
  RecommendationOrderBy,
  RecommendationResponse,
  RedirectRequest,
  getApiV1UrlTrackerRecommendations,
  postApiV1UrlTrackerRecommendationsByRecommendationId,
  postApiV1UrlTrackerRecommendationsUpdatebulk,
} from '@/api';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationContext } from '@umbraco-cms/backoffice/notification';
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from '@umbraco-cms/backoffice/modal';
import { URLTRACKER_EDIT_REDIRECT_MODAL } from '../sidebars/simpleRedirect/manifest';
import { URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL } from '../sidebars/explainRecommendations/manifest';
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from '../sidebars/explainRecommendations/explainrecommendations';
import { URLTRACKER_ANALYSE_RECOMMENDATION_MODAL } from '../sidebars/analyseRecommendation/manifest';

@customElement('urltracker-recommendations-tab')
export class UrlTrackerRecommendationsTab extends UrlTrackerNotificationWrapper(LitElement, 'recommendations') {
  private _notificationContext: UmbNotificationContext | undefined;
  private get notificationContext(): UmbNotificationContext {
    ensureServiceExists(this._notificationContext, 'notificationContext');
    return this._notificationContext;
  }

  private _modalManager: UmbModalManagerContext | undefined;
  private get modalManager(): UmbModalManagerContext {
    ensureServiceExists(this._modalManager, 'modalManager');
    return this._modalManager;
  }

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance?: UmbNotificationContext) => {
      this._notificationContext = instance;
    });

    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance?: UmbModalManagerContext) => {
      this._modalManager = instance;
    });
  }

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @state()
  private recommendationCollection?: RecommendationCollectionResponse;

  @state()
  private loading: number = 0;

  @state()
  private selectedItems: number[] = [];

  private query = '';
  private selectedType: RecommendationOrderBy = RecommendationOrderBy.IMPORTANCE;
  private paginationRef: Ref<UrlTrackerPagination> = createRef();

  private _sortOptions: IDropdownValue[] = [
    {
      display: 'Importance',
      value: RecommendationOrderBy.IMPORTANCE,
      key: RecommendationOrderBy.IMPORTANCE.toString(),
    },
    {
      display: 'Most recently updated',
      value: RecommendationOrderBy.MOST_RECENTLY_UPDATED,
      key: RecommendationOrderBy.MOST_RECENTLY_UPDATED.toString(),
    },
    {
      display: 'Url',
      value: RecommendationOrderBy.URL,
      key: RecommendationOrderBy.URL.toString(),
    },
  ];

  protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    await this.search();
  }

  private async search() {
    this.recommendationCollection = undefined;
    ensureExists(this.paginationRef.value);

    const page = {
      page: this.paginationRef.value!.value.page + 1,
      pageSize: this.paginationRef.value!.value.pageSize,
    };
    const type = this.selectedType;
    const query = this.query;

    this.loading++;
    try {
      const { data } = await tryExecuteAndNotify(
        this,
        getApiV1UrlTrackerRecommendations({ ...page, query, orderBy: type }),
      );
      if (data) {
        this.recommendationCollection = data;
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

    this.search();
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
      /* Nothing to do when the promise rejects */
    }
  }

  private async openExplanationPanel(data: RecommendationResponse) {
    const modal = this.modalManager.open(this, URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL, {
      data: {
        recommendation: data,
      },
    });

    try {
      const result = await modal.onSubmit();
      await this.submitExplanationPanel(data, result);
    } catch {
      /* Nothing to do when the promise rejects */
    }
  }

  private submitExplanationPanel = async (recommendation: RecommendationResponse, action: IRecommendationAction) => {
    switch (action) {
      case RECCOMENDATION_ACTIONS.MAKE_PERMANENT:
        await this.handleCreatePermanentRedirect(new CustomEvent('', { detail: recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.MAKE_TEMPORARY:
        await this.handleCreateTemporaryRedirect(new CustomEvent('', { detail: recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.IGNORE:
        await this.handleIgnore(new CustomEvent('', { detail: recommendation }));
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
      /* Nothing to do when the promise rejects */
    }
  }

  private onSearch = ({ detail: { query } = {} }: CustomEvent) => {
    this.query = query;
    this.search();
  };

  private onSortChange = ({ data }: DropdownChangeEvent) => {
    this.selectedType = data.value as RecommendationOrderBy;
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
    const selectedRecommendations =
      this.recommendationCollection?.results.filter((r) => this.selectedItems.some((i) => i === r.id)) || [];
    const bulkToUpdate = selectedRecommendations.map((r) => {
      return {
        id: r.id,
        data: {
          recommendationStrategy: r.strategy,
          ignore: true,
        },
      };
    });
    const { error } = await tryExecuteAndNotify(
      this,
      postApiV1UrlTrackerRecommendationsUpdatebulk({
        requestBody: bulkToUpdate,
      }),
    );

    if (!error) {
      this.notificationContext.peek('positive', {
        data: {
          headline: 'Recommendations ignored',
          message: 'All selected recommendations have been removed from the overview',
        },
      });
    }

    this.selectedItems = [];
    this.search();
  };

  private renderRecommendations(): unknown {
    if (!this.recommendationCollection?.results) return nothing;
    return repeat(
      this.recommendationCollection.results,
      (recommendation) => recommendation.id,
      (r) =>
        html`<urltracker-recommendation-item
          .item=${r}
          .isSelected=${this.selectedItems.some((i) => i === r.id)}
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
        <uui-button look="secondary" @click=${this.onIgnoreSelection}>
          <uui-icon name="delete"></uui-icon>
          Ignore
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
          label="Order by"
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
            .header=${`Results (${this.recommendationCollection ? this.recommendationCollection.total : 0})`}
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
