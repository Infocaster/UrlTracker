import { consume } from "@lit/context";
import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { localizationServiceContext } from "../../../context/localizationservice.context";
import { ILocalizationService } from "../../../umbraco/localization.service";
import { debounce } from "../../../util/functions/debounce";

@customElement("urltracker-recommendation-search")
export class UrlTrackerRecommendationSearch extends LitElement {
  @state()
  private _placeholderText = "localize this";

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (!this.localizationService)
      throw new Error("This element requires the localization service");

    this.localizePlaceholderText();
  }

  private async localizePlaceholderText(): Promise<void> {
    const actionsText = await this.localizationService?.localize(
      "urlTrackerRecommendationFilter_search-placeholder"
    );

    this._placeholderText = actionsText ?? this._placeholderText;
  }

  // uui docs dont specifiy what the event type is
  private _onSearchInput = (e: any) => {
    this._dispatchSearch(e.explicitOriginalTarget.value);
  };

  private _dispatchSearch = (searchQuery: string) => {
    this.dispatchEvent(
      new CustomEvent("search", {
        detail: {
          query: searchQuery,
        },
        bubbles: true,
        composed: false,
      })
    );
  };

  private _debouncedOnSearchInput = debounce(this._onSearchInput, 500);

  protected render(): unknown {
    return html` <uui-input
      .placeholder=${this._placeholderText}
      @input=${this._debouncedOnSearchInput}
    >
      <div class="prepend" slot="prepend">
        <uui-icon-registry-essential>
          <uui-icon name="search"></uui-icon>
        </uui-icon-registry-essential>
      </div>
    </uui-input>`;
  }

  static styles = [
    css`
      uui-input {
        width: 100%;
      }

      .prepend {
        margin-left: 0.5rem;
      }
    `,
  ];
}
