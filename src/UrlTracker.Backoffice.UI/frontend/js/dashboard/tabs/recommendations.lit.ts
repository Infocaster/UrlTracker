import { LitElement, PropertyValueMap, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import {
  IRecommendationCollection,
  IRecommendationResponse,
  IRecommendationsService,
} from "../../services/recommendation.service";
import { UrlTrackerPagination } from "../../util/elements/inputs/pagination.lit";
import {
  ensureExists,
  ensureServiceExists,
} from "../../util/tools/existancecheck";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";

import { IEditorService, editorServiceContext } from "@/context/editorservice.context";
import { consume, provide } from "@lit/context";
import { repeat } from "lit/directives/repeat.js";
import {
  IChangeManager,
  changeManagerContext,
} from "../../context/changemanager.context";
import { recommendationServiceContext } from "../../context/recommendationservice.context";
import { RECOMMENDATION_SORT_TYPE, RecommendationSortType } from "../../enums/sortType";
import { DropdownChangeEvent, IDropdownValue } from "../../util/elements/inputs/dropdown.lit";
import "./recommendations/recommendationSearch.lit";
import "./recommendations/recommendationitem.lit";

@customElement("urltracker-recommendations-tab")
export class UrlTrackerRecommendationsTab extends UrlTrackerNotificationWrapper(
  LitElement,
  "recommendations"
) {
  @consume({ context: recommendationServiceContext })
  private _recommendationsService?: IRecommendationsService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @state()
  private _recommendationCollection?: IRecommendationCollection;

  @state()
  private _loading: number = 0;

  @state()
  private _error: string | null = null;

  @state()
  private _totalPages = 0;

  @state()
  private selectedItems: number[] = [];

  private query = "";
  private selectedType: RecommendationSortType = RECOMMENDATION_SORT_TYPE.LAST_OCCURRENCE;
  private paginationRef: Ref<UrlTrackerPagination> = createRef();

  private _sortOptions: IDropdownValue[] = [
    {
      display: "Last occurrance descending",
      value: RECOMMENDATION_SORT_TYPE.LAST_OCCURRENCE,
      key: RECOMMENDATION_SORT_TYPE.LAST_OCCURRENCE.toString()
    },
    {
      display: "Importance",
      value: RECOMMENDATION_SORT_TYPE.IMPORTANCE,
      key: RECOMMENDATION_SORT_TYPE.IMPORTANCE.toString()
    },
    {
      display: "Url",
      value: RECOMMENDATION_SORT_TYPE.URL,
      key: RECOMMENDATION_SORT_TYPE.URL.toString()
    },
    {
      display: "Amount of occurrences",
      value: RECOMMENDATION_SORT_TYPE.OCCURRENCES,
      key: RECOMMENDATION_SORT_TYPE.OCCURRENCES.toString()
    },
  ];

  protected async firstUpdated(
    _changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>
  ): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    ensureServiceExists(this._recommendationsService, "recommendations service");
    ensureServiceExists(this.editorService, "editor service");

    await this.search();
  }

  private async search() {
    this._loading++;
    try {
      this._recommendationCollection = await this._recommendationsService?.list(
        {
          page: 1,
          pageSize: 25,
        }
      );
      ensureExists(
        this.paginationRef.value?.value,
        "pagination ref does not exist"
      );

      let page = this.paginationRef.value.value;

      if (page.page < 1) page.page = 1;

      this._recommendationCollection = await this._recommendationsService?.list(
        { ...page }
      );
    } catch (error: any) {
      this._error = error.message;
    } finally {
      this._loading--;
    }
  }

  private openExplanationPanel(data: IRecommendationResponse) {
    const options = {
      title: `Recommendations for: ${data.url}`,
      view: "/App_Plugins/UrlTracker/sidebar/recommendations/inspectRecommendations.html",
      size: "medium",
      submit: this.closePanel,
      close: this.closePanel,
      value: data,
    };
    this.editorService!.open(options);
  }

  private openAnalysePanel(data: IRecommendationResponse) {
    const options = {
      title: `Recommendations for: ${data.url}`,
      view: "/App_Plugins/UrlTracker/sidebar/recommendations/analyseRecommendation.html",
      size: "medium",
      submit: this.closePanel,
      close: this.closePanel,
      value: data,
    };
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
  }

  private onDeselectItem = (e: any) => {
    this.selectedItems = this.selectedItems.filter(i => i !== e.item.id);
  }

  private onSelectAll = (e: any) => {
    if(this.selectedItems.length === this._recommendationCollection?.total) {
      this.selectedItems = [];
    }
    else {
      this.selectedItems = this._recommendationCollection?.results.map(r => r.id) || [];
    }
  }

  private onClearSelection = (e: any) => {
    this.selectedItems = [];
  }

  private onDeleteSelection = async (e: any) => {
    const selectedRedirects = this._recommendationCollection?.results.filter(r => this.selectedItems.some(i => i === r.id)) || [];
    const bulkToDelete = selectedRedirects.map(r => r.id);
    //await redirectService.deleteBulk(bulkToDelete);
    this.selectedItems = [];
    //this.search();
  }

  private renderRecommendations(): unknown {
    if (!this._recommendationCollection?.results) return nothing;
    return repeat(
      this._recommendationCollection.results,
      (recommendation) => recommendation.id,
      (r) =>
        html`<urltracker-recommendation-item
          .item=${r}
          .isSelected=${this.selectedItems.some(i => i === r.id)} 
          @selected=${this.onSelectItem} 
          @deselected=${this.onDeselectItem} 
          @explain=${this.onExplain}
          @analyse=${this.onAnalyse}
        ></urltracker-recommendation-item>`
    );
  }

  private renderPagination(): unknown {
    return html`
      <urltracker-pagination
        ${ref(this.paginationRef)}
        class="pagination"
        .total=${this._totalPages}
        .testpages=${this._totalPages}
        @change=${this.onFilterChange}
      ></urltracker-pagination>`;
  }

  private renderBulkActions(): unknown {
    if(!this.selectedItems.length) return nothing;
    return html`
      <urltracker-bulk-actions
        class="bulk"
        .selectedCount=${this.selectedItems.length}
        .total=${this._recommendationCollection ? this._recommendationCollection.total : 0}
        @select-all=${this.onSelectAll}
        @clear-selection=${this.onClearSelection}
      >
        <uui-button
          look="secondary"
          @click=${this.onDeleteSelection}
        >
          <uui-icon name="delete"></uui-icon>
          Delete
        </uui-button>
      </urltracker-bulk-actions>
    `;
  }

  protected renderInternal(): unknown {
    if (this._error !== null) {
      return html`<div class="error">${this._error}</div>`;
    }

    return html`
      <div class="grid-root">
        <div class="filters">
          <urltracker-recommendation-search
            @search=${this.onSearch}
          ></urltracker-recommendation-search>
          <urltracker-dropdown
            label="Order by"
            .options=${this._sortOptions}
            @change=${this.onSortChange}
          ></urltracker-dropdown>
        </div>

        ${this.renderBulkActions()}

        <div class="results">
          <urltracker-result-list
            .loading=${!!this._loading}
            .header=${`Results (${
              this._recommendationCollection
                ? this._recommendationCollection.total
                : 0
            })`}
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
    }

    .filters urltracker-recommendation-search {
      flex: 0 1 30%;
    }

    .bulk {
      grid-column: 1 / span 2;
      grid-row: 2;
    }

    .results {
      grid-column: 1 / span 2;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
  `;
}
