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
    this._headerText = this.localize.term('urlTrackerRedirectPreserveQuerystring_header') ?? 'fallback';
  };

  private _localizeInfoText = async () => {
    this._infoText = this.localize.term('urlTrackerRedirectPreserveQuerystring_info') ?? 'fallback';
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
      <p><strong>${this._headerText}</strong></p>
      <p>${this._infoText}</p>
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
