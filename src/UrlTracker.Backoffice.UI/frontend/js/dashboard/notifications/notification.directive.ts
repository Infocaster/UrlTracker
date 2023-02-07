export class UrlTrackerNotificationController {

    public static alias: string = "UrlTracker.Notification.Controller"

    public notifications: Array<any> | null = null;

    
}

UrlTrackerNotificationDirective.alias = "ngUrltrackerNotification"
export function UrlTrackerNotificationDirective(): angular.IDirective {
    return {
        restrict: 'E',
        template: 'App_Plugins/UrlTracker/dashboard/notifications/notification.directive.html',
        controller: UrlTrackerNotificationController.alias,
        controllerAs: 'vm'
    };
}