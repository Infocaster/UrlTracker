import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";

@customElement("urltracker-redirect-tab")
export class UrlTrackerRedirectTab extends UrlTrackerNotificationWrapper(LitElement, 'redirects') {

    protected renderInternal(): unknown {
        
        return html`<h2>Redirects</h2>`;
    }
}