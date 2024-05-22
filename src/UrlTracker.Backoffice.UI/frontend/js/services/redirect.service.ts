import { Axios } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, { IControllerUrlResource, IUrlResource } from "../util/tools/urlresource.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { IPaginationRequestBase } from "./models/paginationrequestbase";
import { IQueryRequestBase } from "./models/queryrequestbase";
import { IRedirectFilterRequestBase } from "./models/redirectfilterrequestbase";
import qs from "qs";

export interface IRedirectResponseStrategy {
    strategy: string;
    value: string;
}

export interface IRedirectResponse {
    id: number;
    createDate: Date;
    updateDate: Date;
    source: IRedirectResponseStrategy;
    target: IRedirectResponseStrategy;
    permanent: boolean;
    retainQuery: boolean;
    force: boolean;
    key: string;
    additionalData: Record<string, unknown>;
}

export interface ISolvedRecommendationRequest {
    solvedRecommendation?: number
}

export type IRedirectCollectionResponse = IPagedCollectionResponseBase<IRedirectResponse>;
export type IListRedirectRequest = IPaginationRequestBase & IRedirectFilterRequestBase & IQueryRequestBase;

export interface IRedirectService {
    list: (request: IListRedirectRequest) => Promise<IRedirectCollectionResponse>;
    create: (request: IRedirectResponse & ISolvedRecommendationRequest) => Promise<IRedirectResponse>;
    update: (request: IRedirectResponse) => Promise<IRedirectResponse>;
    delete: (id: number) => Promise<void>;
    updateBulk: (request: IRedirectResponse[]) => Promise<IRedirectResponse[]>;
    deleteBulk: (ids: number[]) => Promise<void>;
}

export class RedirectService implements IRedirectService {
    constructor(private axios: Axios, private urlResource: IUrlResource) { }

    private get controller(): IControllerUrlResource{
        return this.urlResource.getController("redirects");
    }

    public async list(request: IListRedirectRequest): Promise<IRedirectCollectionResponse> {

        let response = await this.axios.get<IRedirectCollectionResponse>(this.controller.getUrl('list'),
        {
            params: request,
            paramsSerializer: (params) => {
                return qs.stringify(params, { arrayFormat: "repeat" })
            }
        }).catch((error) => { console.log(error); throw error; });

        return response.data;
    }

    public async create(request: IRedirectResponse & ISolvedRecommendationRequest): Promise<IRedirectResponse> {
        let response = await this.axios.post<IRedirectResponse>(this.controller.getUrl('create'), request);
        return response.data;
    }

    public async update(request: IRedirectResponse): Promise<IRedirectResponse> {
        let response = await this.axios.post<IRedirectResponse>(this.controller.getUrl('update') + `/${request.id}`, request);
        return response.data;
    }

    public async delete(id: number): Promise<void> {
        await this.axios.post(this.controller.getUrl('delete') + `/${id}`);
    }

    public async updateBulk(request: IRedirectResponse[]): Promise<IRedirectResponse[]> {
        const payload = request.map((r) => ({
            ...r,
            redirect: {
                source: r.source,
                target: r.target
            }
        }));
        let response = await this.axios.post<IRedirectResponse[]>(this.controller.getUrl('updateBulk'), payload);
        return response.data;
    }

    public async deleteBulk(ids: number[]): Promise<void> {
        await this.axios.post(this.controller.getUrl('deleteBulk'), ids);
    }
}

export default new RedirectService(axiosInstance, urlresource);