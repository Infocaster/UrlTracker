import { ILocalizationService } from "../../umbraco/localizationService";
import { INotificationCollection, ITranslatedNotification, ITranslatedNotificationCollection } from "./notification";

export interface INotificationService {

    GetNotifications(alias: string): angular.IHttpPromise<INotificationCollection>
}

export class NotificationService implements INotificationService {

    public static alias = "urlTrackerNotificationService";

    public static $inject = ["$http"];
    constructor(private $http: angular.IHttpService) { }

    public GetNotifications(alias: string): angular.IHttpPromise<INotificationCollection> {

        return this.$http({
            url: '/umbraco/backoffice/urltracker/notifications/get',
            method: "GET",
            params: {
                alias: alias
            }
        });
    }
}

export interface INotificationResource {

    GetNotifications(alias: string): angular.IPromise<ITranslatedNotificationCollection | null>
}

export class NotificationResource implements INotificationResource {

    public static alias = "urlTrackerNotifcationResource";

    public static $inject = [NotificationService.alias, "localizationService", "$q"];

    constructor(private notificationService: INotificationService, private localizationService: ILocalizationService, private $q: angular.IQService) { }

    public GetNotifications(alias: string): angular.IPromise<ITranslatedNotificationCollection | null> {

        return this.notificationService.GetNotifications(alias)
            .then((result) => {

                let notifications = result.data.Notifications;

                if (!notifications) return this.$q.resolve(null);

                return this.$q.all([

                    // localize all titles and descriptions
                    this.localizationService.localizeMany(notifications.map((n) => n.TranslatableTitleComponent)),
                    this.localizationService.localizeMany(notifications.map((n) => n.TranslatableBodyComponent))

                ]).then<ITranslatedNotificationCollection>((val) => {

                    // turn all the translated components into notifications
                    return {
                        notifications: notifications.map<ITranslatedNotification>((n, i) => ({
                            id: n.Id,
                            title: this.localizationService.tokenReplace(val[0][i], n.TitleArguments),
                            body: this.localizationService.tokenReplace(val[1][i], n.BodyArguments)
                        }))
                    }
                })
            })
    }
}