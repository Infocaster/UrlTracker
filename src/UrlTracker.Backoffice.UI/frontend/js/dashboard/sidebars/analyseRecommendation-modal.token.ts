import { RecommendationResponse } from '@/context/recommendationitem.context';
import { UmbModalToken } from '@umbraco-cms/backoffice/modal';

export interface IAnalyseRecommendationModalData {
  recommendation: RecommendationResponse;
}

export type UrlTrackerAnalyseRecommendationModalData = IAnalyseRecommendationModalData;
export type UrlTrackerAnalyseRecommendationModalValue = undefined;

export const URL_TRACKER_ANALYSE_RECOMMENDATION_MODAL = new UmbModalToken<
  UrlTrackerAnalyseRecommendationModalData,
  UrlTrackerAnalyseRecommendationModalValue
>('UrlTracker.Modal.AnalyseRecommendation', {
  modal: {
    type: 'sidebar',
    size: 'medium',
  },
});
