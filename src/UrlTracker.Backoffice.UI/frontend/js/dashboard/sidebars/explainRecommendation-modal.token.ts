import { RecommendationResponse } from '@/context/recommendationitem.context';
import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { IRecommendationAction } from './explainRecommendations/explainRecommendations.lit';

export type UrlTrackerExplainRecommendationModalData = {
  recommendation: RecommendationResponse;
};

export type UrlTrackerExplainRecommendationModalValue = {
  type: IRecommendationAction;
};

export const URL_TRACKER_EXPLAIN_RECOMMENDATION_MODAL = new UmbModalToken<
  UrlTrackerExplainRecommendationModalData,
  UrlTrackerExplainRecommendationModalValue
>('UrlTracker.Modal.ExplainRecommendation', {
  modal: {
    type: 'sidebar',
    size: 'medium',
  },
});
