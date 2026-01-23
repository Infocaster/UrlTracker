import { css, html } from 'lit';
import { customElement } from 'lit/decorators.js';
import type { RedirectResponse } from '../../../../../api-client/types.gen';
import { UrlTrackerSelectableResultListItem } from '../../../util/elements/selectableresultlistitem.lit';
import { actionButton, cardWithClickableHeader, errorStyle } from '../styles';

const RedirectListItem = UrlTrackerSelectableResultListItem<RedirectResponse>();

@customElement('urltracker-redirect-item-skeleton')
export class UrlTrackerRedirectItemSkeleton extends RedirectListItem {
  protected renderBody(): unknown {
    return html`
      <div class="body loading">
        <h3>
          <div class="skeleton skeleton-title"></div>
        </h3>
        <div class="target">
          <div class="skeleton skeleton-target"></div>
        </div>
        <uui-button-group class="actions">
          <div class="skeleton skeleton-button"></div>
          <div class="skeleton skeleton-button"></div>
        </uui-button-group>
        <div class="createdate">
          <div class="skeleton skeleton-date"></div>
        </div>
      </div>
    `;
  }

  static styles = [
    ...RedirectListItem.styles,
    errorStyle,
    cardWithClickableHeader,
    actionButton,
    css`
      .body {
        min-width: 0;
        width: 100%;
      }

      .target,
      .createdate {
        line-height: 15px;
        font-size: 12px;
      }

      .target {
        margin-top: 8px;
      }

      .createdate {
        font-style: italic;
        color: var(--uui-palette-chamoisee-dimmed);
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
        width: 60%;
        margin-bottom: 4px;
      }

      .skeleton-target {
        height: 15px;
        width: 80%;
      }

      .skeleton-button {
        height: 24px;
        margin-right: 8px;
      }

      .skeleton-date {
        height: 15px;
        width: 40%;
      }

      .loading .actions {
        width: 144px;
        display: flex;
        align-items: center;
      }
    `,
  ];
}
