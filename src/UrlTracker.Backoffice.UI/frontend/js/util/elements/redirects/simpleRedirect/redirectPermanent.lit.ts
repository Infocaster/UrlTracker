import { consume } from '@lit/context';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { localizationServiceContext } from '../../../../context/localizationservice.context';
import { ILocalizationService } from '../../../../umbraco/localization.service';

@customElement('urltracker-redirect-permanent')
export class UrlTrackerRedirectPermanent extends LitElement {
  @property({ type: Boolean })
  public isPermanent: boolean = false;

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeInfoText();
  }

  private _localizeHeaderText = async () => {
    const text = await this._localizationService?.localize('urlTrackerNewRedirect_permanent');

    this._headerText = text ?? 'fallback';
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize('urlTrackerNewRedirect_permanent-info');

    this._infoText = text ?? 'fallback';
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
      <uui-toggle label="" .checked=${this.isPermanent} @change=${this._onToggleChange}></uui-toggle>
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
