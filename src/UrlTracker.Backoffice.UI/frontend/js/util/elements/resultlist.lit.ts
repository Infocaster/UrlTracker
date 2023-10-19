import { LitElement, css, html } from "lit";
import { ILocalizationService } from "../../umbraco/localization.service";
import { consume } from "@lit/context";
import { localizationServiceContext } from "../../context/localizationservice.context";
import { customElement, property } from "lit/decorators.js";
import "@umbraco-ui/uui";

@customElement("urltracker-result-list")
export class UrlTrackerResultList extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @property({ type: Boolean })
  public loading: boolean = false;

  @property({ type: String })
  public header?: string;

  private _loadingText?: string;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._loadingText = await this._localizationService?.localize(
      "urltrackergeneral_loading"
    );
  }

  private renderBody(): unknown {
    if (this.loading) {
      return html`<uui-loader-bar animationDuration="1.5"></uui-loader-bar>`;
    }

    return html`<slot></slot>`;
  }

  protected render(): unknown {
    return html`
      <header>${this.loading ? this._loadingText : this.header}</header>
      ${this.renderBody()}
    `;
  }

  static styles = css`
    header {
      padding: 16px 20px;
      color: white;
      background-color: var(--uui-color-header-surface);
      font-weight: bolder;
      border-radius: var(--uui-border-radius) var(--uui-border-radius) 0 0;
    }

    ::slotted(:not(:first-child)) {
      margin-top: 8px;
    }

    ::slotted(:first-child) {
      border-top-left-radius: 0;
      border-top-right-radius: 0;
    }
  `;
}
