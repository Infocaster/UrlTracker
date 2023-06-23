import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";

@customElement("urltracker-recommendations-tab")
export class UrlTrackerRecommendationsTab extends UrlTrackerNotificationWrapper(LitElement, 'recommendations') {

    protected renderInternal(): unknown {
        
        return html`<h2>Recommendations</h2>`;
    }
}