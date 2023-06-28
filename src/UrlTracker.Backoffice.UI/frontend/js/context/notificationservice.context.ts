import { createContext } from "@lit-labs/context";
import type { INotificationService } from "../dashboard/notifications/notification.service";
export type { INotificationService } from "../dashboard/notifications/notification.service";
export const notificationServiceKey = 'notificationService';
export const notificationServiceContext = createContext<INotificationService>(notificationServiceKey);