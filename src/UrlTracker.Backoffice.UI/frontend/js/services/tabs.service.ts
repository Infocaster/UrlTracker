import { Axios } from "axios";
import { IDashboardPageResponse } from "../extensions/dashboardPageResponse";

export class TabService{

    constructor(private axios: Axios) { }

    public async GetTabs(): Promise<IDashboardPageResponse> {
        
        let response = await this.axios.get<IDashboardPageResponse>('dashboardpages');
        return response.data;
    }
}

const axiosInstance = new Axios({
    baseURL: '/umbraco/backoffice/urltracker/extensions',
    transformResponse: [
        (data) => {
            return JSON.parse(data.substring(6));
        }
    ]
});
export default new TabService(axiosInstance);