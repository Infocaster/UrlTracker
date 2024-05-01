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

import {
  IEditorService,
  editorServiceContext,
} from "@/context/editorservice.context";
import { redirectServiceContext } from "@/context/redirectservice.context";
import {
  IRedirectResponse,
  IRedirectService,
} from "@/services/redirect.service";
import variableresourceService from "@/util/tools/variableresource.service";
import { consume, provide } from "@lit/context";
import { ifDefined } from "lit/directives/if-defined.js";
import { repeat } from "lit/directives/repeat.js";
import {
  IChangeManager,
  changeManagerContext,
} from "../../context/changemanager.context";
import { recommendationServiceContext } from "../../context/recommendationservice.context";
import {
  RECOMMENDATION_SORT_TYPE,
  RecommendationSortType,
} from "../../enums/sortType";
import {
  DropdownChangeEvent,
  IDropdownValue,
} from "../../util/elements/inputs/dropdown.lit";
import {
  IRecommendationAction,
  RECCOMENDATION_ACTIONS,
} from "../sidebars/explainRecommendations/explainRecommendations.lit";
import "./recommendations/recommendationSearch.lit";
import "./recommendations/recommendationitem.lit";
import { ISourceStrategies } from "./redirects/source/source.constants";
import { ITargetStrategies } from "./redirects/target/target.constants";

@customElement("urltracker-recommendations-tab")
export class UrlTrackerRecommendationsTab extends UrlTrackerNotificationWrapper(
  LitElement,
  "recommendations"
) {
  @consume({ context: recommendationServiceContext })
  private _recommendationsService?: IRecommendationsService;

  @consume({ context: redirectServiceContext })
  private _redirectService?: IRedirectService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @state()
  private recommendationCollection?: IRecommendationCollection;

  @state()
  private loading: number = 0;

  @state()
  private selectedItems: number[] = [];

  private query = "";
  private selectedType: RecommendationSortType =
    RECOMMENDATION_SORT_TYPE.LAST_OCCURRENCE;
  private paginationRef: Ref<UrlTrackerPagination> = createRef();

  private _sortOptions: IDropdownValue[] = [
    {
      display: "Last occurrance descending",
      value: RECOMMENDATION_SORT_TYPE.LAST_OCCURRENCE,
      key: RECOMMENDATION_SORT_TYPE.LAST_OCCURRENCE.toString(),
    },
    {
      display: "Importance",
      value: RECOMMENDATION_SORT_TYPE.IMPORTANCE,
      key: RECOMMENDATION_SORT_TYPE.IMPORTANCE.toString(),
    },
    {
      display: "Url",
      value: RECOMMENDATION_SORT_TYPE.URL,
      key: RECOMMENDATION_SORT_TYPE.URL.toString(),
    },
    {
      display: "Amount of occurrences",
      value: RECOMMENDATION_SORT_TYPE.OCCURRENCES,
      key: RECOMMENDATION_SORT_TYPE.OCCURRENCES.toString(),
    },
  ];

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
    ensureServiceExists(this._redirectService, "redirect service");
    ensureExists(this._recommendationsService, "recommendations service");
    ensureServiceExists(this.editorService, "editor service");

    await this.search();
  }

  private async search() {
    ensureExists(this.paginationRef.value);

    const page = {
      page: this.paginationRef.value!.value.page + 1,
      pageSize: this.paginationRef.value!.value.pageSize,
    }
    const type = this.selectedType;
    const query = this.query;

    this.loading++;
    try {
      this.recommendationCollection = await this._recommendationsService?.list({ ...page, query, OrderBy: type});
    } finally {
      this.loading--;
    }
  }

  private handleCreatePermanentRedirect = async (
    event: CustomEvent<IRecommendationResponse>
  ) => {
    const redirect = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>(
          "redirectSourceStrategies"
        ).url,
        value: event.detail.url,
      },
      target: {
        strategy: variableresourceService.get<ITargetStrategies>(
          "redirectTargetStrategies"
        ).url,
        value: "",
      },
      permanent: true,
      retainQuery: true,
      force: false,
    } as IRedirectResponse;

    this.openNewRedirectPanel(redirect);
  };

  private handleCreateTemporaryRedirect = async (
    event: CustomEvent<IRecommendationResponse>
  ) => {
    const redirect = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>(
          "redirectSourceStrategies"
        ).url,
        value: event.detail.url,
      },
      target: {
        strategy: variableresourceService.get<ITargetStrategies>(
          "redirectTargetStrategies"
        ).url,
        value: "",
      },
      permanent: false,
      retainQuery: true,
      force: false,
    } as IRedirectResponse;

    this.openNewRedirectPanel(redirect);
  };

  private handleIgnore = async (event: CustomEvent<IRecommendationResponse>) => {
    await this._recommendationsService!.update({
      id: event.detail.id,
      recommendationStrategy: event.detail.strategy,
      ignore: true,
    });

    this.search();
  };

  private openNewRedirectPanel(data?: IRedirectResponse) {
    const options = {
      title: "New redirect",
      view: "/App_Plugins/UrlTracker/sidebar/redirect/simpleRedirect.html",
      size: "medium",
      submit: this.submitNewRedirectPanel,
      close: this.closePanel,
      value: data,
    };

    this.editorService!.open(options);
  }

  submitNewRedirectPanel = async (value: IRedirectResponse) => {
    console.info("submit new or update redirect", value);
    if (value.id) {
      await this._redirectService?.update(value);
    } else {
      await this._redirectService?.create(value);
    }

    this.closePanel();
    this.search();
  };

  private openExplanationPanel(data: IRecommendationResponse) {
    const options = {
      title: `Recommendations for: ${data.url}`,
      view: "/App_Plugins/UrlTracker/sidebar/recommendations/inspectRecommendations.html",
      size: "medium",
      submit: this.submitExplanationPanel,
      close: this.closePanel,
      value: data,
    };
    this.editorService!.open(options);
  }

  private submitExplanationPanel = (payload: {
    recommendation: IRecommendationResponse;
    action: IRecommendationAction;
  }) => {
    this.editorService!.close();
    switch (payload.action) {
      case RECCOMENDATION_ACTIONS.MAKE_PERMANENT:
        this.handleCreatePermanentRedirect(new CustomEvent("", { detail: payload.recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.MAKE_TEMPORARY:
        this.handleCreateTemporaryRedirect(new CustomEvent("", { detail: payload.recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.IGNORE:
        this.handleIgnore(new CustomEvent("", { detail: payload.recommendation }));
        break;
    }
  };

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
  };

  private onDeselectItem = (e: any) => {
    this.selectedItems = this.selectedItems.filter((i) => i !== e.item.id);
  };

  private onSelectAll = (e: any) => {
    if (this.selectedItems.length === this.recommendationCollection?.total) {
      this.selectedItems = [];
    } else {
      this.selectedItems =
        this.recommendationCollection?.results.map((r) => r.id) || [];
    }
  };

  private onClearSelection = (e: any) => {
    this.selectedItems = [];
  };

  private onDeleteSelection = async (e: any) => {
    const selectedRedirects =
      this.recommendationCollection?.results.filter((r) =>
        this.selectedItems.some((i) => i === r.id)
      ) || [];
    const bulkToDelete = selectedRedirects.map((r) => r.id);
    //await redirectService.deleteBulk(bulkToDelete);
    this.selectedItems = [];
    //this.search();
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
        ></urltracker-recommendation-item>`
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
        .total=${this.recommendationCollection
          ? this.recommendationCollection.total
          : 0}
        @select-all=${this.onSelectAll}
        @clear-selection=${this.onClearSelection}
      >
        <uui-button look="secondary" @click=${this.onDeleteSelection}>
          <uui-icon name="delete"></uui-icon>
          Delete
        </uui-button>
      </urltracker-bulk-actions>
    `;
  }

  protected renderInternal(): unknown {
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
            .loading=${!!this.loading}
            .header=${`Results (${
              this.recommendationCollection
                ? this.recommendationCollection.total
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
