import { ContextConsumer } from "@lit/context";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../../../context/localizationservice.context";
import { ReactiveControllerHost } from "lit";
import {
  IRedirectResponse,
} from "../../../../context/redirectitem.context";
import { IRedirectSourceStrategy } from "./source.strategy";

type HostElement = ReactiveControllerHost & HTMLElement

export class UrlTrackerRedirectSource implements IRedirectSourceStrategy {

  private _localizationServiceConsumer;

  constructor (base: HostElement, private _typeKey: string, private _redirect: IRedirectResponse) {
    
    this._localizationServiceConsumer = new ContextConsumer(base, {
      context: localizationServiceContext,
    });
  }

  async getTitle(): Promise<string> {
    
    const typeString = await this.localizationService?.localize(this._typeKey);
    return `${typeString ?? this._typeKey}: ${this._redirect.source.value}`;
  }
  
  protected get localizationService(): ILocalizationService | undefined {
    return this._localizationServiceConsumer.value;
  }
}
