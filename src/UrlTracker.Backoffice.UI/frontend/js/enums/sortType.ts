export const RECOMMENDATION_SORT_TYPE = {
  IMPORTANCE: 0,
  MOST_RECENTLY_UPDATED: 1,
  URL: 2,
} as const;

export type RecommendationSortType = (typeof RECOMMENDATION_SORT_TYPE)[keyof typeof RECOMMENDATION_SORT_TYPE];

export const REDIRECTTYPE_SORT_TYPE = {
  NONE: 0,
  PERMANENT: 1,
  TEMPORARY: 2,
  ALL: 3,
} as const;

export type RedirectSortType = (typeof REDIRECTTYPE_SORT_TYPE)[keyof typeof REDIRECTTYPE_SORT_TYPE];
