import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";
import { consume, provide } from "@lit-labs/context";
import { IRedirectService, redirectServiceContext } from "../../context/redirectservice.context";
import { IRedirectCollectionResponse } from "../../services/redirect.service";
import { IChangeManager, changeManagerContext } from "../../context/changemanager.context";
import { ensureExists, ensureServiceExists } from "../../util/tools/existancecheck";
import '../../util/elements/resultlist.lit';
import '../../util/elements/resultlistitem.lit';
import '../../util/elements/inputs/pagination.lit';
import './redirects/redirectitem.lit';
import { UrlTrackerPagination } from "../../util/elements/inputs/pagination.lit";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { repeat } from "lit/directives/repeat.js";
import { PropertyValueMap } from "lit";

@customElement("urltracker-redirect-tab")
export class UrlTrackerRedirectTab extends UrlTrackerNotificationWrapper(LitElement, 'redirects') {

    @consume({context: redirectServiceContext})
    private _redirectService?: IRedirectService;

    @provide({context: changeManagerContext})
    public changeManager: IChangeManager = { element: this };

    @state()
    private _redirectCollection?: IRedirectCollectionResponse

    @state()
    private _loading: number = 0;

    private paginationRef: Ref<UrlTrackerPagination> = createRef();

    private onFilterChange = (_: Event) => {

        this.init();
    }

    protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
        
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
        }
        finally {

            this._loading--;
        }
    }

    private renderRedirects(): unknown {
        
        if (!this._redirectCollection?.results) return nothing;
        return repeat(this._redirectCollection.results, (redirect) => redirect.id, (r) => html`<urltracker-redirect-item .item=${r}></urltracker-redirect-item>`);
    }

    protected renderInternal(): unknown {

        return html`
            <div class="grid-root">
                <div class="filters"></div>
                <urltracker-result-list class="results" .loading=${!!this._loading} .header=${`Results (${this._redirectCollection ? this._redirectCollection.total : 0})`}>
                    ${this.renderRedirects()}
                </urltracker-result-list>
                <div class="actions"></div>
                <urltracker-pagination ${ref(this.paginationRef)} class="pagination" total="${this._redirectCollection?.total}" @change=${this.onFilterChange}></urltracker-pagination>
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
            background-color: blue;
            height: 100px;
        }

        .results {
            grid-column: 1;
            grid-row: 2;
        }

        .actions {
            grid-column: 2;
            grid-row: 2 / span 2;
            background-color: green;
        }

        .pagination {
            grid-column: 1;
            grid-row: 3;
        }
    `;
}