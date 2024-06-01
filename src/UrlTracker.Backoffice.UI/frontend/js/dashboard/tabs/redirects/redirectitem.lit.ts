import { consume } from '@lit/context';
import { css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { ILocalizationService, localizationServiceContext } from '../../../context/localizationservice.context';
import { redirectContext } from '../../../context/redirectitem.context';
import { IRedirectResponse } from '../../../services/redirect.service';
import { UrlTrackerSelectableResultListItem } from '../../../util/elements/selectableresultlistitem.lit';
import sourceStrategyResolver from './source/source.strategy';
import targetStrategyResolver from './target/target.strategy';
import { ensureExists, ensureServiceExists } from '@/util/tools/existancecheck';
import { ifDefined } from 'lit/directives/if-defined.js';
import { actionButton, cardWithClickableHeader, errorStyle } from '../styles';

const RedirectListItem = UrlTrackerSelectableResultListItem<IRedirectResponse>(redirectContext);

@customElement('urltracker-redirect-item')
export class UrlTrackerRedirectItem extends RedirectListItem {
  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @state()
  private redirectToText?: string;

  @state()
  private redirectSourceText?: string;

  @state()
  private sourceIsError: boolean = false;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    ensureServiceExists(this.localizationService, 'localizationService');
    ensureExists(this.item, 'A redirect is required to use this element, but no redirect was provided');

    this.redirectToText = await this.localizationService.localize('urlTrackerRedirectTarget_redirectto');

    const sourceStrategy = sourceStrategyResolver.getStrategy({ redirect: this.item, element: this });
    if (sourceStrategy) {
      this.redirectSourceText = await sourceStrategy.getTitle();
      this.sourceIsError = false;
    } else {
      this.redirectSourceText = await this.localizationService.localize('urlTrackerRedirectSource_unknown');
      this.sourceIsError = true;
    }
  }

  private handleInspect(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent('inspect', { detail: this.item }));
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
        <button class="inspect-button" @click=${this.handleInspect}>${this.redirectSourceText}</button>
      </h3>
    `;
  }

  private renderTarget(): unknown {
    if (!this.item) return nothing;
    return targetStrategyResolver.getStrategy(this.item).getTemplate();
  }

  protected renderBody(): unknown {
    return html`
      <div class="body">
        ${this.renderSource()}
        <div class="target">${this.redirectToText}: ${this.renderTarget()}</div>
        <uui-button-group class="actions">
          <button class="action-button" @click=${this.handleEdit}>
            <uui-icon name="edit" class="icon-before"></uui-icon>Edit
          </button>
          <button class="action-button" @click=${this.handleDelete}>
            <uui-icon name="delete" class="icon-before"></uui-icon>Delete
          </button>
        </uui-button-group>
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

      .target {
        line-height: 15px;
        font-size: 12px;
        margin-top: 8px;
      }
    `,
  ];
}
