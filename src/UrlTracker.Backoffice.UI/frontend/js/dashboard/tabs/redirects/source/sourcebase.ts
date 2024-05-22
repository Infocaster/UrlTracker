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
import { IRedirectViewContext, redirectViewContext } from "../redirectview.context";

type HostElement = ReactiveControllerHost & HTMLElement

export class UrlTrackerRedirectSource implements IRedirectSourceStrategy {

  private _localizationServiceConsumer;
  private _redirectViewContextConsumer;

  constructor (base: HostElement, private _typeKey: string, private _redirect: IRedirectResponse) {
    
    this._localizationServiceConsumer = new ContextConsumer(base, {
      context: localizationServiceContext,
    });

    this._redirectViewContextConsumer = new ContextConsumer(base, {
      context: redirectViewContext
    })
  }

  async getTitle(): Promise<string> {
    
    let result = this._redirect.source.value;
    if (!this.viewContext?.advanced) return result;

    const typeString = await this.localizationService?.localize(this._typeKey);
    result = `${typeString ?? this._typeKey}: ${result}`;

    return result;
  }
  
  protected get localizationService(): ILocalizationService | undefined {
    return this._localizationServiceConsumer.value;
  }

  protected get viewContext(): IRedirectViewContext | undefined {
    return this._redirectViewContextConsumer.value;
  }
}
