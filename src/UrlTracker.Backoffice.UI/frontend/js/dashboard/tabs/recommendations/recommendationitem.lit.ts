import { toReadableDateOnly } from '@/util/functions/dateformatter';
import { css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { RecommendationResponse } from '../../../api-client';
import { UrlTrackerSelectableResultListItem } from '../../../util/elements/selectableresultlistitem.lit';
import { actionButton, cardWithClickableHeader, errorStyle } from '../styles';
import {
  RECCOMENDATION_TYPES,
  RecommendationTypes,
  calculateRecommendationType,
  recommendationTagFactory,
} from './recommendationTag/recommendationTag';
import './recommendationTag/recommendationTag.lit';
import recommendationTypeStrategyResolver from './recommendationType/recommendation.strategy';

const RecommendationListItem = UrlTrackerSelectableResultListItem<RecommendationResponse>();

@customElement('urltracker-recommendation-item')
export class UrlTrackerRecommendationItem extends RecommendationListItem {
  @property({ type: Boolean, reflect: true })
  public ignoreRecommendationLoading: boolean = false;

  @state()
  private recommendationTagText = '';

  @state()
  private actionsText = '';

  @state()
  private actionTemporaryText = '';

  @state()
  private actionPermanentText = '';

  @state()
  private actionIgnoreText = '';

  @state()
  private recommendationType: RecommendationTypes = RECCOMENDATION_TYPES.NOT_IMPORTANT;

  @state()
  private recommendationTypeText?: string;

  @state()
  private recommendationTypeIsError: boolean = false;

  @state()
  private occurranceDatesText?: string;

  @state()
  private isImageRecommendation: boolean = false;

  @state()
  private isTechnicalFileRecommendation: boolean = false;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (this.item) {
      this.recommendationType = calculateRecommendationType(this.item.variableScore);
      this.tagText(this.recommendationType);
    }
    this.localizeActionsText();
    this.localizeDatesText();
    this.localizeActionTemporaryText();
    this.localizeActionPermanentText();
    this.localizeActionIgnoreText();

    const sourceStrategy = recommendationTypeStrategyResolver.getStrategy({ recommendation: this.item, element: this });

    if (sourceStrategy) {
      this.recommendationTypeText = this.localize.term(sourceStrategy.typeKey);
      this.recommendationTypeIsError = false;

      if (this.recommendationTypeText === this.localize.term('urlTrackerRecommendationType_image')) {
        this.isImageRecommendation = true;
      }

      if (this.recommendationTypeText === this.localize.term('urlTrackerRecommendationType_technicalFile')) {
        this.isTechnicalFileRecommendation = true;
      }
    } else {
      this.recommendationTypeText = this.localize.term('urlTrackerRecommendationType_unknown');
      this.recommendationTypeIsError = true;
    }
  }

  private renderRecommendationType(): unknown {
    if (!this.recommendationTypeText) return nothing;
    let errorClass: string | undefined;

    if (this.recommendationTypeIsError) {
      errorClass = 'error';
    }

    return html`
      <h3 class="${ifDefined(errorClass)}">
        <button class="inspect-button" @click=${this.handleAnalyse}>${this.recommendationTypeText}</button>
      </h3>
    `;
  }

  private renderTarget(): unknown {
    if (!this.item) return nothing;
    return this.item.url;
  }

  private async localizeActionsText(): Promise<void> {
    const actionsText = this.localize.term('urlTrackerRecommendationItem_actions');
    this.actionsText = actionsText ?? '';
  }

  private async localizeActionTemporaryText(): Promise<void> {
    const temporaryText = this.localize.term('urlTrackerRecommendationItem_action-temporary');
    this.actionTemporaryText = temporaryText ?? '';
  }

  private async localizeActionPermanentText(): Promise<void> {
    const permanentText = this.localize.term('urlTrackerRecommendationItem_action-permanent');
    this.actionPermanentText = permanentText ?? '';
  }

  private async localizeActionIgnoreText(): Promise<void> {
    const ignoreText = this.localize.term('urlTrackerRecommendationItem_action-ignore');
    this.actionIgnoreText = ignoreText ?? '';
  }

  private async localizeDatesText(): Promise<void> {
    const datesText = this.localize.term('urlTrackerRecommendationItem_dates');
    this.occurranceDatesText = datesText;
  }

  private async tagText(importance: RecommendationTypes): Promise<void> {
    const text = this.localize.term(`urlTrackerRecommendationImportance_${importance}`);
    this.recommendationTagText = text ?? '';
  }

  private renderTag(text: string): unknown {
    if (!this.item) return nothing;
    return recommendationTagFactory(this.recommendationType, text ?? '');
  }

  private handleExplain(e: Event): void {
    this.dispatchEvent(new CustomEvent('explain', { detail: this.item }));
    e.stopPropagation();
  }

  private handleAnalyse(e: Event): void {
    this.dispatchEvent(new CustomEvent('analyse', { detail: this.item }));
    e.stopPropagation();
  }

  private handleCreateTemporaryRedirect(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent('createTemporary', { detail: this.item }));
  }

  private handleCreatePermanentRedirect(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent('createPermanent', { detail: this.item }));
  }

  private handleIgnoreRecommendation(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent('ignore', { detail: this.item }));
  }

  private renderTemporaryRedirect(): unknown {
    if (this.isImageRecommendation || this.isTechnicalFileRecommendation) return nothing;

    return html`<button class="action-button" @click=${this.handleCreateTemporaryRedirect}>
      <uui-icon name="icon-navigation-right" class="icon-before"></uui-icon>${this.actionTemporaryText}
    </button>`;
  }

  private renderPermanentRedirect(): unknown {
    if (this.isImageRecommendation || this.isTechnicalFileRecommendation) return nothing;

    return html`<button class="action-button" @click=${this.handleCreatePermanentRedirect}>
      <uui-icon name="icon-navigation-right" class="icon-before"></uui-icon>${this.actionPermanentText}
    </button>`;
  }

  private renderIgnore(): unknown {
    if (this.ignoreRecommendationLoading) {
      return html`<uui-loader-circle id="loader"></uui-loader-circle>`;
    }
    return html`<button class="action-button" @click=${this.handleIgnoreRecommendation}>
      <uui-icon name="icon-navigation-right" class="icon-before"></uui-icon>${this.actionIgnoreText}
    </button>`;
  }

  protected renderBody(): unknown {
    return html`
      <div class="body">
        <div class="type">${this.renderRecommendationType()} ${this.renderTag(this.recommendationTagText)}</div>
        <div class="target">${this.renderTarget()}</div>
        <div class="actions">
          <button class="action-button help-button" @click=${this.handleExplain}>
            ${this.actionsText}<uui-icon name="icon-help-alt" class="icon-after"></uui-icon>
          </button>
          ${this.renderTemporaryRedirect()} ${this.renderPermanentRedirect()} ${this.renderIgnore()}
        </div>
        <div class="dates">
          ${this.occurranceDatesText}: ${toReadableDateOnly(new Date(this.item.createDate))} -
          ${toReadableDateOnly(new Date(this.item.updateDate))}
        </div>
      </div>
    `;
  }

  static styles = [
    ...RecommendationListItem.styles,
    errorStyle,
    cardWithClickableHeader,
    actionButton,
    css`
      .body {
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

      .action-button.help-button {
        text-decoration: none;
      }

      .action-button.help-button:hover {
        text-decoration: underline;
      }

      .target,
      .dates {
        color: var(--uui-palette-chamoisee-dimmed);
        line-height: 15px;
        font-size: 12px;
      }

      .target {
        margin-bottom: 0.5rem;
      }

      .dates {
        font-style: italic;
        margin-top: 1rem;
      }
    `,
  ];
}
