import { html } from '@umbraco-cms/backoffice/external/lit';
import './contenttarget.lit';
import { RedirectResponse, RedirectStrategyResponse } from '@/api';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { IRedirectTargetStrategy, UrlTrackerRedirectTargetBase } from '../api/redirecttarget.types';
import UrlTrackerRedirectTargetConstantsContext from '../api/redirecttargetconstants.context';
import { URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT } from '../api/redirecttargetconstants.contexttokens';
import { URLTRACKER_REDIRECT_TARGET_CONTENT_ALIAS } from './manifests';

export default class ContentTargetStrategyFactory extends UrlTrackerRedirectTargetBase {
  private strategy?: RedirectStrategyResponse;

  constructor(host: UmbControllerHost) {
    super(host, URLTRACKER_REDIRECT_TARGET_CONTENT_ALIAS);
    this.consumeContext(
      URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT,
      (instance?: UrlTrackerRedirectTargetConstantsContext) => {
        instance?.getStrategyKey('content').subscribe((value) => {
          this.strategy = value;
        });
      },
    );
  }

  getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy | undefined {
    if (redirect.target.strategy === this.strategy?.key) {
      return {
        getTemplate() {
          return html`<urltracker-redirect-target-content></urltracker-redirect-target-content>`;
        },
      };
    }
  }
}
