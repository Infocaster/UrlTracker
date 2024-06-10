import { ISourceStrategies } from '@/dashboard/tabs/redirects/source/source.constants';
import { ensureExists, ensureServiceExists } from '@/util/tools/existancecheck';
import variableresourceService from '@/util/tools/variableresource.service';
import { css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { customElement, property, state } from 'lit/decorators.js';

import '../../../util/elements/redirects/simpleRedirect/createSimpleRedirect.lit';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import {
  RedirectRequest,
  RedirectResponse,
  RedirectStrategyResponse,
  postApiV1UrlTrackerRedirects,
  postApiV1UrlTrackerRedirectsByRedirectId,
} from '@/api';
import { IManageRedirectModel } from './manageredirect';
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationContext } from '@umbraco-cms/backoffice/notification';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT } from '@/dashboard/tabs/redirects/target/api/redirecttargetconstants.contexttokens';
import UrlTrackerRedirectTargetConstantsContext from '@/dashboard/tabs/redirects/target/api/redirecttargetconstants.context';

export const ContentElementTag = 'urltracker-sidebar-simple-redirect';

@customElement(ContentElementTag)
export default class UrlTrackerSidebarSimpleRedirect extends UmbModalBaseElement<
  IManageRedirectModel,
  RedirectResponse
> {
  private _notificationContext?: UmbNotificationContext;
  private get notificationContext(): UmbNotificationContext {
    ensureServiceExists(this._notificationContext, 'notificationContext');
    return this._notificationContext;
  }

  private contentStrategy?: RedirectStrategyResponse;

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance?: UmbNotificationContext) => {
      this._notificationContext = instance;
    });

    this.consumeContext(
      URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT,
      (instance?: UrlTrackerRedirectTargetConstantsContext) => {
        instance?.getStrategyKey('content').subscribe((value) => {
          this.contentStrategy = value;
        });
      },
    );
  }

  @property({ attribute: false })
  get advancedView() {
    ensureExists(this.data, 'data must exist');
    return this.data.advanced;
  }

  @property({ attribute: false })
  get redirect() {
    ensureExists(this.data, 'data must exist');
    return this.data.data;
  }

  @state()
  private headerText = '';

  @state()
  private redirectData?: RedirectRequest;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    ensureExists(this.data, 'Data is required when using this modal');

    this.headerText = this.data.title;

    if (this.redirect) {
      this.redirectData = {
        source: this.redirect.source,
        target: this.redirect.target,
        permanent: this.redirect.permanent,
        retainQuery: this.redirect.retainQuery,
        force: this.redirect.force,
      };
    } else {
      this.assignDefaultRedirect();
    }
  }

  private assignDefaultRedirect() {
    ensureExists(this.contentStrategy, "Don't know what the strategy key is for content, because it is not assigned");
    this.redirectData = {
      source: {
        strategy: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').url,
        value: '',
      },
      target: {
        strategy: this.contentStrategy?.key,
        value: '',
      },
      permanent: false,
      retainQuery: true,
      force: false,
    };
  }

  async save() {
    ensureExists(this.data, 'data is required when using this modal');
    ensureExists(this.redirectData, "Redirect data should exist at this point, but it doesn't");
    let response: RedirectResponse;
    if (this.data.id) {
      const { data } = await tryExecuteAndNotify(
        this,
        postApiV1UrlTrackerRedirectsByRedirectId({ redirectId: this.data.id, requestBody: this.redirectData }),
      );
      response = data!;
      this.notificationContext.peek('positive', {
        data: {
          headline: 'Redirect updated',
          message: 'The redirect has been successfully updated',
        },
      });
    } else {
      const { data } = await tryExecuteAndNotify(
        this,
        postApiV1UrlTrackerRedirects({
          requestBody: {
            ...this.redirectData,
            solvedRecommendation: this.data.solvedRecommendation,
          },
        }),
      );
      response = data!;
      this.notificationContext.peek('positive', {
        data: {
          headline: 'Redirect created',
          message: 'The redirect has been successfully created',
        },
      });
    }

    this.modalContext?.setValue(response);
    this.modalContext?.submit();
  }

  close() {
    this.modalContext?.reject();
  }

  protected render() {
    if (!this.redirectData) {
      return nothing;
    }
    return html`<div class="header">${this.headerText}</div>
      <div class="main">
        <urltracker-create-simple-redirect
          .advancedView=${this.advancedView}
          .redirect=${this.redirectData}
          @update=${({ detail }: { detail: RedirectRequest }) => (this.redirectData = detail)}
        ></urltracker-create-simple-redirect>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>Cancel</uui-button>
        <uui-button look="primary" color="positive" @click=${this.save}>Save</uui-button>
      </div>`;
  }

  static styles = css`
    :host {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }

    .header {
      display: flex;
      align-items: center;
      font-weight: 600;
      padding: 10px 20px;
      height: 2.5rem;
      background-color: white;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }

    .main {
      flex: 1;
      padding: 16px 20px;
      overflow-y: auto;
    }

    .footer {
      display: flex;
      justify-content: flex-end;
      background-color: white;
      padding: 10px 20px;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }
  `;
}
