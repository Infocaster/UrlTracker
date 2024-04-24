import { debounce } from "@/util/functions/debounce";
import { consume } from "@lit/context";
import { UUIInputEvent } from "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { localizationServiceContext } from "../../../../context/localizationservice.context";
import { ILocalizationService } from "../../../../umbraco/localization.service";

@customElement("urltracker-redirect-incoming-url")
export class UrlTrackerRedirectIncomingUrl extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @property({ type: String })
  private _data: string = "";

  @state()
  private _headerText: string = "";

  @state()
  private _infoText: string = "";

  private inputRef: Ref<HTMLInputElement> = createRef();

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this._localizeHeaderText();
    this._localizeInfoText();
  }

  private _localizeHeaderText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_incoming-url"
    );

    this._headerText = text ?? "";
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_incoming-url-info"
    );

    this._infoText = text ?? "";
  };

  private onInput = (e: UUIInputEvent) => {
    this.dispatchEvent(
      new CustomEvent("input", {
        detail: this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '',
        bubbles: true,
        composed: false,
      })
    );
  };

  private _debouncedOnInput = debounce(this.onInput, 500);

  protected render(): unknown {
    return html`
      <p><strong>${this._headerText}</strong></p>
      <p>${this._infoText}</p>
      <uui-input
        ${ref(this.inputRef)}
        .value=${this._data}
        placeholder="https://example.com/"
        @input=${this._debouncedOnInput}
      ></uui-input>
    `;
  }

  static styles = [
    css`
      :host {
        display: block;
      }

      uui-input {
        width: 100%;
      }
    `,
  ];
}
