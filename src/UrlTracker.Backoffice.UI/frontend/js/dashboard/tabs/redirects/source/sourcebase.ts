import { ContextConsumer } from '@lit/context';
import { IRedirectSourceStrategy } from './source.strategy';
import { IRedirectViewContext, redirectViewContext } from '../redirectview.context';
import { RedirectResponse } from '@/api';
import { LitElement } from '@umbraco-cms/backoffice/external/lit';
import { UmbElement } from '@umbraco-cms/backoffice/element-api';

export class UrlTrackerRedirectSource implements IRedirectSourceStrategy {
  private _redirectViewContextConsumer;

  constructor(
    private base: UmbElement & LitElement,
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

    const typeString = this.base.localize.term(this._typeKey);
    result = `${typeString ?? this._typeKey}: ${result}`;

    return result;
  }

  protected get viewContext(): IRedirectViewContext | undefined {
    return this._redirectViewContextConsumer.value;
  }
}
