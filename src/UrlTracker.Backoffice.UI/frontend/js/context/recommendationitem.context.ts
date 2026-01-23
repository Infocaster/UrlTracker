import { createContext } from '@lit/context';
import type { RecommendationResponse } from '../../../api-client/types.gen';
export type { RecommendationResponse } from '../../../api-client/types.gen';
export const recommendationKey = 'recommendation';
export const recommendationContext = createContext<RecommendationResponse>(recommendationKey);
