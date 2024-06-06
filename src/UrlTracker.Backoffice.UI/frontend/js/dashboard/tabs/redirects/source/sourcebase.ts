import { ContextConsumer } from '@lit/context';
import { IRedirectSourceStrategy } from './source.strategy';
import { IRedirectViewContext, redirectViewContext } from '../redirectview.context';
import { RedirectResponse } from '@/api';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

export class UrlTrackerRedirectSource implements IRedirectSourceStrategy {
  private _redirectViewContextConsumer;

  constructor(
    private base: UmbLitElement,
    private _typeKey: string,
    private _redirect: RedirectResponse,
  ) {
    this._redirectViewContextConsumer = new ContextConsumer(base, {
      context: redirectViewContext,
    });
  }

  async getTitle(): Promise<string> {
    let result = this._redirect.source.value;
    if (!this.viewContext?.advanced) return result;

    const typeString = await this.base.localize.term(this._typeKey);
    result = `${typeString ?? this._typeKey}: ${result}`;

    return result;
  }

  protected get viewContext(): IRedirectViewContext | undefined {
    return this._redirectViewContextConsumer.value;
  }
}
