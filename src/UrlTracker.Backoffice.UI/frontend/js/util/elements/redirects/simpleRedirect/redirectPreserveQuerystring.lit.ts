import { consume } from '@lit/context';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

@customElement('urltracker-redirect-preserve-querystring')
export class UrlTrackerRedirectPreserveQuerystring extends UmbElementMixin(LitElement) {
  @property({ type: Boolean })
  public preserve: boolean = false;

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeInfoText();
  }

  private _localizeHeaderText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_permanent');

    this._headerText = text ?? 'fallback';
  };

  private _localizeInfoText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_permanent-info');

    this._infoText = text ?? 'fallback';
  };

  private _onToggleChange = (_: any) => {
    this.preserve = !this.preserve;

    this.dispatchEvent(
      new CustomEvent('toggle', {
        detail: this.preserve,
        bubbles: true,
        composed: true,
      }),
    );
  };

  protected render(): unknown {
    return html`
      <p><strong>Preserve query string</strong></p>
      <p>
        The query string is the part behind the ? in a URL and consists of so-called “key/value pairs”. Enabling this
        property will copy the query string from the incoming URL to the outgoing URL.
      </p>
      <uui-toggle label="" .checked=${this.preserve} @change=${this._onToggleChange}></uui-toggle>
    `;
  }

  static styles = [
    css`
      :host {
        display: block;
      }

      uui-toggle {
        position: relative;
      }
    `,
  ];
}
