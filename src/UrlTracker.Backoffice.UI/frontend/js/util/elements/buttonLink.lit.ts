import "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("urltracker-button-link")
export class UrltrackerButtonLink extends LitElement {
  @property({ type: String })
  public text = "";

  @property({ type: String })
  public header?: string;
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }

  protected render(): unknown {
    return html`
        <button>
            <slot></slot>
            ${this.text}
        </button>
    `;
  }

  static styles = css`
    button {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 4px;
        background: none!important;
        border: none;
        padding: 0!important;
        font-family: arial, sans-serif;
        color: #000;
        text-decoration: underline;
        cursor: pointer;

        font-size: 12px;
        line-height: 15px;
        font-weight: 400;
        font-family: lato, sans-serif;

        &:hover {
            color: var(--uui-color-default-emphasis);
        }
    }
  `;
}
