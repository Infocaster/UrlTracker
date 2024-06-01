import { createContext } from '@lit/context';
import type { IRecommendationsService } from '../services/recommendation.service';
export type { IRecommendationsService } from '../services/recommendation.service';
export const recommendationServiceKey = 'recommendationService';
export const recommendationServiceContext = createContext<IRecommendationsService>(recommendationServiceKey);
