import { IRecommendationsAnalysisService } from "@/services/recommendationanalysis.service";
import { createContext } from "@lit/context";
export type { IRecommendationsAnalysisService } from "../services/recommendationanalysis.service";
export const recommendationsAnalysisServiceKey = "recommendationAnalysisService";
export const recommendationsAnalysisServiceContext =
  createContext<IRecommendationsAnalysisService>(recommendationsAnalysisServiceKey);
