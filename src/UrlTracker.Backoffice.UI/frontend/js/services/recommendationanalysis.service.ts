import { Axios } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, {
  IControllerUrlResource,
  IUrlResource,
} from "../util/tools/urlresource.service";
import { IRecommendationResponse } from "./recommendation.service";

export interface IRecommendationHistoryResponse {
  firstOccurance: string;
  lastOccurance: string;
  averagePerDay: number;
  dailyOccurances: {
    occurances: number;
    dateTime: string;
  }[];
  trend: number;
}

export interface IRecommendationReferrer {
  ReferrerOccurances: number;
  ReferrerUrl: string;
}

export type IRecommendationReferrerResponse = IRecommendationReferrer[];

export interface IRecommendationsAnalysisService {
  getHistory: (
    request: IRecommendationResponse
  ) => Promise<IRecommendationHistoryResponse>;

  getReferrers: (
    request: IRecommendationResponse
  ) => Promise<IRecommendationReferrerResponse>;
}

export class RecommendationsAnalysisService implements IRecommendationsAnalysisService {
  constructor(private axios: Axios, private urlResource: IUrlResource) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController("recommendationAnalysis");
  }

  public async getHistory(
    request: IRecommendationResponse
  ) {
    const response = await this.axios.get<IRecommendationHistoryResponse>(
      this.controller.getUrl("getHistory") + `/${request.id}`,
      {
        params: request,
      }
    );

    return response.data;
  }

  public async getReferrers(
    request: IRecommendationResponse
  ) {
    const response = await this.axios.get<IRecommendationReferrerResponse>(
      this.controller.getUrl("getReferrers") + `/${request.id}`,
      {
        params: request,
      }
    );

    return response.data;
  }
}

export default new RecommendationsAnalysisService(axiosInstance, urlresource);
