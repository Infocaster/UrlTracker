import { ManifestUrlTrackerRedirectTarget, URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE } from '../api/redirecttarget.types';

export const URLTRACKER_REDIRECT_TARGET_CONTENT_ALIAS = 'UrlTracker.Content.redirecttarget';
const contenttargetmanifest: ManifestUrlTrackerRedirectTarget = {
  type: URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE,
  alias: URLTRACKER_REDIRECT_TARGET_CONTENT_ALIAS,
  name: 'URL Tracker content redirect target',
  js: () => import('./contenttarget'),
};

export const URLTRACKER_REDIRECT_TARGET_URL_ALIAS = 'UrlTracker.Url.redirecttarget';
const urltargetmanifest: ManifestUrlTrackerRedirectTarget = {
  type: URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE,
  alias: URLTRACKER_REDIRECT_TARGET_URL_ALIAS,
  name: 'URL Tracker URL redirect target',
  js: () => import('./urltarget'),
};

export const manifests = [contenttargetmanifest, urltargetmanifest];
