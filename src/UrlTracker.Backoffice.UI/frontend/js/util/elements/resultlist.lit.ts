import { LitElement, css, html } from '@umbraco-cms/backoffice/external/lit';
import { customElement, property } from 'lit/decorators.js';
import '@umbraco-cms/backoffice/external/uui';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

@customElement('urltracker-result-list')
export default class UrlTrackerResultList extends UmbElementMixin(LitElement) {
  @property({ type: Boolean })
  public loading: boolean = false;

  @property({ type: String })
  public header?: string;

  private _loadingText?: string;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._loadingText = this.localize.term('urltrackergeneral_loading');
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

declare global {
  interface HTMLElementTagNameMap {
    'urltracker-result-list': UrlTrackerResultList;
  }
}
