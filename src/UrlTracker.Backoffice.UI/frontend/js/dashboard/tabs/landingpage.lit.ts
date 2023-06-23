import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerNotificationWrapper } from "../notifications/notifications.mixin";
import './landingpage/recommendations.lit'

@customElement("urltracker-landing-tab")
export class UrlTrackerLandingTab extends UrlTrackerNotificationWrapper(LitElement, 'landingpage') {

    protected renderInternal(): unknown {
        
        return html`
        <div>
            <div><urltracker-recommendation-list></urltracker-recommendation-list></div>
            <div></div>
        </div>`;
    }
}