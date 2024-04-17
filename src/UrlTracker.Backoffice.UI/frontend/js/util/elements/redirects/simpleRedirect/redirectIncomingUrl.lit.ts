import { LitElement, css, html, nothing } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { consume } from "@lit/context";
import { localizationServiceContext } from "../../../../context/localizationservice.context";
import { ILocalizationService } from "../../../../umbraco/localization.service";

@customElement("urltracker-redirect-incoming-url")
export class UrlTrackerRedirectIncomingUrl extends LitElement {
  @property({ type: String })
  private _data: string = "";

  @state()
  private _headerText: string = "";

  @state()
  private _infoText: string = "";

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    // if (!this._localizationService)
    //   throw new Error(
    //     "localization service is not defined, but is required by this element"
    //   );

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

  private _onInput = (e: any) => {
    console.log(e);
  };

  protected render(): unknown {
    return html`
      <p><strong>${this._headerText}</strong></p>
      <p>${this._infoText}</p>
      <uui-input
        .value=${this._data}
        placeholder="https://example.com/"
        @input=${this._onInput}
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
