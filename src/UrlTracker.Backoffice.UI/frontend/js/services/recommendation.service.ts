import { axiosInstance } from "../util/tools/axios.service";
import urlresource, {
  IControllerUrlResource,
  IUrlResource,
} from "../util/tools/urlresource.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { Axios } from "axios";
import { IQueryRequestBase } from "./models/queryrequestbase";
import { IPaginationRequestBase } from "./models/paginationrequestbase";

export interface IRecommendationResponse {
  id: number;
  ignore: boolean;
  url: string;
  strategy: string;
  score: number;
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
    let response = await this.axios.get<IRecommendationCollection>(
      this.controller.getUrl("list"),
      {
        params: request,
      }
    );

    return response.data;
  }
}

export default new RecommendationsService(axiosInstance, urlresource);
