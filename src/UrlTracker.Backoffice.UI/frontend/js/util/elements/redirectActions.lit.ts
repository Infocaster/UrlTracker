import { LitElement, css, html } from 'lit';
import { ILocalizationService } from '../../umbraco/localization.service';
import { consume } from '@lit/context';
import { localizationServiceContext } from '../../context/localizationservice.context';
import { customElement, property, state } from 'lit/decorators.js';
import '@umbraco-ui/uui';

@customElement('urltracker-redirect-actions')
export class UrlTrackerRedirectActions extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @property({ type: Boolean })
  public loading: boolean = false;

  @property({ type: String })
  public header?: string;

  @state()
  private _headerText: string = '';

  private _loadingText?: string;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeLoadingText();
  }

  private _localizeHeaderText = async () => {
    const translatedText = await this._localizationService?.localize('urlTrackerRedirectActions_header');

    // If custom property provided
    // Else if use translated header
    // Else fallback
    if (this.header) {
      this._headerText = `${this.header}`;
    } else if (translatedText) {
      this._headerText = `${translatedText}`;
    } else {
      this._headerText = '';
    }
  };

  private _localizeLoadingText = async () => {
    this._loadingText = await this._localizationService?.localize('urltrackergeneral_loading');
  };

  private renderBody(): unknown {
    if (this.loading) {
      return html`<uui-loader-bar animationDuration="1.5"></uui-loader-bar>`;
    }

    return html`<div class="action-container"><slot></slot></div>`;
  }

  protected render(): unknown {
    return html`
      <header>${this.loading ? this._loadingText : this._headerText}</header>
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

    .action-container {
      background-color: white;
      display: block;
      padding-bottom: var(--uui-border-radius);
      padding-top: var(--uui-border-radius);
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }

    ::slotted(:last-child),
    .action-container {
      border-bottom-left-radius: var(--uui-border-radius);
      border-bottom-right-radius: var(--uui-border-radius);
    }
  `;
}
