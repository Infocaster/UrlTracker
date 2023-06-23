import { LitElement, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { IRecommendationCollection, IRecommendationsService } from "../../../services/recommendation.service";
import { consume } from "@lit-labs/context";
import { recommendationServiceContext } from "../../../context/recommendationservice.context";

@customElement("urltracker-recommendation-list")
export class UrlTrackerRecommendations extends LitElement {

    @consume({context:recommendationServiceContext})
    private _recommendationsService?: IRecommendationsService;

    @state()
    private _collection?: IRecommendationCollection;

    async connectedCallback(): Promise<void> {
        
        super.connectedCallback();

        this._collection = await this._recommendationsService?.list(1, 10);
    }

    protected render(): unknown {

        let collection: any = nothing;
        if (this._collection) {

            collection = html`
            <ul>
                ${this._collection.results.map((i) => html`<li>${i.url}</li>`)}
            </ul>`
        }

        return html`
        <div>
            ${collection}
        </div>
        `;
    }
}