import { INotificationCollection } from "../dashboard/notifications/notification";
import { Axios } from "axios";

export interface INotificationService {

    GetNotifications(alias: string): Promise<INotificationCollection>
}

export class NotificationService implements INotificationService {

    constructor(private axios: Axios) { }

    public async GetNotifications(alias: string): Promise<INotificationCollection> {

        let response = await this.axios.get<INotificationCollection>('get', {
            params: {
                alias: alias
            }
        });
        return response.data;
    }
}

const axiosInstance = new Axios({
    baseURL: '/umbraco/backoffice/urltracker/notifications',
    transformResponse: [
        (data) => {
            return JSON.parse(data.substring(6));
        }
    ]
});
export default new NotificationService(axiosInstance);