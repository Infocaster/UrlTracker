import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { IDashboardFooter } from "./footer";
import { ILocalizationService } from "../../umbraco/localization.service";
import { IVersionProvider } from "../../util/tools/versionprovider.service";
import { consume } from "@lit/context";
import { versionProviderContext } from "../../context/versionprovider.context";
import { localizationServiceContext } from "../../context/localizationservice.context";

@customElement("urltracker-dashboard-footer")
export class DashboardFooter extends LitElement {
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
                <a
                  href="${el.url}"
                  target="${el.target}"
                  rel="noreferrer noopener"
                  >${el.title}</a
                >
              </li>
            `
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
          ><img
            src="${this.model.logo}"
            alt="Infocaster logo"
            class="url-tracker__footer__logo"
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

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  private initModel = () => {
    this.localizationService
      ?.localizeMany([
        "urlTrackerDashboardFooter_logo",
        "urlTrackerDashboardFooter_logourl",
        "urlTrackerDashboardFooter_featurelabel",
        "urlTrackerDashboardFooter_buglabel",
        "urlTrackerDashboardFooter_wikilabel",
      ])
      .then((result) => {
        this.model = {
          logo: result[0],
          logoUrl: result[1],
          version: this.versionProvider ? this.versionProvider.version : "",
          links: [
            {
              url: "https://github.com/Infocaster/UrlTracker/discussions",
              title: result[2],
              target: "_blank",
            },
            {
              url: "https://github.com/Infocaster/UrlTracker/issues",
              title: result[3],
              target: "_blank",
            },
            {
              url: "https://github.com/Infocaster/UrlTracker/wiki",
              title: result[4],
              target: "_blank",
            },
          ],
        };
      });
  };
}
