import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";
import { consume, provide } from "@lit/context";
import {
  IRedirectService,
  redirectServiceContext,
} from "../../context/redirectservice.context";
import { IRedirectCollectionResponse } from "../../services/redirect.service";
import {
  IChangeManager,
  changeManagerContext,
} from "../../context/changemanager.context";
import {
  ensureExists,
  ensureServiceExists,
} from "../../util/tools/existancecheck";
import "../../util/elements/resultlist.lit";
import "../../util/elements/resultlistitem.lit";
import "../../util/elements/inputs/pagination.lit";
import "../../util/elements/redirectActions.lit";
import "../../util/elements/inputs/addRedirectAction.lit";
import "../../util/elements/inputs/exportRedirectsAction.lit";
import "../../util/elements/inputs/redirectImport.lit";
import "./redirects/redirectitem.lit";
import { UrlTrackerPagination } from "../../util/elements/inputs/pagination.lit";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { repeat } from "lit/directives/repeat.js";
import { PropertyValueMap } from "lit";
import { ifDefined } from "lit/directives/if-defined.js";

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
    ensureServiceExists(this._redirectService, "redirect service");
    ensureExists(this.paginationRef.value);

    let page = this.paginationRef.value.value;
    this._loading++;
    try {
      this._redirectCollection = await this._redirectService?.list({ ...page });
    } finally {
      this._loading--;
    }
  }

  private renderRedirects(): unknown {
    if (!this._redirectCollection?.results) return nothing;
    return repeat(
      this._redirectCollection.results,
      (redirect) => redirect.id,
      (r) =>
        html`<urltracker-redirect-item .item=${r}></urltracker-redirect-item>`
    );
  }

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

  protected renderInternal(): unknown {
    return html`
      <div class="redirect-container">
        <div class="filters"></div>
        <div class="main">
          <urltracker-result-list
            class="results"
            .loading=${!!this._loading}
            .header=${`Results (${
              this._redirectCollection ? this._redirectCollection.total : 0
            })`}
          >
            ${this.renderRedirects()}
          </urltracker-result-list>
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
        <urltracker-pagination
          ${ref(this.paginationRef)}
          class="pagination"
          total="${ifDefined(this._redirectCollection?.total)}"
          @change=${this.onFilterChange}
        ></urltracker-pagination>
      </div>
    `;
  }

  static styles = css`
    .redirect-container {
      display: flex;
      flex-direction: column;
    }

    .filters {
      margin-bottom: 2rem;
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

    .filters {
      background-color: blue;
      height: 100px;
    }

    .results {
    }

    .functions {
      flex: 1 0 15rem;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
  `;
}
