import { ensureExists } from "@/util/tools/existancecheck";
import { consume } from "@lit/context";
import { UUIInputEvent } from "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { localizationServiceContext } from "../../../context/localizationservice.context";
import { ILocalizationService } from "../../../umbraco/localization.service";
import { debounce } from "../../../util/functions/debounce";

@customElement("urltracker-recommendation-search")
export class UrlTrackerRecommendationSearch extends LitElement {
  @state()
  private _placeholderText = "localize this";

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  private inputRef: Ref<HTMLInputElement> = createRef();

  async connectedCallback() {
    super.connectedCallback();
    
    ensureExists(this.localizationService);
    this.localizePlaceholderText();
  }

  private async localizePlaceholderText(): Promise<void> {
    const actionsText = await this.localizationService?.localize(
      "urlTrackerRecommendationFilter_search-placeholder"
    );

    this._placeholderText = actionsText ?? this._placeholderText;
  }

  private _onSearchInput = (e: UUIInputEvent) => {
    this._dispatchSearch(this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '');
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
     ${ref(this.inputRef)}
      .placeholder=${this._placeholderText}
      @input=${this._debouncedOnSearchInput}
    >
      <div class="prepend" slot="prepend">
          <uui-icon name="search"></uui-icon>
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
