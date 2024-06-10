import { RedirectResponse } from '@/api';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UmbController, UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { ManifestApi } from '@umbraco-cms/backoffice/extension-api';

export const URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE = 'urlTrackerRedirectTarget';

export interface IRedirectTargetStrategy {
  getTemplate(): unknown;
}

export interface UrlTrackerRedirectTarget extends UmbController {
  get alias(): string;
  getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy | undefined;
}

export interface ManifestUrlTrackerRedirectTarget extends ManifestApi<UrlTrackerRedirectTarget> {
  type: typeof URLTRACKER_REDIRECT_TARGET_MANIFESTTYPE;
}

export abstract class UrlTrackerRedirectTargetBase extends UmbControllerBase implements UrlTrackerRedirectTarget {
  constructor(
    host: UmbControllerHost,
    private _alias: string,
  ) {
    super(host);
  }

  public get alias(): string {
    return this._alias;
  }

  abstract getStrategy(redirect: RedirectResponse): IRedirectTargetStrategy | undefined;
}
