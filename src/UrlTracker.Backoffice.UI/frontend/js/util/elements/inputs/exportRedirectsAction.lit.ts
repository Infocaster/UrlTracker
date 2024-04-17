import { css, html } from "lit";
import { customElement } from "lit/decorators.js";
import "@umbraco-ui/uui";
import { UrlTrackerRedirectAction } from "./redirectActionBase.lit";

@customElement("urltracker-export-redirects-action")
export class UrlTrackerExportRedirectsAction extends UrlTrackerRedirectAction {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeActionText();
  }

  private _localizeActionText = async () => {
    let translatedText = await this._localizationService?.localize(
      "urlTrackerRedirectActions_export"
    );

    this._actionText = `${translatedText}`;
  };

  protected override render(): unknown {
    return html` <div @click=${this._onAddClick}>
      <uui-icon-registry-essential>
        <uui-icon name="download"></uui-icon>
      </uui-icon-registry-essential>
      ${this._actionText}
    </div>`;
  }

  static styles = css`
    div:hover {
      cursor: pointer;
    }
  `;
}
