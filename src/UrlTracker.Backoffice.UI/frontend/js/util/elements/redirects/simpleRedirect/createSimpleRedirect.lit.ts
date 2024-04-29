import { IRedirectResponse } from "@/services/redirect.service";
import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import "./redirectIncomingUrl.lit";
import "./redirectOutgoingUrl.lit";
import "./redirectPermanent.lit";
import { ITypeButton } from "./simpleRedirectTypeProvider";

@customElement("urltracker-create-simple-redirect")
export class UrlTrackerCreateSimpleRedirect extends LitElement {
  @property({ type: Object })
  public redirect!: IRedirectResponse

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this.redirect = this.redirect;
  }

  private onToggle = ({ detail }: { detail: boolean}) => {
    this.redirect.permanent = detail;
    this.updateRedirect();
  };

  private onIncomingUrlInput = ({ detail }: { detail: string}) => {
    this.redirect.source.value = detail;
    this.updateRedirect();
  }

  private onOutgoingUrlInput = ({ detail }: { detail: string}) => {
    this.redirect.target.value = detail;
    this.updateRedirect();
  }

  private onTypeChange = ({ detail }: { detail: ITypeButton}) => {
    this.redirect.target.strategy = detail.value;
    this.updateRedirect();
  }

  private updateRedirect = () => this.dispatchEvent(
    new CustomEvent("update", {
      detail: this.redirect,
      bubbles: true,
      composed: true,
    })
  );
  
  protected render(): unknown {
    return html`
      <urltracker-redirect-permanent
        class="border-bottom"
        .isPermanent=${this.redirect.permanent}
        @toggle=${this.onToggle}
      ></urltracker-redirect-permanent>
      <urltracker-redirect-incoming-url
        class="border-bottom"
        .incomingStrategy=${this.redirect.source.strategy}
        .incomingUrl=${this.redirect.source.value}
        @input=${this.onIncomingUrlInput}
      ></urltracker-redirect-incoming-url>
      <urltracker-redirect-outgoing-url
        .outgoingStrategy=${this.redirect.target.strategy}
        .outgoingUrl=${this.redirect.target.value} 
        @input=${this.onOutgoingUrlInput} 
        @typechange=${this.onTypeChange}>
      </urltracker-redirect-outgoing-url>
    `;
  }

  static styles = [
    css`
      :host {
        box-sizing: border-box;
        display: block;
        background-color: white;
        width: 100%;
        padding: 16px 20px;
        box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
      }

      .border-bottom {
        padding-bottom: 1.2rem;
        border-bottom: 1px solid #e9e9eb;
      }
    `,
  ];
}
