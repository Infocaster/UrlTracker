import { IRedirectData } from "@/services/redirect.service";
import { LitElement, css, html, nothing } from "lit";
import { customElement, property } from "lit/decorators.js";
import "./redirectForce.lit";
import "./redirectIncomingUrl.lit";
import "./redirectOutgoingUrl.lit";
import "./redirectPermanent.lit";
import "./redirectPreserveQuerystring.lit";
import { ITypeButton } from "./simpleRedirectTypeProvider";

@customElement("urltracker-create-simple-redirect")
export class UrlTrackerCreateSimpleRedirect extends LitElement {
  @property({ type: Object })
  public redirect!: IRedirectData

  @property({ type: Boolean })
  public advancedView = false;

  private onTogglePermanent = ({ detail }: { detail: boolean}) => {
    this.redirect.permanent = detail;
    this.updateRedirect();
  };

  private onTogglePreserveQuerystring = ({ detail }: { detail: boolean}) => {
    this.redirect.retainQuery = detail;
    this.updateRedirect();
  };

  private onToggleForce = ({ detail }: { detail: boolean}) => {
    this.redirect.force = detail;
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

  private onIncomingTypeChange = ({ detail }: { detail: ITypeButton}) => {
    this.redirect.source.strategy = detail.value;
    this.updateRedirect();
  }

  private onOutgoingTypeChange = ({ detail }: { detail: ITypeButton}) => {
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

  protected renderPreserveQuerystring(): unknown {
    if(!this.advancedView) {
      return nothing;
    }
    return html`
      <urltracker-redirect-preserve-querystring
        class="border-bottom"
        .preserve=${this.redirect.retainQuery}
        @toggle=${this.onTogglePreserveQuerystring}
      ></urltracker-redirect-preserve-querystring>
    `;
  }

  protected renderForce(): unknown {
    if(!this.advancedView) {
      return nothing;
    }
    return html`
      <urltracker-redirect-force
        .force=${this.redirect.force}
        @toggle=${this.onToggleForce}
      ></urltracker-redirect-force>
    `;
  }
  
  protected render(): unknown {
    return html`
      <urltracker-redirect-permanent
        class="border-bottom"
        .isPermanent=${this.redirect.permanent}
        @toggle=${this.onTogglePermanent}
      ></urltracker-redirect-permanent>

      <urltracker-redirect-incoming-url
        class="border-bottom"
        .advancedView=${this.advancedView}
        .incomingStrategy=${this.redirect.source.strategy}
        .incomingUrl=${this.redirect.source.value}
        @input=${this.onIncomingUrlInput}
        @typechange=${this.onIncomingTypeChange}
      ></urltracker-redirect-incoming-url>

      <urltracker-redirect-outgoing-url
        class="border-bottom"
        .outgoingStrategy=${this.redirect.target.strategy}
        .outgoingUrl=${this.redirect.target.value} 
        @input=${this.onOutgoingUrlInput} 
        @typechange=${this.onOutgoingTypeChange}>
      </urltracker-redirect-outgoing-url>
      
      ${this.renderPreserveQuerystring()}
      ${this.renderForce()}
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
