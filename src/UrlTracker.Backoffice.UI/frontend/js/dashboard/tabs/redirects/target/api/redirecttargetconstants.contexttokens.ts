import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import UrlTrackerRedirectTargetConstantsContext from './redirecttargetconstants.context';

export const URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT_ALIAS = 'UrlTracker.RedirectTargetConstants.context';

export const URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT =
  new UmbContextToken<UrlTrackerRedirectTargetConstantsContext>(URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT_ALIAS);
