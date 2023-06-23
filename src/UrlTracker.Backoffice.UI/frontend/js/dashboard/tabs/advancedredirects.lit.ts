import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";

@customElement("urltracker-advancedredirect-tab")
export class UrlTrackerAdvancedRedirectTab extends UrlTrackerNotificationWrapper(LitElement, 'advancedredirects') {

    protected renderInternal(): unknown {
        
        return html`<h2>Advanced Redirects</h2>`;
    }
}