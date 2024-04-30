import { ensureServiceExists } from "@/util/tools/existancecheck";
import { ContextConsumer } from "@lit/context";
import { html, nothing } from "lit";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../context/localizationservice.context";
import {
  INotificationService,
  notificationServiceContext,
} from "../../context/notificationservice.context";
import { LitElementConstructor } from "../../util/tools/litelementconstructor";
import {
  ITranslatedNotification,
  ITranslatedNotificationCollection,
} from "./notification";

export function UrlTrackerNotificationWrapper<
  TBase extends LitElementConstructor
>(Base: TBase, alias: string) {
  return class NotificationWrapper extends Base {
    private _notifications?: ITranslatedNotificationCollection;
    private get notifications(): ITranslatedNotificationCollection | undefined {
      return this._notifications;
    }
    private set notifications(
      value: ITranslatedNotificationCollection | undefined
    ) {
      this._notifications = value;
      this.requestUpdate("notifications");
    }

    private _notificationServiceConsumer = new ContextConsumer(this, {
      context: notificationServiceContext,
    });
    private _localizationServiceConsumer = new ContextConsumer(this, {
      context: localizationServiceContext,
    });

    protected get notificationService(): INotificationService | undefined {
      return this._notificationServiceConsumer.value;
    }

    protected get localizationService(): ILocalizationService | undefined {
      return this._localizationServiceConsumer.value;
    }

    connectedCallback(): void {
      super.connectedCallback();
      this.updateNotifications(alias);
    }

    protected async updateNotifications(alias: string): Promise<void> {
      const notificationService = this.notificationService;
      const localizationService = this.localizationService;

      ensureServiceExists(notificationService, "notification service");
      ensureServiceExists(localizationService, "localization service");

      let response = await notificationService.GetNotifications(alias);
      if (!response) {
        this.notifications = undefined;
        return;
      }

      let notifications = response;

      let translations = await Promise.all([
        // localize all titles and descriptions
        localizationService.localizeMany(
          notifications.map((n) => n.translatableTitleComponent)
        ),
        localizationService.localizeMany(
          notifications.map((n) => n.translatableBodyComponent)
        ),
      ]);

      this.notifications = {
        notifications: notifications.map<ITranslatedNotification>((n, i) => ({
          id: n.id,
          title: localizationService.tokenReplace(
            translations[0][i],
            n.titleArguments
          ),
          body: localizationService.tokenReplace(
            translations[1][i],
            n.bodyArguments
          ),
        })),
      };
    }

    protected renderInternal(): unknown {
      return nothing;
    }

    protected render(): unknown {
      const internalRender = this.renderInternal();
      if (!this.notifications?.notifications) return internalRender;

      return html`
        <div>
          <aside>
            <header>${this.notifications.notifications[0].title}</header>
          </aside>
          <section>${internalRender}</section>
        </div>
      `;
    }
  };
}
