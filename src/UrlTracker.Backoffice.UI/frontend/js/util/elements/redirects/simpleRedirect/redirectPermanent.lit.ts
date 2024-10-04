import { consume } from '@lit/context';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { localizationServiceContext } from '../../../../context/localizationservice.context';
import { ILocalizationService } from '../../../../umbraco/localization.service';
import { ensureServiceExists } from '@/util/tools/existancecheck';

@customElement('urltracker-redirect-permanent')
export class UrlTrackerRedirectPermanent extends LitElement {
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

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;
  private get localizationService(): ILocalizationService {
    ensureServiceExists(this._localizationService, 'Localization service');
    return this._localizationService;
  }

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    await this._localizeLabels();
  }

  private _localizeLabels = async () => {
    const [permanent, temporary, title, description] = await this.localizationService.localizeMany([
      'urlTrackerNewRedirect_permanent-permanent-label',
      'urlTrackerNewRedirect_permanent-temporary-label',
      'urlTrackerNewRedirect_permanent',
      'urlTrackerNewRedirect_permanent-info',
    ]);

    this._permanentLabel = permanent ?? 'Permanent (301)';
    this._temporaryLabel = temporary ?? 'Temporary (302)';
    this._headerText = title ?? 'Permanent';
    this._infoText =
      description ??
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
