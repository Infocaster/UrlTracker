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

export interface IGetHistoryRequest {
  pastDays?: number
}

export type IRecommendationReferrerResponse = IRecommendationReferrer[];

export interface IRecommendationsAnalysisService {
  getHistory: (
    id: number,
    request: IGetHistoryRequest
  ) => Promise<IRecommendationHistoryResponse>;

  getReferrers: (
    id: number
  ) => Promise<IRecommendationReferrerResponse>;
}

export class RecommendationsAnalysisService implements IRecommendationsAnalysisService {
  constructor(private axios: Axios, private urlResource: IUrlResource) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController("RecommendationAnalysis");
  }

  public async getHistory(
    id: number,
    request: IGetHistoryRequest
  ) {
    const response = await this.axios.get<IRecommendationHistoryResponse>(
      this.controller.getUrl("GetHistory", {recommendationId: id}),
      {
        params: request,
      }
    );

    return response.data;
  }

  public async getReferrers(
    id: number
  ) {
    const response = await this.axios.get<IRecommendationReferrerResponse>(
      this.controller.getUrl("GetReferrers", {recommendationId: id})
    );

    return response.data;
  }
}

export default new RecommendationsAnalysisService(axiosInstance, urlresource);
