import { ensureExists, ensureServiceExists } from '@/util/tools/existancecheck';
import { ContextConsumer } from '@lit/context';
import { html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { LitElementConstructor } from '../../util/tools/litelementconstructor';
import { ITranslatedNotification, ITranslatedNotificationCollection } from './notification';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { getApiV1UrlTrackerNotificationsByAlias } from '@/api';

export function UrlTrackerNotificationWrapper<TBase extends LitElementConstructor>(Base: TBase, alias?: string) {
  return class NotificationWrapper extends UmbElementMixin(Base) {
    private _alias = alias;
    protected set alias(newAlias: string) {
      this._alias = newAlias;
    }

    private _notifications?: ITranslatedNotificationCollection;
    private get notifications(): ITranslatedNotificationCollection | undefined {
      return this._notifications;
    }
    private set notifications(value: ITranslatedNotificationCollection | undefined) {
      this._notifications = value;
      this.requestUpdate('notifications');
    }

    private async onNotificationClosed(event: CustomEvent<ITranslatedNotification>) {
      const seenNotifications = localStorage.getItem('seenNotifications');
      if (seenNotifications) {
        const json = JSON.parse(seenNotifications);
        json[event.detail.id] = true;
        localStorage.setItem('seenNotifications', JSON.stringify(json));
      } else {
        const json = { [event.detail.id]: true };
        localStorage.setItem('seenNotifications', JSON.stringify(json));
      }
    }

    protected async updateNotifications(alias: string): Promise<void> {
      const response = await getApiV1UrlTrackerNotificationsByAlias({ alias });
      if (!response) {
        this.notifications = undefined;
        return;
      }

      const notifications = response;

      const normalizedNotifications = {
        notifications: notifications.map<ITranslatedNotification>((n, i) => ({
          id: n.id,
          title: this.localize.term(n.translatableTitleComponent, ...n.titleArguments),
          body: this.localize.term(n.translatableBodyComponent, ...n.bodyArguments),
        })),
      };

      const seenNotifications = localStorage.getItem('seenNotifications');

      if (seenNotifications) {
        const json = JSON.parse(seenNotifications);
        normalizedNotifications.notifications = normalizedNotifications.notifications.filter((n) => !json[n.id]);
      }

      this.notifications = normalizedNotifications;
    }

    connectedCallback(): void {
      super.connectedCallback();
      ensureExists(this._alias, 'An alias is required when using this element, but none was provided.');
      this.updateNotifications(this._alias);
    }

    protected renderInternal(): unknown {
      return nothing;
    }

    protected render(): unknown {
      const internalRender = this.renderInternal();
      if (!this.notifications?.notifications) return internalRender;

      return html`
        <urltracker-notification-collection
          .notifications=${this.notifications.notifications}
          @notification-closed=${this.onNotificationClosed}
        ></urltracker-notification-collection>
        <section>${internalRender}</section>
      `;
    }
  };
}
