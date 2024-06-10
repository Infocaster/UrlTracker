import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT } from './redirecttargetconstants.contexttokens';
import { UmbArrayState } from '@umbraco-cms/backoffice/observable-api';
import { RedirectStrategyResponse, getApiV1UrlTrackerRedirectStrategyTargets } from '@/api';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';

export default class UrlTrackerRedirectTargetConstantsContext extends UmbContextBase<UrlTrackerRedirectTargetConstantsContext> {
  private constants: UmbArrayState<RedirectStrategyResponse> = new UmbArrayState<RedirectStrategyResponse>(
    [],
    (entry) => entry.name,
  );
  public constantsObservable = this.constants.asObservable();

  private initialising: boolean = false;
  private initialised: boolean = false;

  constructor(host: UmbControllerHost) {
    super(host, URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT);
  }

  public getStrategyKey(name: string) {
    if (!this.initialised && !this.initialising) {
      this.initialising = true;
      this.fetchStrategies()
        .then(() => (this.initialised = true))
        .finally(() => (this.initialising = false));
    }

    return this.constants.asObservablePart((list) => list.find((el) => el.name === name));
  }

  private async fetchStrategies(): Promise<void> {
    const { data, error } = await tryExecuteAndNotify(this, getApiV1UrlTrackerRedirectStrategyTargets());
    if (!data) {
      throw new Error(error?.toString());
    }

    this.constants.setValue(data);
  }
}
