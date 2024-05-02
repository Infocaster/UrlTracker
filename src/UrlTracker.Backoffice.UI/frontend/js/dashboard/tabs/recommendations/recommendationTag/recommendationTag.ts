import { TemplateResult, html } from "lit";
import "./recommendationTag.lit";

export const RECCOMENDATION_TYPES = {
  VERY_IMPORTANT: "VERY_IMPORTANT",
  IMPORTANT: "IMPORTANT",
  MODERATELY_IMPORTANT: "MODERATELY_IMPORTANT",
  SLIGHTLY_IMPORTANT: "SLIGHTLY_IMPORTANT",
  NOT_IMPORTANT: "NOT_IMPORTANT",
} as const;

export type RecommendationTypes =
  (typeof RECCOMENDATION_TYPES)[keyof typeof RECCOMENDATION_TYPES];

export const calculateRecommendationType = (score: number) => {
  if (score >= 100) {
    return RECCOMENDATION_TYPES.VERY_IMPORTANT;
  } else if (score >= 20) {
    return RECCOMENDATION_TYPES.IMPORTANT;
  } else if (score >= 1) {
    return RECCOMENDATION_TYPES.MODERATELY_IMPORTANT;
  } else if (score >= 0.05) {
    return RECCOMENDATION_TYPES.SLIGHTLY_IMPORTANT;
  } else {
    return RECCOMENDATION_TYPES.NOT_IMPORTANT;
  }
};

export const recommendationTagFactory = (
  type: RecommendationTypes,
  importanceText: string
) => {
  const factories: Record<RecommendationTypes, () => TemplateResult> = {
    VERY_IMPORTANT: () =>
      html`<urltracker-recommendation-tag
        color="#F00"
        .text="${importanceText}"
      ></urltracker-recommendation-tag>`,
    IMPORTANT: () =>
      html`<urltracker-recommendation-tag
        color="#FFB800"
        .text="${importanceText}"
      ></urltracker-recommendation-tag>`,
    MODERATELY_IMPORTANT: () =>
      html`<urltracker-recommendation-tag
        color="#009906"
        .text="${importanceText}"
      ></urltracker-recommendation-tag>`,
    SLIGHTLY_IMPORTANT: () =>
      html`<urltracker-recommendation-tag
        color="#00B5B5"
        .text="${importanceText}"
      ></urltracker-recommendation-tag>`,
    NOT_IMPORTANT: () =>
      html`<urltracker-recommendation-tag
        color="#062EFF"
        .text="${importanceText}"
      ></urltracker-recommendation-tag>`,
  };

  return factories[type]();
};
