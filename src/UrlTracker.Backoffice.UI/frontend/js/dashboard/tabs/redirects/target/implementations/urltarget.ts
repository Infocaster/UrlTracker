import { html } from '@umbraco-cms/backoffice/external/lit';
import './urltarget.lit';
import { RedirectResponse, RedirectStrategyResponse } from '@/api';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { IRedirectTargetStrategy, UrlTrackerRedirectTargetBase } from '../api/redirecttarget.types';
import { URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT } from '../api/redirecttargetconstants.contexttokens';
import UrlTrackerRedirectTargetConstantsContext from '../api/redirecttargetconstants.context';

export default class UrlTargetStrategyFactory extends UrlTrackerRedirectTargetBase {
  private strategy?: RedirectStrategyResponse;

  constructor(host: UmbControllerHost) {
    super(host, 'UrlTracker.Url.redirecttarget');
    this.consumeContext(
      URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT,
      (instance?: UrlTrackerRedirectTargetConstantsContext) => {
        instance?.getStrategyKey('url').subscribe((value) => {
          this.strategy = value;
        });
      },
    );
  }

  public getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy | undefined {
    if (redirect.target.strategy === this.strategy?.key) {
      return {
        getTemplate() {
          return html`<urltracker-redirect-target-url></urltracker-redirect-target-url>`;
        },
      };
    }
  }
}
