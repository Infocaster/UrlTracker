import { RecommendationResponse } from '@/api';

export type IRecommendationAction = (typeof RECCOMENDATION_ACTIONS)[keyof typeof RECCOMENDATION_ACTIONS];

export const RECCOMENDATION_ACTIONS = {
  MAKE_PERMANENT: 'MAKE_PERMANENT',
  MAKE_TEMPORARY: 'MAKE_TEMPORARY',
  IGNORE: 'IGNORE',
} as const;

export interface IExplainRecommendationsModel {
  recommendation: RecommendationResponse;
}
