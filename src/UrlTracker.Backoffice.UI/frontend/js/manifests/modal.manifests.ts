import { ManifestModal } from '@umbraco-cms/backoffice/modal';

export const modalManifests: Array<ManifestModal> = [
  {
    type: 'modal',
    alias: 'UrlTracker.Modal.SimpleRedirect',
    name: 'Simple Redirect Modal',
    element: () => import('../dashboard/sidebars/simpleRedirect/simpleRedirect.lit.js'),
  },
  {
    type: 'modal',
    alias: 'UrlTracker.Modal.InspectRedirect',
    name: 'Inspect Redirect Modal',
    element: () => import('../dashboard/sidebars/inspectRedirect/inspectRedirect.lit.js'),
  },
  {
    type: 'modal',
    alias: 'UrlTracker.Modal.AnalyseRecommendation',
    name: 'Analyse Recommendation Modal',
    element: () => import('../dashboard/sidebars/analyseRecommendation/analyseRecommendation.lit.js'),
  },
  {
    type: 'modal',
    alias: 'UrlTracker.Modal.ExplainRecommendation',
    name: 'Explain Recommendation Modal',
    element: () => import('../dashboard/sidebars/explainRecommendations/explainRecommendations.lit.js'),
  },
];
