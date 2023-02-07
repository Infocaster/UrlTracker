import { css, html, LitElement, nothing } from "lit";
import { customElement, property } from "lit/decorators.js";
import { IDashboardFooter } from "./dashboardFooter";

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
        
        .left {

        }

        .right {

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

    @property({ type: Object, reflect: true })
    public model: IDashboardFooter | null;
}