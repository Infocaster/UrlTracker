export const RECOMMENDATION_SORT_TYPE = {
  IMPORTANCE: 0,
  LAST_OCCURRENCE: 1,
  URL: 2,
  OCCURRENCES: 3,
} as const;

export type RecommendationSortType =
  (typeof RECOMMENDATION_SORT_TYPE)[keyof typeof RECOMMENDATION_SORT_TYPE];
