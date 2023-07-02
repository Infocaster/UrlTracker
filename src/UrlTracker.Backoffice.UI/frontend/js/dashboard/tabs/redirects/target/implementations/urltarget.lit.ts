import { LitElement, css, html } from "lit";
import { UrlTrackerRedirectTarget } from "../targetbase.mixin";
import { customElement } from "lit/decorators.js";
import '@umbraco-ui/uui';

let baseType = UrlTrackerRedirectTarget(LitElement, "urlTrackerRedirectTarget_url");

@customElement('urltracker-redirect-target-url')
export class UrlTrackerUrlRedirectTarget extends baseType {

    protected renderBody(): unknown {
        
        return html`
            <uui-icon name="icon-link"></uui-icon> ${this.redirect?.target.value}
        `;
    }

    static styles = [
        ...baseType.styles,
        css`
            uui-icon {
                align-self: center;
            }
        `
    ]
}