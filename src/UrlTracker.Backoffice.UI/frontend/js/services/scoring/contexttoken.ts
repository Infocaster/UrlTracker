import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import ScoringService from './scoring.service';

export const URLTRACKER_SCORING_CONTEXT_ALIAS = 'UrlTracker.Scoring.context';
export const URLTRACKER_SCORING_CONTEXT = new UmbContextToken<ScoringService>(URLTRACKER_SCORING_CONTEXT_ALIAS);
