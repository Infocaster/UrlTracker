import { ManifestGlobalContext } from '@umbraco-cms/backoffice/extension-registry';
import { URLTRACKER_SCORING_CONTEXT_ALIAS } from './contexttoken';

const manifest: ManifestGlobalContext = {
  type: 'globalContext',
  alias: URLTRACKER_SCORING_CONTEXT_ALIAS,
  name: 'Scoring parameters context',
  api: () => import('./scoring.service'),
};

export const manifests = [manifest];
