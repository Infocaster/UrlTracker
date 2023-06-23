import { ContextConsumer, consume } from "@lit-labs/context";
import { LitElement, html, nothing } from "lit";
import { INotificationService, notificationServiceContext } from "../../context/notificationservice.context";
import { ILocalizationService, localizationServiceContext } from "../../context/localizationservice.context";
import { ITranslatedNotification, ITranslatedNotificationCollection } from "./notification";

type LitElementConstructor<T = LitElement> = new (...args: any[]) => T;

export function UrlTrackerNotificationWrapper<TBase extends LitElementConstructor>(Base: TBase, alias: string){

    return class NotificationWrapper extends Base{

        private _notifications?: ITranslatedNotificationCollection;
        private get notifications(): ITranslatedNotificationCollection | undefined {

            return this._notifications;
        }
        private set notifications(value: ITranslatedNotificationCollection | undefined) {

            this._notifications = value;
            this.requestUpdate('notifications');
        }

        private _notificationServiceConsumer = new ContextConsumer(this, {context: notificationServiceContext});
        private _localizationServiceConsumer = new ContextConsumer(this, {context: localizationServiceContext});

        protected get notificationService(): INotificationService | undefined {
            return this._notificationServiceConsumer.value
        };

        protected get localizationService(): ILocalizationService | undefined {
            return this._localizationServiceConsumer.value;
        };

        connectedCallback(): void {
            super.connectedCallback();
            this.updateNotifications(alias);
        }

        protected async updateNotifications(alias: string): Promise<void>{

            const notificationService = this.notificationService
            const localizationService = this.localizationService;

            if (!notificationService) throw Error("notification service is required before calling this method");
            if (!localizationService) throw Error("localization service is required before calling this method");

            let response = await notificationService.GetNotifications(alias);
            if (!response?.Notifications){
                this.notifications = undefined;
                return;
            }

            let notifications = response.Notifications;

            let translations = await Promise.all([
                // localize all titles and descriptions
                localizationService.localizeMany(notifications.map((n) => n.TranslatableTitleComponent)),
                localizationService.localizeMany(notifications.map((n) => n.TranslatableBodyComponent))
            ]);

            this.notifications = {
                notifications: notifications.map<ITranslatedNotification>((n, i) => ({
                    id: n.Id,
                    title: localizationService.tokenReplace(translations[0][i], n.TitleArguments),
                    body: localizationService.tokenReplace(translations[1][i], n.BodyArguments)
                }))
            }
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
                        <header>
                        ${this.notifications.notifications[0].title}
                        </header>
                    </aside>
                    <section>
                        ${internalRender}
                    </section>
                </div>
            `;
        }
    }
}