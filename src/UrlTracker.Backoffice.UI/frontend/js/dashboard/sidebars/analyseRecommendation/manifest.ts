import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { IAnalyseRecommendationModel } from './analyserecommendation';
import { ManifestModal } from '@umbraco-cms/backoffice/extension-registry';

export const URLTRACKER_ANALYSE_RECOMMENDATION_MODAL_ALIAS = 'UrlTracker.AnalyzeRecommendation.modal';

export const URLTRACKER_ANALYSE_RECOMMENDATION_MODAL = new UmbModalToken<IAnalyseRecommendationModel, void>(
  URLTRACKER_ANALYSE_RECOMMENDATION_MODAL_ALIAS,
  {
    modal: {
      type: 'sidebar',
      size: 'medium',
    },
  },
);

const modalmanifest: ManifestModal = {
  type: 'modal',
  alias: URLTRACKER_ANALYSE_RECOMMENDATION_MODAL_ALIAS,
  name: 'Analyze recommendation',
  element: () => import('./analyseRecommendation.lit'),
};

export const manifests = [modalmanifest];
