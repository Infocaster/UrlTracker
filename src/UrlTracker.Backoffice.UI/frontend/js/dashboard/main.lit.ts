import { html, LitElement } from "lit";
import { customElement } from "lit/decorators.js";
import './content.lit'
import { UrlTrackerMainContext } from "../context/maincontext.mixin";
import { provide } from "@lit-labs/context";
import notificationService, { INotificationService } from "./notifications/notification.service";
import { notificationServiceContext } from "../context/notificationservice.context";
import recommendationService, { IRecommendationsService } from "../services/recommendation.service";
import { recommendationServiceContext } from "../context/recommendationservice.context";
import versionProvider, { IVersionProvider } from "../util/tools/versionprovider.service";
import { versionProviderContext } from "../context/versionprovider.context";
import { IRedirectService, redirectServiceContext } from "../context/redirectservice.context";
import redirectService from "../services/redirect.service";
import '../util/elements/angulariconregistry.lit'
import { ITargetService, redirectTargetServiceContext } from "../context/redirecttargetservice.context";
import targetService from "./tabs/redirects/target/target.service";

@customElement('urltracker-dashboard')
export class UrlTrackerDashboard extends UrlTrackerMainContext(LitElement) {

    @provide({context: notificationServiceContext})
    notificationService: INotificationService = notificationService;

    @provide({context: recommendationServiceContext})
    recommendationService: IRecommendationsService = recommendationService;

    @provide({context: versionProviderContext})
    versionProvider: IVersionProvider = versionProvider;

    @provide({context: redirectServiceContext})
    redirectService: IRedirectService = redirectService;

    @provide({context: redirectTargetServiceContext})
    redirectTargetService: ITargetService = targetService;

    protected render(): unknown {
        return html`
            <urltracker-angular-icon-registry>
                <urltracker-dashboard-content></urltracker-dashboard-content>
            </urltracker-angular-icon-registry>
        `;
    }
}