import { consume } from "@lit/context";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { localizationServiceContext } from "../../../../context/localizationservice.context";
import { ILocalizationService } from "../../../../umbraco/localization.service";

@customElement("urltracker-redirect-preserve-querystring")
export class UrlTrackerRedirectPreserveQuerystring extends LitElement {
  @property({ type: Boolean })
  public preserve: boolean = false;

  @state()
  private _headerText: string = "";

  @state()
  private _infoText: string = "";

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeInfoText();
  }

  private _localizeHeaderText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_permanent"
    );

    this._headerText = text ?? "fallback";
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_permanent-info"
    );

    this._infoText = text ?? "fallback";
  };

  private _onToggleChange = (e: any) => {
    this.preserve = !this.preserve;

    this.dispatchEvent(
      new CustomEvent("toggle", {
        detail: this.preserve,
        bubbles: true,
        composed: true,
      })
    );
  };

  protected render(): unknown {
    return html`
      <p><strong>Preserve query string</strong></p>
      <p>The query string is the part behind the ? in a URL and consists of so-called “key/value pairs”. Enabling this property will copy the query string from the incoming URL to the outgoing URL.</p>
      <uui-toggle
        label=""
        .checked=${this.preserve}
        @change=${this._onToggleChange}
      ></uui-toggle>
    `;
  }

  static styles = [
    css`
      :host {
        display: block;
      }

      uui-toggle {
        position: relative;
      }
    `,
  ];
}
