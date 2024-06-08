import { ManifestLocalization } from '@umbraco-cms/backoffice/extension-registry';

export const enusManifest: ManifestLocalization = {
  type: 'localization',
  alias: 'UrlTracker.Localize.EnUS',
  name: 'URL Tracker English (United States)',
  meta: {
    culture: 'en-us',
  },
  js: () => import('./en-us.ts'),
};

export const manifests = [enusManifest];
