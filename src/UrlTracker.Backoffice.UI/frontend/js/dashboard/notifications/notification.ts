export interface INotificationCollection {

    Notifications: Array<INotification>
}

export interface INotification {

    Id: string,
    TranslatableTitleComponent: string,
    TitleArguments: Array<string>,
    TranslatableBodyComponent: string,
    BodyArguments: Array<string>
}

export interface ITranslatedNotificationCollection {

    notifications: Array<ITranslatedNotification>
}

export interface ITranslatedNotification {

    id: string,
    title: string,
    body: string
}