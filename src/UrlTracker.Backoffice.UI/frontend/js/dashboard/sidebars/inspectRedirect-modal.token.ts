import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { RedirectResponse } from '../../../../api-client';

export type UrlTrackerInspectRedirectModalData = {
  redirect: RedirectResponse;
};

export type UrlTrackerInspectRedirectModalValue = undefined;

export const URL_TRACKER_INSPECT_REDIRECT_MODAL = new UmbModalToken<
  UrlTrackerInspectRedirectModalData,
  UrlTrackerInspectRedirectModalValue
>('UrlTracker.Modal.InspectRedirect', {
  modal: {
    type: 'sidebar',
    size: 'medium',
  },
});
