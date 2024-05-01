export type INotificationCollection = Array<INotification>;

export interface INotification {
    id: string,
    translatableTitleComponent: string,
    titleArguments: Array<string>,
    translatableBodyComponent: string,
    bodyArguments: Array<string>
}

export interface ITranslatedNotificationCollection {
    notifications: Array<ITranslatedNotification>
}

export interface ITranslatedNotification {
    id: string,
    title: string,
    body: string
}