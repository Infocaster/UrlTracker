import { IRecommendationTypeStrategy } from './recommendation.strategy';
import { UmbElement } from '@umbraco-cms/backoffice/element-api';

export class UrlTrackerRecommendationType implements IRecommendationTypeStrategy {
  constructor(
    private base: UmbElement,
    private _typeKey: string,
    private _typeDescriptionKey: string,
  ) {}

  async getTitle(): Promise<string> {
    const typeString = this.base.localize.term(this._typeKey);
    return typeString ?? this._typeKey;
  }

  async getDescription(): Promise<string> {
    const result = this.base.localize.term(this._typeDescriptionKey);
    return result ?? this._typeDescriptionKey;
  }
}
