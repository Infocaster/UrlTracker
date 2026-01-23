import { ReactiveControllerHost } from 'lit';
import { IRecommendationTypeStrategy } from './recommendation.strategy';

type HostElement = ReactiveControllerHost & HTMLElement;

export class UrlTrackerRecommendationType implements IRecommendationTypeStrategy {
  constructor(
    base: HostElement,
    private _typeKey: string,
    private _typeDescriptionKey: string,
  ) {}

  public get typeKey(): string {
    return this._typeKey;
  }

  public getTitle(): string {
    const typeString = 'typestring';
    //FIXME: localize
    // const typeString = await this.localizationService?.localize(this._typeKey);
    return typeString ?? this._typeKey;
  }

  public getDescription(): string {
    const result = 'fallback';
    //FIXME: localize
    // const result = await this.localizationService?.localize(this._typeDescriptionKey);
    return result ?? this._typeDescriptionKey;
  }
}
