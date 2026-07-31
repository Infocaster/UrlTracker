import { toReadableDate } from '@/util/functions/dateformatter';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import type { RedirectResponse } from '../../../api-client/types.gen';
import { UrlTrackerSelectableResultListItem } from '../../../util/elements/selectableresultlistitem.lit';
import { actionButton, cardWithClickableHeader, errorStyle } from '../styles';
import sourceStrategyResolver from './source/source.strategy';
import targetStrategyResolver from './target/target.strategy';

const RedirectListItem = UrlTrackerSelectableResultListItem<RedirectResponse>();

@customElement('urltracker-redirect-item')
export class UrlTrackerRedirectItem extends UmbElementMixin(RedirectListItem) {
  @property({ type: Boolean, reflect: true })
  public deleteRedirectLoading: boolean = false;

  @state()
  private redirectToText?: string;

  @state()
  private createDateText?: string;

  @state()
  private redirectSourceText?: string;

  @state()
  private sourceIsError: boolean = false;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this.redirectToText = this.localize.term('urlTrackerRedirectTarget_redirectto');
    this.createDateText = this.localize.term('urlTrackerRedirectTarget_redirectdate');

    const sourceStrategy = sourceStrategyResolver.getStrategy({ redirect: this.item, element: this });
    if (sourceStrategy) {
      this.redirectSourceText = await sourceStrategy.getTitle();
      this.sourceIsError = false;
    } else {
      this.redirectSourceText = this.localize.term('urlTrackerRedirectSource_unknown');
      this.sourceIsError = true;
    }
  }

  private handleEdit(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent('edit', { detail: this.item }));
  }

  private handleDelete(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent('delete', { detail: this.item }));
  }

  private renderSource(): unknown {
    if (!this.redirectSourceText) return nothing;
    let errorClass: string | undefined;

    if (this.sourceIsError) {
      errorClass = 'error';
    }

    return html`
      <h3 class="${ifDefined(errorClass)}">
        <button class="inspect-button" @click=${this.handleEdit}>${this.redirectSourceText}</button>
      </h3>
    `;
  }

  private renderTarget(): unknown {
    if (!this.item) return nothing;
    return targetStrategyResolver.getStrategy(this.item as any).getTemplate();
  }

  private renderDelete(): unknown {
    if (this.deleteRedirectLoading) {
      return html`<button class="action-button" disabled>
        <uui-loader-circle id="loader"></uui-loader-circle>
        <umb-localize key="urlTrackerGeneral_delete"></umb-localize>
      </button>`;
    } else {
      return html`<button class="action-button" @click=${this.handleDelete}>
        <uui-icon name="delete" class="icon-before"></uui-icon
        ><umb-localize key="urlTrackerGeneral_delete">Delete</umb-localize>
      </button>`;
    }
  }

  protected renderBody(): unknown {
    return html`
      <div class="body">
        ${this.renderSource()}
        <div class="target">${this.redirectToText}: ${this.renderTarget()}</div>
        <uui-button-group class="actions">
          <button class="action-button" @click=${this.handleEdit}>
            <uui-icon name="edit" class="icon-before"></uui-icon>
            <umb-localize key="urlTrackerGeneral_edit">Edit</umb-localize>
          </button>
          ${this.renderDelete()}
        </uui-button-group>
        <div class="createdate">${this.createDateText}: ${toReadableDate(new Date(this.item.createDate))}</div>
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
    `,
  ];
}
