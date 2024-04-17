import { css, html } from "lit";
import { customElement } from "lit/decorators.js";
import "@umbraco-ui/uui";
import { UrlTrackerRedirectAction } from "./redirectActionBase.lit";

@customElement("urltracker-add-redirect-action")
export class UrlTrackerAddRedirectActions extends UrlTrackerRedirectAction {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeActionText();
  }

  private _localizeActionText = async () => {
    let translatedText = await this._localizationService?.localize(
      "urlTrackerRedirectActions_new"
    );

    this._actionText = `${translatedText}`;
  };

  protected override render(): unknown {
    return html` <div @click=${this._onAddClick}>
      <uui-icon-registry-essential>
        <uui-icon name="add"></uui-icon>
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
