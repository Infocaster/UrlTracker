import urlresource, { IControllerUrlResource, IUrlResource } from "../util/urlresource.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { Axios } from "axios";

export interface IRecommendation {

    id: number;
    ignore: boolean;
    url: string;
    strategy: string;
}

export type IRecommendationCollection = IPagedCollectionResponseBase<IRecommendation>;

export interface IRecommendationsService {

    list(page: number, pageSize: number): Promise<IRecommendationCollection>;
}

export class RecommendationsService implements IRecommendationsService {

    constructor(private axios: Axios, private urlResource: IUrlResource) { }

    private get controller(): IControllerUrlResource {
        return this.urlResource.getController('recommendations');
    }

    public async list(page: number, pageSize: number): Promise<IRecommendationCollection> {

        let response = await this.axios.get<IRecommendationCollection>(this.controller.getUrl("list"), {
            params: {
                page,
                pageSize
            }
        });
        return response.data;
    }
}

const axiosInstance = new Axios({
    transformResponse: [
        (data) => {
            return JSON.parse(data.substring(6));
        }
    ]
});

export default new RecommendationsService(axiosInstance, urlresource);