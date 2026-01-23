import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { RedirectResponse } from '../../../../api-client';
import type { RedirectRequest } from '../../../../api-client/types.gen';

export type UrlTrackerSimpleRedirectModalData = {
  title: string;
  advanced: boolean;
  sourceEditable: boolean;
  data?: RedirectRequest;
  id?: number;
  solvedRecommendation?: any;
};

export type UrlTrackerSimpleRedirectModalValue = {
  response?: RedirectResponse;
};

export const URL_TRACKER_SIMPLE_REDIRECT_MODAL = new UmbModalToken<
  UrlTrackerSimpleRedirectModalData,
  UrlTrackerSimpleRedirectModalValue
>('UrlTracker.Modal.SimpleRedirect', {
  modal: {
    type: 'sidebar',
    size: 'medium',
  },
});
