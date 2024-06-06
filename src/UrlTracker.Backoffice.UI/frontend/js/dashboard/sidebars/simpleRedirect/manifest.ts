import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { IManageRedirectModel } from './manageredirect';
import { RedirectResponse } from '@/api';
import { ManifestModal } from '@umbraco-cms/backoffice/extension-registry';

export const URLTRACKER_EDIT_REDIRECT_MODAL_ALIAS = 'UrlTracker.EditRedirect.modal';

export const URLTRACKER_EDIT_REDIRECT_MODAL = new UmbModalToken<IManageRedirectModel, RedirectResponse>(
  URLTRACKER_EDIT_REDIRECT_MODAL_ALIAS,
  {
    modal: {
      type: 'sidebar',
      size: 'medium',
    },
  },
);

const manifest: ManifestModal = {
  type: 'modal',
  alias: URLTRACKER_EDIT_REDIRECT_MODAL_ALIAS,
  name: 'Create or update redirect',
  element: () => import('./simpleRedirect.lit'),
};

export const manifests = [manifest];
