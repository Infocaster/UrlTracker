import { LitElement, css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { customElement, state } from 'lit/decorators.js';
import { IDashboardFooter } from './footer';
import { IVersionProvider } from '../../util/tools/versionprovider.service';
import { consume } from '@lit/context';
import { versionProviderContext } from '../../context/versionprovider.context';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

@customElement('urltracker-dashboard-footer')
export class DashboardFooter extends UmbElementMixin(LitElement) {
  static styles = css`
    .url-tracker__footer {
      background-color: white;
      box-sizing: border-box;
      border-top: 1px solid #e9e9eb;
      height: 50px;
      display: flex;
      align-items: center;
      flex-direction: row;
    }

    .url-tracker__footer__logo {
      height: 100%;
    }

    .url-tracker__footer__logo__link {
      height: 100%;
      margin-right: 1rem;
    }

    .url-tracker__footer__links {
      flex: 1;
      margin-right: 2rem;
    }

    .url-tracker__footer__links ul {
      display: flex;
      justify-content: flex-end;
      list-style-type: none;
    }

    .url-tracker__footer__links ul li {
      margin-left: 2rem;
    }

    .url-tracker__footer__links ul li a:link,
    a:visited,
    a:hover,
    a:active {
      color: black;
      text-decoration: none;
    }

    .url-tracker__footer__links ul li a:hover {
      text-decoration: underline;
    }
  `;

  constructor() {
    super();
    this.model = null;
  }

  render() {
    if (!this.model) {
      return nothing;
    }

    let linkList = null;
    if (this.model.links) {
      linkList = html`
        <ul>
          ${this.model.links.map(
            (el) => html`
              <li>
                <a href="${el.url}" target="${el.target}" rel="noreferrer noopener">${el.title}</a>
              </li>
            `,
          )}
        </ul>
      `;
    }

    return html`
      <footer class="url-tracker__footer">
        <a
          href="${this.model.logoUrl}"
          target="_blank"
          rel="noopener noreferrer"
          class="url-tracker__footer__logo__link"
          ><img src="${this.model.logo}" alt="Infocaster logo" class="url-tracker__footer__logo"
        /></a>
        <div class="url-tracker__footer__version">v${this.model.version}</div>
        <div class="url-tracker__footer__links">${linkList}</div>
      </footer>
    `;
  }

  connectedCallback(): void {
    super.connectedCallback();
    this.initModel();
  }

  @state()
  public model: IDashboardFooter | null;

  @consume({ context: versionProviderContext })
  private versionProvider?: IVersionProvider;

  private initModel = () => {
    this.model = {
      logo: this.localize.term('urlTrackerDashboardFooter_logo'),
      logoUrl: this.localize.term('urlTrackerDashboardFooter_logourl'),
      version: this.versionProvider ? this.versionProvider.version : '',
      links: [
        {
          url: 'https://github.com/Infocaster/UrlTracker/discussions',
          title: this.localize.term('urlTrackerDashboardFooter_featurelabel'),
          target: '_blank',
        },
        {
          url: 'https://github.com/Infocaster/UrlTracker/issues',
          title: this.localize.term('urlTrackerDashboardFooter_buglabel'),
          target: '_blank',
        },
        {
          url: 'https://github.com/Infocaster/UrlTracker/wiki',
          title: this.localize.term('urlTrackerDashboardFooter_wikilabel'),
          target: '_blank',
        },
      ],
    };
  };
}
