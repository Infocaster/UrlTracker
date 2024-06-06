import { manifests as analyseRecommendationManifests } from './analyseRecommendation/manifest.ts';
import { manifests as explainRecommendationManifests } from './explainRecommendations/manifest.ts';
import { manifests as analyseRedirectManifests } from './inspectRedirect/manifest.ts';
import { manifests as simpleRedirectManifests } from './simpleRedirect/manifest.ts';

export const manifests = [
  ...analyseRecommendationManifests,
  ...explainRecommendationManifests,
  ...analyseRedirectManifests,
  ...simpleRedirectManifests,
];
