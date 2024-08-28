import { ContextConsumer } from '@lit/context';
import { ILocalizationService, localizationServiceContext } from '../../../../context/localizationservice.context';
import { ReactiveControllerHost } from 'lit';
import { IRecommendationTypeStrategy } from './recommendation.strategy';

type HostElement = ReactiveControllerHost & HTMLElement;

export class UrlTrackerRecommendationType implements IRecommendationTypeStrategy {
  private _localizationServiceConsumer;

  constructor(
    base: HostElement,
    private _typeKey: string,
    private _typeDescriptionKey: string,
  ) {
    this._localizationServiceConsumer = new ContextConsumer(base, {
      context: localizationServiceContext,
    });
  }

  async getTitle(): Promise<string> {
    const typeString = await this.localizationService?.localize(this._typeKey);
    return typeString ?? this._typeKey;
  }

  async getDescription(): Promise<string> {
    const result = await this.localizationService?.localize(this._typeDescriptionKey);
    return result ?? this._typeDescriptionKey;
  }

  protected get localizationService(): ILocalizationService | undefined {
    return this._localizationServiceConsumer.value;
  }
}
