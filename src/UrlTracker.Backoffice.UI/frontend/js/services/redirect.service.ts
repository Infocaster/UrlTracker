import { Axios } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, { IControllerUrlResource, IUrlResource } from "../util/tools/urlresource.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { IPaginationRequestBase } from "./models/paginationrequestbase";
import { IQueryRequestBase } from "./models/queryrequestbase";
import { IRedirectFilterRequestBase } from "./models/redirectfilterrequestbase";
import qs from "qs";
import { IEntityResponseId } from "./models/entityresponseid";
import { IDataWithId } from "./models/datawithid";

// Base models
export interface IRedirectStrategy {
    strategy: string;
    value: string;
}

export interface IRedirectData {

    source: IRedirectStrategy;
    target: IRedirectStrategy;
    permanent: boolean;
    retainQuery: boolean;
    force: boolean;
}

export interface IRedirectMetaData {
    
    createDate: Date;
    updateDate: Date;
    additionalData: Record<string, unknown>;
}

export interface ISolvedRecommendationRequest {
    solvedRecommendation?: number
}

// Request models
export type IListRedirectRequest = IPaginationRequestBase & IRedirectFilterRequestBase & IQueryRequestBase;
export type ICreateRedirectRequest = IRedirectData & ISolvedRecommendationRequest;
export type IUpdateRedirectRequest = IRedirectData;
export type IUpdateRedirectBulkRequest = IDataWithId<IRedirectData>[];

// Response models
export type IRedirectResponse = IEntityResponseId & IRedirectData & IRedirectMetaData
export type IRedirectCollectionResponse = IPagedCollectionResponseBase<IRedirectResponse>;

export interface IRedirectService {
    list: (request: IListRedirectRequest) => Promise<IRedirectCollectionResponse>;
    create: (request: ICreateRedirectRequest) => Promise<IRedirectResponse>;
    update: (id: number, request: IUpdateRedirectRequest) => Promise<IRedirectResponse>;
    delete: (id: number) => Promise<void>;
    updateBulk: (request: IUpdateRedirectBulkRequest) => Promise<IRedirectResponse[]>;
    deleteBulk: (ids: number[]) => Promise<void>;
}

export class RedirectService implements IRedirectService {
    constructor(private axios: Axios, private urlResource: IUrlResource) { }

    private get controller(): IControllerUrlResource{
        return this.urlResource.getController("Redirects");
    }

    public async list(request: IListRedirectRequest): Promise<IRedirectCollectionResponse> {

        let response = await this.axios.get<IRedirectCollectionResponse>(this.controller.getUrl('List'),
        {
            params: request,
            paramsSerializer: (params) => {
                return qs.stringify(params, { arrayFormat: "repeat" })
            }
        }).catch((error) => { console.log(error); throw error; });

        return response.data;
    }

    public async create(request: ICreateRedirectRequest): Promise<IRedirectResponse> {
        let response = await this.axios.post<IRedirectResponse>(this.controller.getUrl('Create'), request);
        return response.data;
    }

    public async update(id: number, request: IUpdateRedirectRequest): Promise<IRedirectResponse> {
        let response = await this.axios.post<IRedirectResponse>(this.controller.getUrl('Update', {redirectId: id}), request);
        return response.data;
    }

    public async delete(id: number): Promise<void> {
        await this.axios.post(this.controller.getUrl('Delete', {redirectId: id}));
    }

    public async updateBulk(request: IUpdateRedirectBulkRequest): Promise<IRedirectResponse[]> {
        let response = await this.axios.post<IRedirectResponse[]>(this.controller.getUrl('UpdateBulk'), request);
        return response.data;
    }

    public async deleteBulk(ids: number[]): Promise<void> {
        await this.axios.post(this.controller.getUrl('DeleteBulk'), ids);
    }
}

export default new RedirectService(axiosInstance, urlresource);