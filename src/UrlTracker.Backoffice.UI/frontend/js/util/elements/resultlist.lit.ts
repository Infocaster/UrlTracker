import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

@customElement('urltracker-result-list')
export class UrlTrackerResultList extends UmbElementMixin(LitElement) {
  @property({ type: Boolean })
  public loading: boolean = false;

  @property({ type: String })
  public header?: string;

  @state()
  private _loadingText = '';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this._loadingText = this.localize?.term('urltrackerGeneral_loading') || 'Loading...';
  }

  private renderBody(): unknown {
    if (this.loading) {
      return html`<uui-loader-bar animationDuration="1.5"></uui-loader-bar>`;
    }

    return html`<slot></slot>`;
  }

  protected render(): unknown {
    return html`
      <header>${this.loading ? this._loadingText : this.header}</header>
      ${this.renderBody()}
    `;
  }

  static styles = css`
    header {
      padding: 16px 20px;
      color: white;
      background-color: var(--uui-color-header-surface);
      font-weight: bolder;
      border-radius: var(--uui-border-radius) var(--uui-border-radius) 0 0;
    }

    ::slotted(:not(:first-child)) {
      margin-top: 8px;
    }

    ::slotted(:first-child) {
      border-top-left-radius: 0;
      border-top-right-radius: 0;
    }
  `;
}
