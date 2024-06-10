import { html } from '@umbraco-cms/backoffice/external/lit';
import './fallbacktarget.lit';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { IRedirectTargetStrategy, UrlTrackerRedirectTargetBase } from '../api/redirecttarget.types';

export class UnknownTargetStrategyFactory extends UrlTrackerRedirectTargetBase {
  constructor(host: UmbControllerHost) {
    super(host, 'UrlTracker.Unknown.redirecttarget');
  }

  getStrategy(): IRedirectTargetStrategy | undefined {
    return {
      getTemplate() {
        return html`<urltracker-redirect-target-unknown></urltracker-redirect-target-unknown>`;
      },
    };
  }
}
