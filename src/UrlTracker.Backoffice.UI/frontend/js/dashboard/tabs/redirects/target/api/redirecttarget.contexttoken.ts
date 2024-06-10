import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import UrlTrackerRedirectTargetContext from './redirecttarget.context';

export const URLTRACKER_REDIRECT_TARGET_CONTEXT_ALIAS = 'UrlTracker.RedirectTarget.context';

export const URLTRACKER_REDIRECT_TARGET_CONTEXT = new UmbContextToken<UrlTrackerRedirectTargetContext>(
  URLTRACKER_REDIRECT_TARGET_CONTEXT_ALIAS,
);
