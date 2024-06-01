import { RecommendationSortType } from '@/enums/sortType';

export interface IRecommendationFilterRequestBase {
  OrderBy: RecommendationSortType;
}
