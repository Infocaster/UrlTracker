import { Axios } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, {
  IControllerUrlResource,
  IUrlResource,
} from "../util/tools/urlresource.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { IPaginationRequestBase } from "./models/paginationrequestbase";
import { IQueryRequestBase } from "./models/queryrequestbase";

interface IFlatRecommendationResponse {
  id: number;
  ignore: boolean;
  url: string;
  strategy: string;
  score: number;
  updatedate: string;
}

type IFlatRecommendationCollection =
  IPagedCollectionResponseBase<IFlatRecommendationResponse>;

export interface IRecommendationResponse {
  id: number;
  ignore: boolean;
  url: string;
  strategy: string;
  score: number;
  updatedate: Date;
}

export type IRecommendationCollection =
  IPagedCollectionResponseBase<IRecommendationResponse>;

export type IListRecommendationRequest = IPaginationRequestBase &
  IQueryRequestBase;

export interface IRecommendationsService {
  list: (
    request: IListRecommendationRequest
  ) => Promise<IRecommendationCollection>;
}

export class RecommendationsService implements IRecommendationsService {
  constructor(private axios: Axios, private urlResource: IUrlResource) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController("recommendations");
  }

  public async list(
    request: IListRecommendationRequest
  ): Promise<IRecommendationCollection> {
    let response = await this.axios.get<IFlatRecommendationCollection>(
      this.controller.getUrl("list"),
      {
        params: request,
      }
    );

    // normalize all dates into a date object so that we can use a consistent date api in the business logic
    return {
      ...response.data,
      results: response.data.results.map((r) => {return {...r, updatedate: new Date(r.updatedate)}})
    };
  }
}

export default new RecommendationsService(axiosInstance, urlresource);
