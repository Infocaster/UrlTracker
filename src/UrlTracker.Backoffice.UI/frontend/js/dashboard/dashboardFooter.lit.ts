import { css, html, LitElement } from "lit";
import { customElement } from "lit/decorators.js";

@customElement('urltracker-dashboard-footer')
export class DashboardFooter extends LitElement {

    static styles = css`
        footer{
            background-color: white;
            box-sizing: border-box;
            border-top: 1px solid #e9e9eb;
            height: 50px;
        }
    `;

    render() {
        return html`
<footer>

</footer>
        `;
    }
}