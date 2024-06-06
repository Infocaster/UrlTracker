import { consume } from '@lit/context';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

@customElement('urltracker-redirect-force')
export class UrlTrackerRedirectForce extends UmbElementMixin(LitElement) {
  @property({ type: Boolean })
  public force: boolean = false;

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
    const text = this.localize.term('urlTrackerNewRedirect_force');

    this._headerText = text ?? 'fallback';
  };

  private _localizeInfoText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_force-info');

    this._infoText = text ?? 'fallback';
  };

  private _onToggleChange = (_: any) => {
    this.force = !this.force;

    this.dispatchEvent(
      new CustomEvent('toggle', {
        detail: this.force,
        bubbles: true,
        composed: true,
      }),
    );
  };

  protected render(): unknown {
    return html`
      <p><strong>Force</strong></p>
      <p>
        Enabling this feature will apply this redirect on the incoming URL, even if content exists on the incoming URL.
      </p>
      <uui-toggle label="" .checked=${this.force} @change=${this._onToggleChange}></uui-toggle>
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
