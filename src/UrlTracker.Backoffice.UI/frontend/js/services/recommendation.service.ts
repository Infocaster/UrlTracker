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
import { IDataWithId } from "./models/datawithid";
import { IEntityResponseId } from "./models/entityresponseid";

export type IRecommendationResponse = IEntityResponseId & IRecommendationResponseData;

export interface IRecommendationResponseData {
  
  ignore: boolean;
  url: string;
  strategy: string;
  score: number;
  updatedate: Date;
}

export type IRecommendationUpdateBulkRequest = IDataWithId<IRecommendationUpdate>[]

export interface IRecommendationUpdate {
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
    id: number,
    request: IRecommendationUpdate
  ) => Promise<IRecommendationResponse>;

  updateBulk: (
    request: IRecommendationUpdateBulkRequest
  ) => Promise<IRecommendationResponse[]>;
}

export class RecommendationsService implements IRecommendationsService {
  constructor(private axios: Axios, private urlResource: IUrlResource, private scoringService: ScoringService) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController("Recommendations");
  }

  public async list(
    request: IListRecommendationRequest
  ): Promise<IRecommendationCollection> {
    let response = await this.axios.get<IRecommendationCollection>(
      this.controller.getUrl("List"),
      {
        params: request,
      }
    );

    const results = await Promise.all(response.data.results.map(async (r) => {
      const normalized = r;
      const score = await this.scoringService.getScore(normalized)
      return {...normalized, score}
    }))
    
    return {
      ...response.data,
      results: results
    };
  }

  public async update(
    id: number,
    request: IRecommendationUpdate
  ): Promise<IRecommendationResponse> {
    let response = await this.axios.post<IRecommendationResponse>(
      this.controller.getUrl("Update", {recommendationId: id}),
      request
    );
    return response.data;
  }

  public async updateBulk(
    request: IRecommendationUpdateBulkRequest
  ): Promise<IRecommendationResponse[]> {
    let response = await this.axios.post<IRecommendationResponse[]>(
      this.controller.getUrl("UpdateBulk"),
      request
    );
    return response.data;
  }
}

export default new RecommendationsService(axiosInstance, urlresource, new ScoringService(axiosInstance, urlresource));
