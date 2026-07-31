import { provide } from '@lit/context';
import { html, LitElement } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import type { RedirectResponse } from '../../../../../api-client/types.gen';
import { redirectContext } from '../../../../../context/redirectitem.context';
import { IVariableResource } from '../../../../../util/tools/variableresource.service';
import { ITargetStrategies } from '../target.constants';
import { IRedirectTargetStrategy, IRedirectTargetStrategyFactory } from '../target.strategy';
import './contenttarget.lit';

@customElement('urltracker-redirect-target-content-wrapper')
export class UrlTrackerRedirectTargetContentWrapper extends LitElement {
  @property({ type: Object })
  redirect?: RedirectResponse;

  @state()
  @provide({ context: redirectContext })
  private _providedRedirect?: RedirectResponse;

  protected willUpdate(changedProperties: Map<string | number | symbol, unknown>): void {
    super.willUpdate(changedProperties);
    if (changedProperties.has('redirect')) {
      this._providedRedirect = this.redirect;
    }
  }

  protected render() {
    return html`<urltracker-redirect-target-content></urltracker-redirect-target-content>`;
  }
}

export class ContentTargetStrategyFactory implements IRedirectTargetStrategyFactory {
  constructor(private variableResource: IVariableResource) {}

  getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy | undefined {
    const key = this.variableResource.get<ITargetStrategies>('redirectTargetStrategies').content;

    if (redirect.target.strategy === key) {
      return {
        getTemplate() {
          return html`<urltracker-redirect-target-content-wrapper
            .redirect=${redirect}
          ></urltracker-redirect-target-content-wrapper>`;
        },
      };
    }
  }
}
