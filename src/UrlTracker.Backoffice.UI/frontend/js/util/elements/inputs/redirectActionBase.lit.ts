import { consume } from "@lit/context";
import "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { localizationServiceContext } from "../../../context/localizationservice.context";
import { ILocalizationService } from "../../../umbraco/localization.service";

@customElement("urltracker-redirect-action")
export class UrlTrackerRedirectAction extends LitElement {
  @consume({ context: localizationServiceContext })
  protected _localizationService?: ILocalizationService;

  @state()
  protected _actionText = "";

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }

  protected _onAddClick = (e: Event) => {
    e.stopImmediatePropagation();
    this.dispatchEvent(
      new Event("click", {
        bubbles: true,
        composed: true,
      })
    );
  };

  protected render(): unknown {
    return html` <div @click=${this._onAddClick}>
      <slot></slot>
    </div>`;
  }

  static styles = css``;
}
