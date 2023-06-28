import { LitElement, html } from "lit";
import { UrlTrackerRedirectTarget } from "./targetbase.mixin";
import { customElement } from "lit/decorators.js";

@customElement('urltracker-redirect-target-url')
export class UrlTrackerUrlRedirectTarget extends UrlTrackerRedirectTarget(LitElement, "urlTrackerRedirectTarget_url") {

    protected renderBody(): unknown {
        
        return html`
            <uui-icon name="icon-link"></uui-icon> ${this.redirect?.target.value}
        `;
    }
}