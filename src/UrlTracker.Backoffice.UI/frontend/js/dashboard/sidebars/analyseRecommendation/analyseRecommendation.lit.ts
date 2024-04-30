import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { IRecommendationsAnalysisService, recommendationsAnalysisServiceContext } from "@/context/recommendationsanalysis.context";
import { scopeContext } from "@/context/scope.context";
import { IScope } from "@/models/scope.model";
import { IRecommendationResponse } from "@/services/recommendation.service";
import { ensureExists, ensureServiceExists } from "@/util/tools/existancecheck";
import { consume } from "@lit/context";
import { LitElement, css, html, nothing } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import recommendationTypeStrategyResolver from "../../tabs/recommendations/recommendationType/recommendation.strategy";

export const ContentElementTag = "urltracker-sidebar-analyse-recommendation";

@customElement(ContentElementTag)
export class UrlTrackerSidebarAnalyseRecommendation extends LitElement {
  private recommendationTypeStrategy = recommendationTypeStrategyResolver;

  @consume({ context: recommendationsAnalysisServiceContext })
  private recommendationsAnalysisService?: IRecommendationsAnalysisService;

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @consume({ context: scopeContext })
  private $scope?: IScope;

  @property({ attribute: false})
  get scope() { 
    ensureExists(this.$scope, "scope");
    return this.$scope;
  }

  @state()
  private data!: IRecommendationResponse;

  @state()
  private _subText = "";

  private renderRecommendationType(): unknown {
    if (!this.data) return nothing;
    return this.recommendationTypeStrategy.getStrategy(this.data).getTemplate();
  }

  async connectedCallback(): Promise<void> {
      super.connectedCallback();

      ensureServiceExists(this.recommendationsAnalysisService, "recommendationsAnalysisService");
      ensureServiceExists(this.localizationService, "localizationService");

      this.data = this.scope.model.value;
      this._subText = this.scope.model.value.url ?? "";

      const referrersPromise = this.recommendationsAnalysisService.getReferrers(this.data);
      const historyPromise = this.recommendationsAnalysisService.getHistory(this.data);

      const [referrers, history] = await Promise.all([referrersPromise, historyPromise]);
      console.log(referrers, history);
  }

  close() {
    this.scope.model.close();
  }

  protected render() {
    return html`
      <div class="header">
        <h6>${this.renderRecommendationType()}</h6>
        <span>${this._subText}</span>
      </div>
      <div class="main">
        <uui-box>
          <p>
          This entry indicates that an image could not be found. As a consequence, 
          certain pages may not be displayed correctly and visitors might lack visual context to the content on particular pages. 
          Check out the referrer information to see on which pages the image is requested.
          Redirect this url to an existing image to restore the user experience. Alternatively, 
          you can check out the referrer overview below to see from which pages the image is requested.
          After manually repairing the images, you can mark this recommendation as resolved.
          </p>
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}
          >Close</uui-button
        >
      </div>
    `;
  }

  static styles = css`
  :host {
    display: flex;
    flex-direction: column;
    height: 100vh;
  }

  .header {
    padding: 10px 20px;
    background-color: white;
    box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);

    h6{
      font-size: 16px;
      font-weight: 700;
      line-height: 20px;
      text-align: left;
      margin: 0;
      display: block;
    }

    span {
      font-size: 12px;
      font-weight: 400;
      line-height: 15px;
      text-align: left;
      color: #68676B;
    }

  }

  .main {
    flex: 1;
    padding: 16px 20px;
  }

  uui-box {
    margin-bottom: 1rem;
    font-family: lato, sans-serif;
    font-weight: 400;
    font-size: 15px;
    line-height: 1.25;
  }

  ul, p {
    margin-top: 0;
  }

  uui-box uui-button {
    width: 100%;
  }

  .footer {
    display: flex;
    justify-content: flex-end;
    background-color: white;
    padding: 10px 20px;
    box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
  }
`;
}
