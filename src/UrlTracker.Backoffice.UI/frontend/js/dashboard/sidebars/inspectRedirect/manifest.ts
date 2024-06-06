import { RedirectResponse } from '@/api';
import { ManifestModal } from '@umbraco-cms/backoffice/extension-registry';
import { UmbModalToken } from '@umbraco-cms/backoffice/modal';

export const URLTRACKER_INSPECT_REDIRECT_MODAL_ALIAS = 'UrlTracker.InspectRedirect.modal';

export const URLTRACKER_INSPECT_REDIRECT_MODAL = new UmbModalToken<RedirectResponse, void>(
  URLTRACKER_INSPECT_REDIRECT_MODAL_ALIAS,
  {
    modal: {
      type: 'sidebar',
      size: 'small',
    },
  },
);

const manifest: ManifestModal = {
  type: 'modal',
  alias: URLTRACKER_INSPECT_REDIRECT_MODAL_ALIAS,
  name: 'Inspect redirect',
  element: () => import('./inspectRedirect.lit'),
};

export const manifests = [manifest];
