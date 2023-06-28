import { LitElement, css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { IDashboardFooter } from "./footer";
import { ILocalizationService } from "../../umbraco/localization.service";
import { IVersionProvider } from "../../util/tools/versionprovider.service";
import { consume } from "@lit-labs/context";
import { versionProviderContext } from "../../context/versionprovider.context";
import { localizationServiceContext } from "../../context/localizationservice.context";

@customElement('urltracker-dashboard-footer')
export class DashboardFooter extends LitElement {

    static styles = css`
        footer {
            background-color: white;
            box-sizing: border-box;
            border-top: 1px solid #e9e9eb;
            height: 50px;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
        }

        .left a {
            display: inline-block;
            height: 100%;
        }

        .left a img {
            height: 100%;
        }

        .right ul {
            list-style-type: none;
        }

        .right ul li {
            float: left;
            margin: 0 8px;
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
                    ${this.model.links.map(el => html`
                        <li><a href="${el.url}" target="${el.target}" rel="noreferrer noopener">${el.title}</a></li>
                    `)}
                </ul>
            `
        }

        return html`
<footer>
    <div class="left">
        <a href="${this.model.logoUrl}" target="_blank" rel="noopener noreferrer"><img src="${this.model.logo}" alt="" /></a>
        <span>${this.model.version}</span>
    </div>
    <div class="right">
        ${linkList}
    </div>
</footer>
        `;
    }

    connectedCallback(): void {

        super.connectedCallback();
        this.initModel();
    }

    @state()
    public model: IDashboardFooter | null;

    @consume({context: versionProviderContext})
    private versionProvider?: IVersionProvider;
    
    @consume({context: localizationServiceContext})
    private localizationService?: ILocalizationService;

    private initModel = () => {

        this.localizationService?.localizeMany([
            "urlTrackerDashboardFooter_logo",
            "urlTrackerDashboardFooter_logourl",
            "urlTrackerDashboardFooter_featurelabel",
            "urlTrackerDashboardFooter_buglabel"
        ]).then((result) => {

            this.model = {
                logo: result[0],
                logoUrl: result[1],
                version: this.versionProvider ? this.versionProvider.version : '',
                links: [
                    {
                        url: "https://github.com/Infocaster/UrlTracker/Discussions",
                        title: result[2],
                        target: "_blank"
                    },
                    {
                        url: "https://github.com/Infocaster/UrlTracker/Issues",
                        title: result[3],
                        target: "_blank"
                    }
                ]
            };
        })
    }
}