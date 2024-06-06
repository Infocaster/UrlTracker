import {
  RecommendationResponse,
  RedactionScoreResponse,
  ScoreParametersResponse,
  getApiV1UrlTrackerScoringRedactionscores,
  getApiV1UrlTrackerScoringScoreparameters,
} from '@/api';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { URLTRACKER_SCORING_CONTEXT } from './contexttoken';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';

export default class ScoringService extends UmbContextBase<ScoringService> {
  private resourcePromise?: Promise<void>;
  private redactionScores?: RedactionScoreResponse[];
  private parameters?: ScoreParametersResponse;

  constructor(host: UmbControllerHost) {
    super(host, URLTRACKER_SCORING_CONTEXT);
  }

  public async getScore(recommendation: RecommendationResponse): Promise<number> {
    if (!this.redactionScores) {
      if (!this.resourcePromise) {
        this.resourcePromise = this.ensureResources();
        this.resourcePromise.finally(() => (this.resourcePromise = undefined));
      }

      // This check is necessary, in case the 'finally' has already ran.
      // It's possible that the promise has already been truncated.
      if (this.resourcePromise) {
        await this.resourcePromise;
      }
    }

    // At this point, we know for certain that the resources exist
    // Calculate: (C1 × SR + C2 × SV) × 0.5^(ST / C3)
    const sv = recommendation.variableScore;
    const sr = this.redactionScores?.find((s) => s.key === recommendation.strategy)?.score ?? 0;
    const st = (new Date().getTime() - recommendation.updateDate.getTime()) / (1000 * 60 * 60 * 24);

    const adjustedsr = this.parameters!.redactionFactor * sr;
    const adjustedsv = this.parameters!.variableFactor * sv;
    const adjustedst = 0.5 ** (st / this.parameters!.timeFactor);

    const score = (adjustedsr + adjustedsv) * adjustedst;

    return score;
  }

  private async ensureResources(): Promise<void> {
    const [redactionScores, parameters] = await Promise.all([this.getRedactionScores(), this.getParameters()]);
    this.redactionScores = redactionScores;
    this.parameters = parameters;
  }

  private async getRedactionScores(): Promise<RedactionScoreResponse[]> {
    const { data } = await tryExecuteAndNotify(this, getApiV1UrlTrackerScoringRedactionscores());
    if (!data) throw new Error('Failed to fetch required data.');
    return data;
  }

  private async getParameters(): Promise<ScoreParametersResponse> {
    const { data } = await tryExecuteAndNotify(this, getApiV1UrlTrackerScoringScoreparameters());
    if (!data) throw new Error('Failed to fetch required data.');
    return data;
  }
}
