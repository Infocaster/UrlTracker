import { createContext } from '@lit/context';
import type { IUmbracoNotificationsService } from '../umbraco/notifications.service';
export type { IUmbracoNotificationsService } from '../umbraco/notifications.service';
export const umbracoNotificationsServiceContextKey = 'umbracoNotificationsService';
export const umbracoNotificationsServiceContext = createContext<IUmbracoNotificationsService>(
  umbracoNotificationsServiceContextKey,
);
