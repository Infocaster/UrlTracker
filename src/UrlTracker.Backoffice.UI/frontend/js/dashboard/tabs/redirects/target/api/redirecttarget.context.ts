import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { URLTRACKER_REDIRECT_TARGET_CONTEXT } from './redirecttarget.contexttoken';
import { UmbExtensionsApiInitializer } from '@umbraco-cms/backoffice/extension-api';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import {
  IRedirectTargetStrategy,
  ManifestUrlTrackerRedirectTarget,
  URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE,
  UrlTrackerRedirectTarget,
} from './redirecttarget.types';
import { Observable, UmbArrayState } from '@umbraco-cms/backoffice/observable-api';
import { RedirectResponse } from '@/api';
import { UnknownTargetStrategyFactory } from '../implementations/fallbacktarget';

export default class UrlTrackerRedirectTargetContext extends UmbContextBase<UrlTrackerRedirectTargetContext> {
  private strategies: Array<UrlTrackerRedirectTarget> = [];

  private fallback: UrlTrackerRedirectTarget = new UnknownTargetStrategyFactory(this);

  constructor(host: UmbControllerHost) {
    super(host, URLTRACKER_REDIRECT_TARGET_CONTEXT);

    this.initialiseStrategies();
  }

  private initialiseStrategies = () => {
    new UmbExtensionsApiInitializer<ManifestUrlTrackerRedirectTarget, typeof URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE>(
      this,
      umbExtensionsRegistry,
      URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE,
      [this._host],
      null,
      (providers) => {
        let values: UrlTrackerRedirectTarget[] = [];
        for (const provider of providers) {
          if (provider.api) {
            values.push(provider.api);
          }
        }

        this.strategies = values;
      },
    );
  };

  public getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy {
    return this.strategies.map((s) => s.getStrategy(redirect)).find((s) => !!s) ?? this.fallback.getStrategy(redirect)!;
  }
}
