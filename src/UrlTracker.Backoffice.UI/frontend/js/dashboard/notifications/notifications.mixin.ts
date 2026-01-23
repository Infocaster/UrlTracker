import { ContextConsumer } from '@lit/context';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { html, nothing } from 'lit';
import { getUmbracoManagementApiV1UrlTrackerNotificationsByAlias } from '../../../../api-client';
import type { Client } from '../../../../api-client/client/types.gen';
import { INotificationService, notificationServiceContext } from '../../context/notificationservice.context';
import { LitElementConstructor } from '../../util/tools/litelementconstructor';
import { ITranslatedNotification, ITranslatedNotificationCollection } from './notification';
import './notification.lit';

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

    private _notificationServiceConsumer = new ContextConsumer(this, {
      context: notificationServiceContext,
    });

    protected get notificationService(): INotificationService | undefined {
      return this._notificationServiceConsumer.value;
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
      const response = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerNotificationsByAlias({
          client: umbHttpClient as unknown as Client,
          path: {
            alias: alias,
          },
        }),
      );
      if (!response || !response.data || response.data.length === 0) {
        this.notifications = undefined;
        return;
      }

      const notifications = response.data;

      //TODO: check token replace
      //   const [titleTranslations, bodyTranslations] = await Promise.all([
      //     // localize all titles and descriptions
      //     localizationService.localizeMany(notifications.map((n) => n.translatableTitleComponent)),
      //     localizationService.localizeMany(notifications.map((n) => n.translatableBodyComponent)),
      //   ]);

      //   const normalizedNotifications = {
      //     notifications: notifications.map<ITranslatedNotification>((n, i) => ({
      //       id: n.id,
      //       title: localizationService.tokenReplace(titleTranslations[i], n.titleArguments),
      //       body: localizationService.tokenReplace(bodyTranslations[i], n.bodyArguments),
      //     })),
      //   };

      const titleTranslations = notifications
        .map((n) => n.translatableTitleComponent)
        .map((t) => this.localize.term(t));

      const bodyTranslations = notifications.map((n) => n.translatableBodyComponent).map((t) => this.localize.term(t));

      const normalizedNotifications = {
        notifications: notifications.map<ITranslatedNotification>((n, i) => ({
          id: n.id,
          title: titleTranslations[i],
          body: bodyTranslations[i],
        })),
      };

      const seenNotifications = localStorage.getItem('seenNotifications');

      if (seenNotifications) {
        const json = JSON.parse(seenNotifications);
        normalizedNotifications.notifications = normalizedNotifications.notifications.filter((n) => !json[n.id]);
      }

      this.notifications = normalizedNotifications;
      this.requestUpdate();
    }

    connectedCallback(): void {
      super.connectedCallback();
      this.updateNotifications(this._alias!);
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
