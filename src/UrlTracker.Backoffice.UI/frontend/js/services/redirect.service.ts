import { Axios } from "axios";
import urlresource, { IControllerUrlResource, IUrlResource } from "../util/tools/urlresource.service";
import { axiosInstance } from "../util/tools/axios.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { IPaginationRequestBase } from "./models/paginationrequestbase";
import { IQueryRequestBase } from "./models/queryrequestbase";

export interface IRedirectResponseStrategy {

    strategy: string;
    value: string;
}

export interface IRedirectResponse {

    id: number;
    createDate: string;
    source: IRedirectResponseStrategy;
    target: IRedirectResponseStrategy;
    permanent: boolean;
    retainQuery: boolean;
    force: boolean;
    key: string;
}

export type IRedirectCollectionResponse = IPagedCollectionResponseBase<IRedirectResponse>;
export type IListRedirectRequest = IPaginationRequestBase & IQueryRequestBase;

export interface IRedirectService {

    list: (request: IListRedirectRequest) => Promise<IRedirectCollectionResponse>;
}

export class RedirectService implements IRedirectService {
    constructor(private axios: Axios, private urlResource: IUrlResource) { }

    private get controller(): IControllerUrlResource{
        return this.urlResource.getController("redirects");
    }

    public async list(request: IListRedirectRequest): Promise<IRedirectCollectionResponse> {

        let response = await this.axios.get<IRedirectCollectionResponse>(this.controller.getUrl('list'),
        {
            params: request
        });

        return response.data;
    }
}

export default new RedirectService(axiosInstance, urlresource);