import { css, html } from 'lit';
import { customElement } from 'lit/decorators.js';
import { IRecommendationResponse, recommendationContext } from '../../../context/recommendationitem.context';
import { UrlTrackerSelectableResultListItem } from '../../../util/elements/selectableresultlistitem.lit';
import { actionButton, cardWithClickableHeader, errorStyle } from '../styles';
import './recommendationTag/recommendationTag.lit';

const RecommendationListItem = UrlTrackerSelectableResultListItem<IRecommendationResponse>(recommendationContext);

@customElement('urltracker-recommendation-item-skeleton')
export class UrlTrackerRecommendationItemSkeleton extends RecommendationListItem {
  protected renderBody(): unknown {
    return html`
      <div class="body loading">
        <div class="type">
          <h3>
            <div class="skeleton skeleton-title"></div>
          </h3>
          <div class="skeleton skeleton-tag"></div>
        </div>
        <div class="target">
          <div class="skeleton skeleton-target"></div>
        </div>
        <div class="actions">
          <div class="skeleton skeleton-button"></div>
          <div class="skeleton skeleton-button"></div>
          <div class="skeleton skeleton-button"></div>
          <div class="skeleton skeleton-button"></div>
        </div>
        <div class="dates">
          <div class="skeleton skeleton-dates"></div>
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

      /* Skeleton loading styles */
      .skeleton {
        background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
        background-size: 200% 100%;
        animation: skeleton-loading 1.5s infinite;
        border-radius: 4px;
        display: inline-block;
      }

      @keyframes skeleton-loading {
        0% {
          background-position: 200% 0;
        }
        100% {
          background-position: -200% 0;
        }
      }

      .skeleton-title {
        height: 20px;
        width: 65%;
        margin-bottom: 4px;
      }

      .skeleton-tag {
        height: 24px;
        width: 80px;
        border-radius: 12px;
      }

      .skeleton-target {
        height: 15px;
        width: 85%;
        margin-bottom: 0.5rem;
      }

      .skeleton-button {
        height: 24px;
        width: 70px;
        margin-right: 8px;
      }

      .skeleton-dates {
        height: 15px;
        width: 50%;
      }

      .loading .type {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }

      .loading .actions {
        display: flex;
        align-items: center;
        margin-top: 0.5rem;
      }
    `,
  ];
}
