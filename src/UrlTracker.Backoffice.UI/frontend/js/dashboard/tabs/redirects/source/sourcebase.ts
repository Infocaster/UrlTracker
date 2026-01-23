import { ContextConsumer } from '@lit/context';
import { ReactiveControllerHost } from 'lit';
import { RedirectResponse } from '../../../../context/redirectitem.context';
import { IRedirectViewContext, redirectViewContext } from '../redirectview.context';
import { IRedirectSourceStrategy } from './source.strategy';

type HostElement = ReactiveControllerHost & HTMLElement;

export class UrlTrackerRedirectSource implements IRedirectSourceStrategy {
  private _redirectViewContextConsumer;

  constructor(
    base: HostElement,
    private _typeKey: string,
    private _redirect: RedirectResponse,
  ) {
    this._redirectViewContextConsumer = new ContextConsumer(base, {
      context: redirectViewContext,
    });
  }

  public getTitle(): string {
    let result = this._redirect.source.value;
    if (!this.viewContext?.advanced) return result;

    // const typeString = await this.localizationService?.localize(this._typeKey);
    const typeString = 'typestring';
    result = `${typeString ?? this._typeKey}: ${result}`;

    return result;
  }

  protected get viewContext(): IRedirectViewContext | undefined {
    return this._redirectViewContextConsumer.value;
  }
}
