import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

@customElement('urltracker-redirect-permanent')
export class UrlTrackerRedirectPermanent extends UmbElementMixin(LitElement) {
  @property({ type: Boolean })
  public isPermanent: boolean = false;

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  @state()
  private _permanentLabel: string = '';

  @state()
  private _temporaryLabel: string = '';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    await this._localizeLabels();
  }

  private _localizeLabels = async () => {
    this._permanentLabel = this.localize.term('urlTrackerNewRedirect_permanent-permanent-label') ?? 'Permanent (301)';
    this._temporaryLabel = this.localize.term('urlTrackerNewRedirect_permanent-temporary-label') ?? 'Temporary (302)';
    this._headerText = this.localize.term('urlTrackerNewRedirect_permanent') ?? 'Permanent';
    this._infoText =
      this.localize.term('urlTrackerNewRedirect_permanent-info') ??
      'Select whether or not the redirect is permanent. Permanent redirects cannot be changed afterwards.';
  };

  private _onToggleChange = (_: any) => {
    this.isPermanent = !this.isPermanent;

    this.dispatchEvent(
      new CustomEvent('toggle', {
        detail: this.isPermanent,
        bubbles: true,
        composed: true,
      }),
    );
  };

  protected render(): unknown {
    return html`
      <p><strong>${this._headerText}</strong></p>
      <p>${this._infoText}</p>
      <uui-toggle
        label="${this.isPermanent ? this._permanentLabel : this._temporaryLabel}"
        .checked=${this.isPermanent}
        @change=${this._onToggleChange}
      ></uui-toggle>
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
