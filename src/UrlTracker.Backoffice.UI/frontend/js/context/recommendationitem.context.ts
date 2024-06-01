import { createContext } from '@lit/context';
import type { IRecommendationResponse } from '../services/recommendation.service';
export type { IRecommendationResponse } from '../services/recommendation.service';
export const recommendationKey = 'recommendation';
export const recommendationContext = createContext<IRecommendationResponse>(recommendationKey);
