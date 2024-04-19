export const RECOMMENDATION_SORT_TYPE = {
  IMPORTANCE: 0,
  LAST_OCCURRENCE: 1,
  URL: 2,
  OCCURRENCES: 3,
} as const;

export type RecommendationSortType =
  (typeof RECOMMENDATION_SORT_TYPE)[keyof typeof RECOMMENDATION_SORT_TYPE];


export const REDIRECTTYPE_SORT_TYPE = {
  PERMANENT: 0,
  TEMPORARY: 1,
  ALL: 2,
} as const;

export type RedirectSortType =
  (typeof REDIRECTTYPE_SORT_TYPE)[keyof typeof REDIRECTTYPE_SORT_TYPE];