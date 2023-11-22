import { css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { UrlTrackerSelectableResultListItem } from "../../../util/elements/selectableresultlistitem.lit";
import { ContextConsumer, consume } from "@lit/context";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../../context/localizationservice.context";
import {
  IRecommendationResponse,
  recommendationContext,
} from "../../../context/recommendationitem.context";
import recommendationTypeStrategyResolver from "./recommendationType/recommendation.strategy";
import {
  RECCOMENDATION_TYPES,
  RecommendationTypes,
  calculateRecommendationType,
  recommendationTagFactory,
} from "./recommendationTag/recommendationTag";
import "./recommendationTag/recommendationTag.lit";
import "./recommendationitemAction.lit";
import {
  IEditorService,
  editorServiceContext,
} from "@/context/editorservice.context";

const RecommendationListItem =
  UrlTrackerSelectableResultListItem<IRecommendationResponse>(
    recommendationContext
  );

@customElement("urltracker-recommendation-item")
export class UrlTrackerRecommendationItem extends RecommendationListItem {
  private recommendationTypeStrategy = recommendationTypeStrategyResolver;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @state()
  private recommendationTagText = "";

  @state()
  private actionsText = "";

  @state()
  private recommendationType: RecommendationTypes =
    RECCOMENDATION_TYPES.NOT_IMPORTANT;

  //   @consume({ context: localizationServiceContext })
  //   private localizationService?: ILocalizationService;
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
    if (this.item) {
      this.recommendationType = calculateRecommendationType(this.item.score);
      this.tagText(this.recommendationType);
    }
    this.localizeActionsText();
  }

  private renderRecommendationType(): unknown {
    if (!this.item) return nothing;
    return this.recommendationTypeStrategy.getStrategy(this.item).getTemplate();
  }

  private renderTarget(): unknown {
    if (!this.item) return nothing;
    return this.item.url;
  }

  private async localizeActionsText(): Promise<void> {
    const actionsText = await this.localizationService?.localize(
      "urlTrackerRecommendationItem_actions"
    );
    this.actionsText = actionsText ?? "";
  }

  private async tagText(importance: RecommendationTypes): Promise<void> {
    const text = await this.localizationService?.localize(
      `urlTrackerRecommendationImportance_${importance}`
    );
    this.recommendationTagText = text ?? "";
  }

  private renderTag(text: string): unknown {
    if (!this.item) return nothing;
    return recommendationTagFactory(this.recommendationType, text ?? "");
  }

  private createTemporaryRedirect(): void {
    console.log("createTemporaryRedirect");
  }

  private createPermanentRedirect(): void {
    console.log("createPermanentRedirect");
  }

  private ignoreRecommendation(): void {
    console.log("ignoreRecommendation");
  }

  private _openSidebar(_: Event) {
    const options = {
      title: "New redirect", // FIXME: translate
      view: "/App_Plugins/UrlTracker/sidebar/recommendation/recommendations.html",
      size: "small",
      submit: this.submitPanel,
      close: this.closePanel,
      value: "",
    };

    this.editorService!.open(options);
  }

  submitPanel = (value: string) => {
    this.model = value;
    this.closePanel();
  };

  closePanel = () => {
    this.editorService!.close();
  };

  private onClick = (_: Event) => {
    ensureServiceExists(this.editorService, "editor service");

    const onClose = async () => {
      this.editorService!.close();
      await this.init();
      this.dispatchEvent(
        new ContentUpdateEvent(this.contentId!, this.contentItem!)
      );
    };

    this.editorService.contentEditor({
      id: this.contentId!,
      create: false,
      submit: onClose,
      close: onClose,
      documentTypeAlias: "",
      allowPublishAndClose: false,
      allowSaveAndClose: false,
      parentId: "",
    });
  };

  protected renderBody(): unknown {
    return html`
      <div class="body">
        <div class="type">
          ${this.renderRecommendationType()}
          ${this.renderTag(this.recommendationTagText)}
        </div>
        <div class="target">${this.renderTarget()}</div>
        <div class="actions">
          <span class="actions__label">${this.actionsText}</span>
          <uui-icon
            class="actions__help"
            label="Extra information"
            name="icon-help-alt"
            @click=${this._openSidebar}
          ></uui-icon>

          <urltracker-recommendation-item-action
            actionTextKey="temporary"
            .action="${this.createTemporaryRedirect}"
          ></urltracker-recommendation-item-action>
          <urltracker-recommendation-item-action
            actionTextKey="permanent"
            .action="${this.createPermanentRedirect}"
          ></urltracker-recommendation-item-action>
          <urltracker-recommendation-item-action
            actionTextKey="ignore"
            .action="${this.ignoreRecommendation}"
          ></urltracker-recommendation-item-action>
        </div>
      </div>
    `;
  }

  static styles = [
    ...RecommendationListItem.styles,
    css`
      .body {
        margin-left: 16px;
        width: 100%;
      }

      .type {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }

      .actions {
        display: flex;
        align-items: center;
      }

      .actions__label {
        margin-right: 4px;
      }

      .actions__help {
        height: 1em;
        width: 1em;
        margin-right: 12px;
      }

      .actions__help:hover {
        cursor: pointer;
      }

      urltracker-recommendation-item-action {
        margin-right: 1rem;
      }

      .target {
        color: var(--uui-palette-chamoisee-dimmed);
        line-height: 15px;
        font-size: 12px;
        margin-bottom: 0.5rem;
      }
    `,
  ];
}
