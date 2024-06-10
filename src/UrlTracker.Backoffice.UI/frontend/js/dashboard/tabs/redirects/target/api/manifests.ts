import { ManifestGlobalContext } from '@umbraco-cms/backoffice/extension-registry';
import { URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT_ALIAS } from './redirecttargetconstants.contexttokens';
import { URLTRACKER_REDIRECT_TARGET_CONTEXT_ALIAS } from './redirecttarget.contexttoken';

const constantsManifest: ManifestGlobalContext = {
  type: 'globalContext',
  alias: URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT_ALIAS,
  name: 'URL Tracker redirect target constants context',
  js: () => import('./redirecttargetconstants.context'),
};

const strategyManifest: ManifestGlobalContext = {
  type: 'globalContext',
  alias: URLTRACKER_REDIRECT_TARGET_CONTEXT_ALIAS,
  name: 'URL Tracker redirect target strategies context',
  js: () => import('./redirecttarget.context'),
};

export const manifests = [constantsManifest, strategyManifest];
