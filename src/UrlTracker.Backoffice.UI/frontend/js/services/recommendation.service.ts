import { Axios } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, {
  IControllerUrlResource,
  IUrlResource,
} from "../util/tools/urlresource.service";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";
import { IPaginationRequestBase } from "./models/paginationrequestbase";
import { IQueryRequestBase } from "./models/queryrequestbase";
import { IRecommendationFilterRequestBase } from "./models/recommendationfilterrequestbase";
import { ScoringService } from "./scoring.service";

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

export interface IRecommendationUpdate {
  id: number;
  recommendationStrategy: string;
  ignore: boolean;
}

export type IRecommendationCollection =
  IPagedCollectionResponseBase<IRecommendationResponse>;

export type IListRecommendationRequest = IPaginationRequestBase &
  IQueryRequestBase & IRecommendationFilterRequestBase;

export interface IRecommendationsService {
  list: (
    request: IListRecommendationRequest
  ) => Promise<IRecommendationCollection>;

  update: (
    request: IRecommendationUpdate
  ) => Promise<IRecommendationResponse>;

  delete: (
    request: IRecommendationResponse
  ) => Promise<IRecommendationResponse>;

  updateBulk: (
    request: IRecommendationUpdate[]
  ) => Promise<IRecommendationResponse[]>;
}

export class RecommendationsService implements IRecommendationsService {
  constructor(private axios: Axios, private urlResource: IUrlResource, private scoringService: ScoringService) {}

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
    const results = await Promise.all(response.data.results.map(async (r) => {
      const normalized = { ...r, updatedate: new Date(r.updatedate) }
      const score = await this.scoringService.getScore(normalized)
      return {...normalized, score}
    }))
    
    return {
      ...response.data,
      results: results
    };
  }

  public async update(
    request: IRecommendationUpdate
  ): Promise<IRecommendationResponse> {
    let response = await this.axios.post<IRecommendationResponse>(
      this.controller.getUrl("update"),
      request
    );
    return {...response.data, updatedate: new Date(response.data.updatedate)};
  }

  public async delete(
    request: IRecommendationResponse
  ): Promise<IRecommendationResponse> {
    let response = await this.axios.post<IRecommendationResponse>(
      this.controller.getUrl("delete"),
      request
    );
    return {...response.data, updatedate: new Date(response.data.updatedate)};
  }


  public async updateBulk(
    request: IRecommendationUpdate[]
  ): Promise<IRecommendationResponse[]> {
    let response = await this.axios.post<IRecommendationResponse[]>(
      this.controller.getUrl("updatebulk"),
      request
    );
    return response.data.map(r => ({...r, updatedate: new Date(r.updatedate)}));
  }
}

export default new RecommendationsService(axiosInstance, urlresource, new ScoringService(axiosInstance, urlresource));
