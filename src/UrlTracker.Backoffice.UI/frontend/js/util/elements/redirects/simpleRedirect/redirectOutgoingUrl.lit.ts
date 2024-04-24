import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { debounce } from "@/util/functions/debounce";
import { consume } from "@lit/context";
import { UUIInputEvent } from "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { repeat } from "lit/directives/repeat.js";
import "./simpleRedirectTypeProvider";
import { ITypeButton } from "./simpleRedirectTypeProvider";

@customElement("urltracker-redirect-outgoing-url")
export class UrlTrackerRedirectOutgoingUrl extends LitElement {
  @property({ type: String })
  private _data: string = "";

  @state()
  private _headerText: string = "";

  @state()
  private _infoText: string = "";

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  private inputRef: Ref<HTMLInputElement> = createRef();

  public _typeButtons = [
    {
      label: "urlTrackerNewRedirect_outgoing-url-content",
      labelFallback: "Content",
      value: "content",
      placeholder: "link to content placeholder",
    },
    {
      label: "urlTrackerNewRedirect_outgoing-url-media",
      labelFallback: "Media",
      value: "media",
      placeholder: "link to media placeholder",
    },
    {
      label: "urlTrackerNewRedirect_outgoing-url-url",
      labelFallback: "URL",
      value: "url",
      placeholder: "https://example.com/",
    },
  ] as ITypeButton[];

  @state()
  private _selectedType: ITypeButton = this._typeButtons[0];

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    
    // if (!this._localizationService)
    //   throw new Error(
    //     "localization service is not defined, but is required by this element"
    //   );

    this._localizeHeaderText();
    this._localizeInfoText();
    this._localizeButtonLabels();
  }

  private _localizeHeaderText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_outgoing-url"
    );

    this._headerText = text ?? "Outgoing URL fallback";
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_outgoing-url-info"
    );

    this._infoText = text ?? "Select where the URL should redirect to";
  };

  private _localizeButtonLabels = async () => {
    const labels = await this._localizationService?.localizeMany(
      this._typeButtons.map((item) => item.label)
    );

    this._typeButtons = this._typeButtons.map((item, index) => ({
      ...item,
      label: labels?.[index] ?? item.labelFallback,
    }));
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

  private onTypeChange = (item: ITypeButton, e: Event) => {
    this._selectedType = item;
    this.dispatchEvent(
      new CustomEvent("typechange", {
        detail: item,
        bubbles: true,
        composed: false,
      })
    );
  };

  protected render(): unknown {
    return html`
      <p>
        <strong>${this._headerText} <span class="required">*</span></strong>
      </p>
      <p>${this._infoText}</p>
      <uui-button-group>
        ${repeat(
          this._typeButtons,
          (item) => item.value,
          (item) => html` <uui-button
            label=${item.label}
            look=${this._selectedType.value === item.value
              ? "primary"
              : "outline"}
            color="default"
            @click=${(e: Event) => this.onTypeChange(item, e)}
          ></uui-button>`
        )}
      </uui-button-group>
      <uui-input
        ${ref(this.inputRef)}
        .value=${this._data}
        .placeholder=${this._selectedType.placeholder}
        @input=${this._debouncedOnInput}
      ></uui-input>
    `;
  }

  static styles = [
    css`
      :host {
        display: block;
      }

      uui-button-group {
        width: 100%;
        margin-bottom: 1rem;
        flex-wrap: wrap;
      }

      uui-input {
        width: 100%;
      }

      .required {
        color: #ba0000;
      }
    `,
  ];
}
