import { LitElement, css, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./redirectIncomingUrl.lit";
import "./redirectOutgoingUrl.lit";
import "./redirectPermanent.lit";
import { ITypeButton } from "./simpleRedirectTypeProvider";

@customElement("urltracker-create-simple-redirect")
export class UrlTrackerCreateSimpleRedirect extends LitElement {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }

  private onToggle = ({ detail }: { detail: boolean}) => {
    console.log("on toggle");
    console.log(detail);
  };

  private onIncomingUrlInput = ({ detail }: { detail: string}) => {
    console.log("on input");
    console.log(detail);
  }

  private onOutgoingUrlInput = ({ detail }: { detail: string}) => {
    console.log("on input");
    console.log(detail);
  }

  private onTypeChange = ({ detail }: { detail: ITypeButton}) => {
    console.log("on type change");
    console.log(detail);
  }

  protected render(): unknown {
    return html`
      <urltracker-redirect-permanent
        class="border-bottom"
        @toggle=${this.onToggle}
      ></urltracker-redirect-permanent>
      <urltracker-redirect-incoming-url
        class="border-bottom"
        @input=${this.onIncomingUrlInput}
      ></urltracker-redirect-incoming-url>
      <urltracker-redirect-outgoing-url @input=${this.onOutgoingUrlInput} @typechange=${this.onTypeChange}>
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
