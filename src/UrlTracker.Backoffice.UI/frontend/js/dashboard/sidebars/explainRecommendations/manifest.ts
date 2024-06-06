import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { IExplainRecommendationsModel, IRecommendationAction } from './explainrecommendations';
import { ManifestModal } from '@umbraco-cms/backoffice/extension-registry';

export const URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL_ALIAS = 'UrlTracker.ExplainRecommendation.modal';

export const URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL = new UmbModalToken<
  IExplainRecommendationsModel,
  IRecommendationAction
>(URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL_ALIAS, {
  modal: {
    type: 'sidebar',
    size: 'medium',
  },
});

const manifest: ManifestModal = {
  type: 'modal',
  alias: URLTRACKER_EXPLAIN_RECOMMENDATION_MODAL_ALIAS,
  name: 'Explain recommendations',
  element: () => import('./explainRecommendations.lit'),
};

export const manifests = [manifest];
