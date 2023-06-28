import { LitElement, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";
import { consume } from "@lit-labs/context";
import { IRedirectService, redirectServiceContext } from "../../context/redirectservice.context";
import { IRedirectCollectionResponse } from "../../services/redirect.service";
import '../../util/elements/resultlist.lit';
import '../../util/elements/resultlistitem.lit';
import './redirects/redirectitem.lit';

@customElement("urltracker-redirect-tab")
export class UrlTrackerRedirectTab extends UrlTrackerNotificationWrapper(LitElement, 'redirects') {

    @consume({context: redirectServiceContext})
    private _redirectService?: IRedirectService;

    @state()
    private _redirectCollection?: IRedirectCollectionResponse

    @state()
    private _loading: number = 0;

    async connectedCallback(): Promise<void> {
        super.connectedCallback();

        this._loading++;
        try{

            this._redirectCollection = await this._redirectService?.list({page: 1, pageSize: 10});
        }
        finally{

            this._loading--;
        }
    }

    protected renderInternal(): unknown {
        
        let redirects = null;
        if (this._redirectCollection?.results){
            redirects = html`
                ${this._redirectCollection.results.map(r => html`<urltracker-redirect-item .item=${r}></urltracker-redirect-item>`)}
            `;
        }

        return html`
            <h2>Redirects</h2>
            <div>
                <urltracker-result-list .loading=${!!this._loading} .header=${`Results (${this._redirectCollection ? this._redirectCollection.total : 0})`}>
                    ${redirects}
                </urltracker-result-list>
            </div>
        `;
    }
}