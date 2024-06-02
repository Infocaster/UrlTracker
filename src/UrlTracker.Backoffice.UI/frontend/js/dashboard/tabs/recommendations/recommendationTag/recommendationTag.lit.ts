import { LitElement, css, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { consume } from '@lit/context';
import { ILocalizationService, localizationServiceContext } from '../../../../context/localizationservice.context';
import { styleMap } from 'lit/directives/style-map.js';

@customElement('urltracker-recommendation-tag')
export class UrlTrackerRecommendationTag extends LitElement {
  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @property({ type: String })
  color: string = '';

  @property({ type: String })
  text: string = 'default';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (!this.localizationService) throw new Error('This element requires the localization service');
  }

  protected render(): unknown {
    const dotStyle = {
      color: this.color,
    };
    return html`<uui-tag color="default" look="secondary"
      ><span class="dot" style=${styleMap(dotStyle)}>•</span> ${this.text}</uui-tag
    >`;
  }

  static styles = [
    css`
      .body {
        margin-left: 16px;
      }

      .target {
        line-height: 15px;
        font-size: 12px;
        margin-top: 8px;
      }

      .dot {
        font-size: 15px;
        line-height: 20px;
        margin-right: 4px;
      }
    `,
  ];
}
