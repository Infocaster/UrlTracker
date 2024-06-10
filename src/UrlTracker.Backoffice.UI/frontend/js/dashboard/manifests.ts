import { ManifestDashboard } from '@umbraco-cms/backoffice/extension-registry';
import { manifests as sidebarManifests } from './sidebars/manifests';
import { manifests as redirecttargetapimanifests } from './tabs/redirects/target/api/manifests';
import { manifests as redirecttargetimplementationmanifests } from './tabs/redirects/target/implementations/manifests';

const dashboardManifest: ManifestDashboard = {
  type: 'dashboard',
  alias: 'UrlTracker.dashboard',
  name: 'URL Tracker',
  element: () => import('./content.lit'),
  meta: {},
  conditions: [
    {
      alias: 'Umb.Condition.SectionAlias',
      match: 'Umb.Section.Content',
    },
  ],
};

export const dashboardManifests = [
  dashboardManifest,
  ...sidebarManifests,
  ...redirecttargetapimanifests,
  ...redirecttargetimplementationmanifests,
];
