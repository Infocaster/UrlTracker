import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { IRecommendationCollection } from "../../../../services/recommendation.service";
import "./redirectPermanent.lit";
import "./redirectIncomingUrl.lit";
import "./redirectOutgoingUrl.lit";

@customElement("urltracker-create-simple-redirect")
export class UrlTrackerCreateSimpleRedirect extends LitElement {
  @state()
  private _collection?: IRecommendationCollection;

  private _onToggle = ({ detail }: any) => {
    console.log("on toggle");
    console.log(detail);
  };

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }

  protected render(): unknown {
    return html`
      <urltracker-redirect-permanent
        class="border-bottom"
        @toggle=${this._onToggle}
      ></urltracker-redirect-permanent>
      <urltracker-redirect-incoming-url
        class="border-bottom"
      ></urltracker-redirect-incoming-url>
      <urltracker-redirect-outgoing-url></urltracker-redirect-outgoing-url>
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
