import { html } from 'lit';
import { IRedirectTargetStrategy, IRedirectTargetStrategyFactory } from '../target.strategy';
import './fallbacktarget.lit';

export class UnknownTargetStrategyFactory implements IRedirectTargetStrategyFactory {
  getStrategy(): IRedirectTargetStrategy | undefined {
    return {
      getTemplate() {
        return html`<urltracker-redirect-target-unknown></urltracker-redirect-target-unknown>`;
      },
    };
  }
}
