import { css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { customElement, state } from 'lit/decorators.js';
import { recommendationContext } from '../../../context/recommendationitem.context';
import { UrlTrackerSelectableResultListItem } from '../../../util/elements/selectableresultlistitem.lit';
import {
  RECCOMENDATION_TYPES,
  RecommendationTypes,
  calculateRecommendationType,
  recommendationTagFactory,
} from './recommendationTag/recommendationTag';
import './recommendationTag/recommendationTag.lit';
import recommendationTypeStrategyResolver from './recommendationType/recommendation.strategy';
import { ifDefined } from 'lit/directives/if-defined.js';
import { actionButton, cardWithClickableHeader, errorStyle } from '../styles';
import { ProcessedRecommendationResponse } from '@/services/scoring/scoring.service';

const RecommendationListItem =
  UrlTrackerSelectableResultListItem<ProcessedRecommendationResponse>(recommendationContext);

@customElement('urltracker-recommendation-item')
export class UrlTrackerRecommendationItem extends RecommendationListItem {
  @state()
  private recommendationTagText = '';

  @state()
  private actionsText = '';

  @state()
  private recommendationType: RecommendationTypes = RECCOMENDATION_TYPES.NOT_IMPORTANT;

  @state()
  private recommendationTypeText?: string;

  @state()
  private recommendationTypeIsError: boolean = false;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (this.item) {
      this.recommendationType = calculateRecommendationType(this.item.score);
      this.tagText(this.recommendationType);
    }
    this.localizeActionsText();

    const sourceStrategy = recommendationTypeStrategyResolver.getStrategy({ recommendation: this.item, element: this });
    if (sourceStrategy) {
      this.recommendationTypeText = await sourceStrategy.getTitle();
      this.recommendationTypeIsError = false;
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

  protected renderBody(): unknown {
    return html`
      <div class="body">
        <div class="type">${this.renderRecommendationType()} ${this.renderTag(this.recommendationTagText)}</div>
        <div class="target">${this.renderTarget()}</div>
        <div class="actions">
          <button class="action-button help-button" @click=${this.handleExplain}>
            ${this.actionsText}<uui-icon name="icon-help-alt" class="icon-after"></uui-icon>
          </button>
          <button class="action-button" @click=${this.handleCreateTemporaryRedirect}>
            <uui-icon name="icon-navigation-right" class="icon-before"></uui-icon>Create temporary redirect
          </button>
          <button class="action-button" @click=${this.handleCreatePermanentRedirect}>
            <uui-icon name="icon-navigation-right" class="icon-before"></uui-icon>Create permanent redirect
          </button>
          <button class="action-button" @click=${this.handleIgnoreRecommendation}>
            <uui-icon name="icon-navigation-right" class="icon-before"></uui-icon>Ignore this
          </button>
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

      .target {
        color: var(--uui-palette-chamoisee-dimmed);
        line-height: 15px;
        font-size: 12px;
        margin-bottom: 0.5rem;
      }
    `,
  ];
}
