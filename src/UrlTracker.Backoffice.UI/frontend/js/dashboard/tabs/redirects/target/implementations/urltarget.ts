import { html } from 'lit';
import { IRedirectResponse } from '../../../../../services/redirect.service';
import { IVariableResource } from '../../../../../util/tools/variableresource.service';
import { ITargetStrategies } from '../target.constants';
import { IRedirectTargetStrategy, IRedirectTargetStrategyFactory } from '../target.strategy';
import './urltarget.lit';

export class UrlTargetStrategyFactory implements IRedirectTargetStrategyFactory {
  constructor(private variableResource: IVariableResource) {}

  getStrategy(redirect: IRedirectResponse): IRedirectTargetStrategy | undefined {
    const key = this.variableResource.get<ITargetStrategies>('redirectTargetStrategies').url;

    if (redirect.target.strategy === key) {
      return {
        getTemplate() {
          return html`<urltracker-redirect-target-url></urltracker-redirect-target-url>`;
        },
      };
    }
  }
}
