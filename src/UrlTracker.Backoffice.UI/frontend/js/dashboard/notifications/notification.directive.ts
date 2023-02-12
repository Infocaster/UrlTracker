import { IDashboardTab } from "../dashboardTab";
import { ITranslatedNotificationCollection } from "./notification";
import { INotificationResource, NotificationResource } from "./notification.service";

export interface IUrlTrackerNotificationScope extends angular.IScope {
    category: string
}

export class UrlTrackerNotificationController {

    public static alias: string = "UrlTracker.Notification.Controller"

    public notifications: ITranslatedNotificationCollection | null = null;

    public static $inject = [NotificationResource.alias, "$scope"]
    constructor(private notificationResource: INotificationResource, private $scope: IUrlTrackerNotificationScope) { }

    public init(): angular.IPromise<void> {

        return this.updateNotifications();
    }

    private updateNotifications(): angular.IPromise<void> {

        return this.notificationResource.GetNotifications(this.$scope.category)
            .then((result) => {

                this.notifications = result;
            })
    }
}

ngUrlTrackerNotification.alias = "ngUrltrackerNotification"
export function ngUrlTrackerNotification(): angular.IDirective {
    return {
        restrict: 'E',
        templateUrl: '/App_Plugins/UrlTracker/dashboard/notifications/notification.directive.html',
        controller: UrlTrackerNotificationController.alias,
        controllerAs: 'vm',
        scope: {
            category: '<'
        }
    };
}