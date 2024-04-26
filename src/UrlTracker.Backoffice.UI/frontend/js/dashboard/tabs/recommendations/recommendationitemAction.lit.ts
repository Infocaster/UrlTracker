import { ContextConsumer } from "@lit/context";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../../context/localizationservice.context";

@customElement("urltracker-recommendation-item-action")
export class UrlTrackerRecommendationItemAction extends LitElement {
  @property({ type: String })
  public actionTextKey?: string;

  @property()
  public action?: (e: Event) => void;

  @state()
  private actionText = "";

  private _localizationServiceConsumer = new ContextConsumer(this, {
    context: localizationServiceContext,
  });

  protected get localizationService(): ILocalizationService | undefined {
    return this._localizationServiceConsumer.value;
  }

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (!this.localizationService) {
      throw new Error("This element requires the localization service");
    }

    const actionText = await this.localizationService?.localize(
      `urlTrackerRecommendationItem_action-${this.actionTextKey}`
    );
    this.actionText = actionText ?? "";
  }

  public render() {
    return html`
      <uui-icon name="icon-navigation-right"></uui-icon>
      <button class="" @click=${this.action}>${this.actionText}</button>
    `;
  }

  static styles = [
    css`
      :host {
        display: flex;
        align-items: center;
      }

      uui-icon {
        font-size: 10px;
        margin-right: 4px;
      }

      button {
        color: black;
        border: none;
        border-bottom: 1px solid rgba(0, 0, 0, 0.5);
        background: none;
        padding: 2px 0;
      }

      button:hover {
        cursor: pointer;
        border-bottom: 1px solid rgba(0, 0, 0, 1);
      }
    `,
  ];
}
