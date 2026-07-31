import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';
import { LitElement, css, html } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { debounce } from '../../../util/functions/debounce';

@customElement('urltracker-recommendation-search')
export class UrlTrackerRecommendationSearch extends UmbElementMixin(LitElement) {
  @state()
  private _placeholderText = 'localize this';

  private inputRef: Ref<HTMLInputElement> = createRef();

  async connectedCallback() {
    super.connectedCallback();

    this.localizePlaceholderText();
  }

  private localizePlaceholderText() {
    const actionsText = this.localize.term('urlTrackerRecommendationFilter_search-placeholder');

    this._placeholderText = actionsText ?? this._placeholderText;
  }

  private _onSearchInput = (_: UUIInputEvent) => {
    this._dispatchSearch(this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '');
  };

  private _dispatchSearch = (searchQuery: string) => {
    this.dispatchEvent(
      new CustomEvent('search', {
        detail: {
          query: searchQuery,
        },
        bubbles: true,
        composed: false,
      }),
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
        display: flex;
        align-items: center;
        margin-left: 0.5rem;
      }
    `,
  ];
}
