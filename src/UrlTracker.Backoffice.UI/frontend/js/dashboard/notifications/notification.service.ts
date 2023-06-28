import { INotificationCollection } from "./notification";
import { Axios } from "axios";
import urlresource, { IControllerUrlResource, IUrlResource } from "../../util/tools/urlresource.service";
import { axiosInstance } from "../../util/tools/axios.service";

export interface INotificationService {

    GetNotifications(alias: string): Promise<INotificationCollection>
}

export class NotificationService implements INotificationService {

    constructor(private axios: Axios, private urlResource: IUrlResource) { }

    private get controller(): IControllerUrlResource {

        return this.urlResource.getController("notifications");
    }

    public async GetNotifications(alias: string): Promise<INotificationCollection> {

        let response = await this.axios.get<INotificationCollection>(this.controller.getUrl('get'), {
            params: {
                alias: alias
            }
        });
        return response.data;
    }
}

export default new NotificationService(axiosInstance, urlresource);