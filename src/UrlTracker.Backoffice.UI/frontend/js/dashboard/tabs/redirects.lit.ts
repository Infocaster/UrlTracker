import { REDIRECTTYPE_SORT_TYPE, RedirectSortType } from "@/enums/sortType";
import { DropdownChangeEvent, IDropdownValue } from "@/util/elements/inputs/dropdown.lit";
import { consume, provide } from "@lit/context";
import { LitElement, PropertyValueMap, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { ifDefined } from "lit/directives/if-defined.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { repeat } from "lit/directives/repeat.js";
import {
  IChangeManager,
  changeManagerContext,
} from "../../context/changemanager.context";
import {
  IRedirectService,
  redirectServiceContext,
} from "../../context/redirectservice.context";
import { IRedirectCollectionResponse } from "../../services/redirect.service";
import "../../util/elements/inputs/addRedirectAction.lit";
import "../../util/elements/inputs/exportRedirectsAction.lit";
import "../../util/elements/inputs/pagination.lit";
import { UrlTrackerPagination } from "../../util/elements/inputs/pagination.lit";
import "../../util/elements/inputs/redirectImport.lit";
import "../../util/elements/redirectActions.lit";
import "../../util/elements/resultlist.lit";
import "../../util/elements/resultlistitem.lit";
import {
  ensureExists,
  ensureServiceExists,
} from "../../util/tools/existancecheck";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";
import "./redirects/redirectitem.lit";
import "./redirects/redirectsSearch.lit";

@customElement("urltracker-redirect-tab")
export class UrlTrackerRedirectTab extends UrlTrackerNotificationWrapper(
  LitElement,
  "redirects"
) {
  @consume({ context: redirectServiceContext })
  private _redirectService?: IRedirectService;

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @state()
  private _redirectCollection?: IRedirectCollectionResponse;

  @state()
  private _loading: number = 0;

  private paginationRef: Ref<UrlTrackerPagination> = createRef();
  private query = "";
  private selectedType: RedirectSortType = REDIRECTTYPE_SORT_TYPE.ALL;

  private _sortOptions: IDropdownValue[] = [
    {
      display: "Alle",
      value: REDIRECTTYPE_SORT_TYPE.ALL,
      key: REDIRECTTYPE_SORT_TYPE.ALL.toString()
    },
    {
      display: "Permanent",
      value: REDIRECTTYPE_SORT_TYPE.PERMANENT,
      key: REDIRECTTYPE_SORT_TYPE.PERMANENT.toString()
    },
    {
      display: "Tijdelijk",
      value: REDIRECTTYPE_SORT_TYPE.TEMPORARY,
      key: REDIRECTTYPE_SORT_TYPE.TEMPORARY.toString()
    },
  ];

  protected async firstUpdated(
    _changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>
  ): Promise<void> {
    super.firstUpdated(_changedProperties);
    await this.init();
  }

  private async init() {
    ensureServiceExists(this._redirectService, "redirect service");
    await this.search();
  }

  private async search() {
    ensureExists(this.paginationRef.value);

    const page = this.paginationRef.value.value;
    const type = this.selectedType;
    const query = this.query;

    this._loading++;
    try {
      this._redirectCollection = await this._redirectService?.list({ ...page, types: type, query});
    } finally {
      this._loading--;
    }
  }

  private onSearch = ({ detail: { query } = {} }: CustomEvent) => {
    this.query = query;
    this.search();
  };

  private onTypeChange = ({ data }: DropdownChangeEvent) => {
    this.selectedType = data.value as RedirectSortType;
    this.search();
  };

  private onFilterChange = (_: Event) => {
    this.init();
  };

  private _onAddRedirect = (e: any) => {
    // TODO: implement logic
    console.info("add redirect");
    console.info(e);
  };

  private _onExportRedirects = (e: any) => {
    // TODO: implement logic
    console.info("export redirects");
    console.info(e);
  };

  private renderRedirects(): unknown {
    if (!this._redirectCollection?.results) return nothing;
    return repeat(
      this._redirectCollection.results,
      (redirect) => redirect.id,
      (r) =>
        html`<urltracker-redirect-item .item=${r}></urltracker-redirect-item>`
    );
  }

  protected renderInternal(): unknown {
    return html`
      <div class="grid-root">
        <div class="filters">
          <urltracker-redirects-search
            @search=${this.onSearch}
          ></urltracker-redirects-search>
          <urltracker-dropdown
            label="Type"
            .options=${this._sortOptions}
            @change=${this.onTypeChange}
          ></urltracker-dropdown>
        </div>
        <div class="results">
          <urltracker-result-list
            .loading=${!!this._loading}
            .header=${`Results (${
              this._redirectCollection ? this._redirectCollection.total : 0
            })`}
          >
            ${this.renderRedirects()}
          </urltracker-result-list>
          <urltracker-pagination
            ${ref(this.paginationRef)}
            class="pagination"
            total="${ifDefined(this._redirectCollection?.total)}"
            @change=${this.onFilterChange}
          ></urltracker-pagination>
        </div>
        <div class="functions">
          <urltracker-redirect-actions>
            <urltracker-add-redirect-action
              @click=${this._onAddRedirect}
            ></urltracker-add-redirect-action>
            <urltracker-export-redirects-action
              @click=${this._onExportRedirects}
            ></urltracker-export-redirects-action>
          </urltracker-redirect-actions>
          <urltracker-redirect-import></urltracker-redirect-import>
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

    .filters urltracker-redirects-search {
      flex: 0 1 30%;
    }

    .main {
      display: flex;
      margin-bottom: 2rem;
      gap: 2rem;
      flex-wrap: wrap;
    }

    urltracker-result-list {
      flex: 1 1 32rem;
    }

    .functions {
      flex: 1 0 15rem;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .results {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
  `;
}
