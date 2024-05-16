import { IEditorService, editorServiceContext } from "@/context/editorservice.context";
import { landingpageServiceContext } from "@/context/landingspageservice.context";
import { recommendationServiceContext } from "@/context/recommendationservice.context";
import { redirectServiceContext } from "@/context/redirectservice.context";
import { RECOMMENDATION_SORT_TYPE } from "@/enums/sortType";
import { ILandingspageService } from "@/services/landingspage.service";
import { IRecommendationCollection, IRecommendationResponse, IRecommendationsService } from "@/services/recommendation.service";
import { IRedirectResponse, IRedirectService, ISolvedRecommendationRequest } from "@/services/redirect.service";
import { ensureServiceExists } from "@/util/tools/existancecheck";
import variableresourceService from "@/util/tools/variableresource.service";
import { consume } from "@lit/context";
import { LitElement, PropertyValueMap, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { repeat } from "lit/directives/repeat.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";
import { IRecommendationAction, RECCOMENDATION_ACTIONS } from "../sidebars/explainRecommendations/explainRecommendations.lit";
import "./redirects/redirectitem.lit";
import { ISourceStrategies } from "./redirects/source/source.constants";
import { ITargetStrategies } from "./redirects/target/target.constants";
import { IUmbracoNotificationsService, umbracoNotificationsServiceContext } from "@/context/notificationsservice.context";

@customElement("urltracker-landing-tab")
export class UrlTrackerLandingTab extends UrlTrackerNotificationWrapper(
  LitElement,
  "landingpage"
) {
  @consume({ context: recommendationServiceContext })
  private _recommendationsService?: IRecommendationsService;

  @consume({ context: redirectServiceContext })
  private _redirectService?: IRedirectService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: landingpageServiceContext })
  private _landingspageService?: ILandingspageService;

  @consume({ context: umbracoNotificationsServiceContext })
  private _notificationsService?: IUmbracoNotificationsService | undefined;
  public get notificationsService(): IUmbracoNotificationsService {
    ensureServiceExists(this._notificationsService, 'notificationsService');
    return this._notificationsService;
  }
  public set notificationsService(value: IUmbracoNotificationsService | undefined) {
    this._notificationsService = value;
  }

  @state()
  private recommendationCollection?: IRecommendationCollection;

  @state()
  private numericMetric: number = 0;

  @state()
  private loading: number = 0;

  protected async firstUpdated(
    _changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>
  ): Promise<void> {
    super.firstUpdated(_changedProperties);

    await this.init();
  }

  private async init() {
    ensureServiceExists(
      this._recommendationsService,
      "recommendations service"
    );
    ensureServiceExists(this._redirectService, "redirect service");
    ensureServiceExists(this._recommendationsService, "recommendations service");
    ensureServiceExists(this._landingspageService, "landingspage service")
    ensureServiceExists(this.editorService, "editor service");

    await this.search();
  }

  private async search() {
    this.loading++;
    try {
      this.recommendationCollection = await this._recommendationsService!.list({
        page: 1,
        pageSize: 10,
        OrderBy: RECOMMENDATION_SORT_TYPE.IMPORTANCE
      });
      this.numericMetric = await this._landingspageService?.numericMetric() ?? 0;
    } finally {
      this.loading--;
    }
  }

  private handleCreatePermanentRedirect = async (
    event: CustomEvent<IRecommendationResponse>
  ) => {
    const redirect = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>(
          "redirectSourceStrategies"
        ).url,
        value: event.detail.url,
      },
      target: {
        strategy: variableresourceService.get<ITargetStrategies>(
          "redirectTargetStrategies"
        ).content,
        value: "",
      },
      permanent: true,
      retainQuery: true,
      force: false,
    } as IRedirectResponse;

    this.openNewRedirectPanel(redirect, event.detail.id);
  };

  private handleCreateTemporaryRedirect = async (
    event: CustomEvent<IRecommendationResponse>
  ) => {
    const redirect = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>(
          "redirectSourceStrategies"
        ).url,
        value: event.detail.url,
      },
      target: {
        strategy: variableresourceService.get<ITargetStrategies>(
          "redirectTargetStrategies"
        ).content,
        value: "",
      },
      permanent: false,
      retainQuery: true,
      force: false,
    } as IRedirectResponse;

    this.openNewRedirectPanel(redirect, event.detail.id);
  };

  private handleIgnore = async (event: CustomEvent<IRecommendationResponse>) => {
    await this._recommendationsService!.update({
      id: event.detail.id,
      recommendationStrategy: event.detail.strategy,
      ignore: true,
    });
    this.notificationsService.success("Recommendation ignored", "The recommendation has been removed from the overview");

    await this.search();
  };

  private openNewRedirectPanel(data?: IRedirectResponse, solvedRecommendation?: number) {
    const options = {
      title: "New redirect",
      view: "/App_Plugins/UrlTracker/sidebar/redirect/simpleRedirect.html",
      size: "medium",
      submit: (val: IRedirectResponse) => this.submitNewRedirectPanel({...val, solvedRecommendation}),
      close: this.closePanel,
      value: data,
    };

    this.editorService!.open(options);
  }

  submitNewRedirectPanel = async (value: IRedirectResponse & ISolvedRecommendationRequest) => {
    if (value.id) {
      await this._redirectService?.update(value);
      this.notificationsService.success("Redirect updated", "The redirect was successfully updated");
    } else {
      await this._redirectService?.create(value);
      this.notificationsService.success("Redirect created", "The redirect was successfully created");
    }

    this.closePanel();
    this.search();
  };

  private openExplanationPanel(data: IRecommendationResponse) {
    const options = {
      title: `Recommendations for: ${data.url}`,
      view: "/App_Plugins/UrlTracker/sidebar/recommendations/inspectRecommendations.html",
      size: "medium",
      submit: this.submitExplanationPanel,
      close: this.closePanel,
      value: data,
    };
    this.editorService!.open(options);
  }

  private submitExplanationPanel = (payload: {
    recommendation: IRecommendationResponse;
    action: IRecommendationAction;
  }) => {
    this.editorService!.close();
    switch (payload.action) {
      case RECCOMENDATION_ACTIONS.MAKE_PERMANENT:
        this.handleCreatePermanentRedirect(new CustomEvent("", { detail: payload.recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.MAKE_TEMPORARY:
        this.handleCreateTemporaryRedirect(new CustomEvent("", { detail: payload.recommendation }));
        break;
      case RECCOMENDATION_ACTIONS.IGNORE:
        this.handleIgnore(new CustomEvent("", { detail: payload.recommendation }));
        break;
    }
  };

  private openAnalysePanel(data: IRecommendationResponse) {
    const options = {
      title: `Recommendations for: ${data.url}`,
      view: "/App_Plugins/UrlTracker/sidebar/recommendations/analyseRecommendation.html",
      size: "medium",
      submit: this.closePanel,
      close: this.closePanel,
      value: data,
    };
    this.editorService!.open(options);
  }

  closePanel = () => {
    this.editorService!.close();
  };

  private onExplain = (e: CustomEvent<IRecommendationResponse>) => {
    this.openExplanationPanel(e.detail);
  };

  private onAnalyse = (e: CustomEvent<IRecommendationResponse>) => {
    this.openAnalysePanel(e.detail);
  };

  private renderRecommendations(): unknown {
    if (!this.recommendationCollection?.results) return nothing;
    return repeat(
      this.recommendationCollection.results,
      (recommendation) => recommendation.id,
      (r) =>
        html`<urltracker-recommendation-item
          .item=${r}
          .selectable=${false}
          @explain=${this.onExplain}
          @analyse=${this.onAnalyse}
          @createPermanent=${this.handleCreatePermanentRedirect}
          @createTemporary=${this.handleCreateTemporaryRedirect}
          @ignore=${this.handleIgnore}
        ></urltracker-recommendation-item>`
    );
  }

  protected renderInternal(): unknown {
    return html`
      <div class="grid-root">
        <div class="results">
          <urltracker-result-list
              .loading=${!!this.loading}
              header="Top 10 recommendations"
            >
            ${this.renderRecommendations()}
          </urltracker-result-list>
        </div>

        <uui-box>
          <div class="total">
            <span>${this.numericMetric}</span>
            <p>Pages were not found last week</p>
          </div>
        </uui-box>
      </div>
    `;
  }

  static styles = css`
    .grid-root {
      display: grid;
      grid-template-columns: 1fr 360px;
      gap: 1rem;
    }

    .results {
      grid-column: 1;
      display: flex;
      flex-direction: column;
      gap: 1rem;
      min-width: 0;
    }

    urltracker-result-list {
      flex: 1 1 32rem;
    }

    uui-box {
      height: fit-content;
    }

    uui-box .total {
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    uui-box span {
      color: var(--uui-palette-space-cadet);
      font-family: Lato, sans-serif;
      font-size: 40px;
      font-weight: 800;
      line-height: 45px;
    }

    uui-box p {
      font-family: Lato, sans-serif;
      font-size: 15px;
      font-weight: 400;
      line-height: 20px;
      margin: 0;
    }
  `;
}
