import { createContext } from '@lit/context';
import type { RecommendationResponse } from '@/api';
export type { RecommendationResponse } from '@/api';
export const recommendationKey = 'recommendation';
export const recommendationContext = createContext<RecommendationResponse>(recommendationKey);
