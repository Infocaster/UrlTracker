import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import {
  IRecommendationCollection,
  IRecommendationsService,
} from "../../../services/recommendation.service";
import { consume } from "@lit/context";
import { recommendationServiceContext } from "../../../context/recommendationservice.context";
import "../../../util/elements/redirects/simpleRedirect/createSimpleRedirect.lit";
import { localizationServiceContext } from "../../../context/localizationservice.context";
import { ILocalizationService } from "../../../umbraco/localization.service";

@customElement("urltracker-recommendation-list")
export class UrlTrackerRecommendations extends LitElement {
  @consume({ context: recommendationServiceContext })
  private _recommendationsService?: IRecommendationsService;

  @state()
  private _collection?: IRecommendationCollection;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._collection = await this._recommendationsService?.list({page: 1, pageSize: 10});
  }

  protected render(): unknown {
    let collection: any = nothing;
    if (this._collection) {
      collection = html` <ul>
        ${this._collection.results.map((i) => html`<li>${i.url}</li>`)}
      </ul>`;
    }

    return html` <div>${collection}</div> `;
  }

  static styles = [
    css`
      .create-redirect {
        width: 575px;
        height: 685px;
        padding: 20px;
        background-color: rebeccapurple;
      }
    `,
  ];
}
