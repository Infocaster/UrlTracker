import { html, LitElement } from "lit";
import { customElement } from "lit/decorators.js";
import './content.lit'
import { UrlTrackerMainContext } from "../context/maincontext.mixin";
import { provide } from "@lit-labs/context";
import { tabServiceContext } from "../context/tabservice.context";
import tabsService, { TabService } from "../services/tabs.service";
import notificationService, { INotificationService } from "../services/notification.service";
import { notificationServiceContext } from "../context/notificationservice.context";
import recommendationService, { IRecommendationsService } from "../services/recommendation.service";
import { recommendationServiceContext } from "../context/recommendationservice.context";
import versionProvider, { IVersionProvider } from "../util/versionprovider.service";
import { versionProviderContext } from "../context/versionprovider.context";

@customElement('urltracker-dashboard')
export class UrlTrackerDashboard extends UrlTrackerMainContext(LitElement) {

    @provide({context: tabServiceContext})
    tabService: TabService = tabsService;

    @provide({context: notificationServiceContext})
    notificationService: INotificationService = notificationService;

    @provide({context: recommendationServiceContext})
    recommendationService: IRecommendationsService = recommendationService;

    @provide({context: versionProviderContext})
    versionProvider: IVersionProvider = versionProvider;

    protected render(): unknown {
        return html`<urltracker-dashboard-content></urltracker-dashboard-content>`;
    }
}