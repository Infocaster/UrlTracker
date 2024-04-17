import { LitElement, PropertyValueMap, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import {
  IRecommendationCollection,
  IRecommendationsService,
} from "../../services/recommendation.service";
import { UrlTrackerPagination } from "../../util/elements/inputs/pagination.lit";
import {
  ensureExists,
  ensureServiceExists,
} from "../../util/tools/existancecheck";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";

import { consume, provide } from "@lit/context";
import { repeat } from "lit/directives/repeat.js";
import {
  IChangeManager,
  changeManagerContext,
} from "../../context/changemanager.context";
import { recommendationServiceContext } from "../../context/recommendationservice.context";
import { RECOMMENDATION_SORT_TYPE } from "../../enums/sortType";
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

  private onFilterChange = (_: Event) => {
    this.init();
  };

  protected async firstUpdated(
    _changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>
  ): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    ensureServiceExists(
      this._recommendationsService,
      "recommendations service"
    );
    // ensureExists(this.paginationRef.value);

    // let page = this.paginationRef.value.value;
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

      this._loading++;

      this._recommendationCollection = await this._recommendationsService?.list(
        { ...page }
      );

      this._totalPages = this._recommendationCollection.total;
    } catch (error: any) {
      this._error = error.message;
    } finally {
      this._loading--;
    }
  }

  private renderRecommendations(): unknown {
    if (!this._recommendationCollection?.results) return nothing;
    return repeat(
      this._recommendationCollection.results,
      (recommendation) => recommendation.id,
      (r) =>
        html`<urltracker-recommendation-item
          .item=${r}
        ></urltracker-recommendation-item>`
    );
  }

  private renderPagination(): unknown {
    return html` ${this._totalPages}
      <urltracker-pagination
        ${ref(this.paginationRef)}
        class="pagination"
        .total=${this._totalPages}
        .testpages=${this._totalPages}
        @change=${this.onFilterChange}
      ></urltracker-pagination>`;
  }
  private _onSearch = ({ detail: { query } = {} }: CustomEvent) => {
    //TODO: implement search
    console.info("recommendations.lit.ts _onSearch not implemented");
    console.info(query);
  };

  private _onSortChange = ({ data }: DropdownChangeEvent) => {
    //TODO: implement sort
    console.info("recommendations.lit.ts _onSortChange not implemented");
    console.info(data);
  };

  protected renderInternal(): unknown {
    if (this._error !== null) {
      return html`<div class="error">${this._error}</div>`;
    }

    return html`
      <div class="grid-root">
        <div class="filters">
          <urltracker-recommendation-search
            @search=${this._onSearch}
          ></urltracker-recommendation-search>
          <urltracker-dropdown
            label="Order by"
            .options=${this._sortOptions}
            @change=${this._onSortChange}
          ></urltracker-dropdown>
        </div>
        <urltracker-result-list
          class="results"
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
    `;
  }

  static styles = css`
    .grid-root {
      display: grid;
      grid-template-columns: 2;
      grid-template-rows: 3;
      gap: 16px;
    }

    .filters {
      grid-column: 1 / span 2;
      grid-row: 1;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .filters urltracker-recommendation-search {
      flex: 0 1 30%;
    }

    .results {
      grid-column: 1 / span 2;
      grid-row: 2;
    }

    .pagination {
      grid-column: 1;
      grid-row: 3;
    }
  `;
}
