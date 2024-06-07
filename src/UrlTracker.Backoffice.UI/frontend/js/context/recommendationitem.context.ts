import { createContext } from '@lit/context';
import type { ProcessedRecommendationResponse } from '@/services/scoring/scoring.service';
export type { ProcessedRecommendationResponse } from '@/services/scoring/scoring.service';
export const recommendationKey = 'recommendation';
export const recommendationContext = createContext<ProcessedRecommendationResponse>(recommendationKey);
