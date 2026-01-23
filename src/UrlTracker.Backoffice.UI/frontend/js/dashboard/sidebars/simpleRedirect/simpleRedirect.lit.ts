import { ISourceStrategies } from '@/dashboard/tabs/redirects/source/source.constants';
import { ITargetStrategies } from '@/dashboard/tabs/redirects/target/target.constants';

import variableresourceService from '@/util/tools/variableresource.service';
import { provide } from '@lit/context';
import { css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

import { LoadingStatus } from '@/types/loadingStatus';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalContext, UmbModalExtensionElement } from '@umbraco-cms/backoffice/modal';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import type { Client } from '../../../../../api-client/client/types.gen';
import {
  postUmbracoManagementApiV1UrlTrackerRedirects,
  postUmbracoManagementApiV1UrlTrackerRedirectsByRedirectId,
} from '../../../../../api-client/sdk.gen';
import { CreateRedirectRequest, RedirectRequest } from '../../../../../api-client/types.gen';
import { ITargetService, redirectTargetServiceContext } from '../../../context/redirecttargetservice.context';
import targetService from '../../../dashboard/tabs/redirects/target/target.service';
import '../../../util/elements/redirects/simpleRedirect/createSimpleRedirect.lit';
import { UrlTrackerSimpleRedirectModalData, UrlTrackerSimpleRedirectModalValue } from '../simpleRedirect-modal.token';

export const ContentElementTag = 'urltracker-sidebar-simple-redirect';

@customElement(ContentElementTag)
export class UrlTrackerSidebarSimpleRedirect
  extends UmbLitElement
  implements UmbModalExtensionElement<UrlTrackerSimpleRedirectModalData, UrlTrackerSimpleRedirectModalValue>
{
  @provide({ context: redirectTargetServiceContext })
  redirectTargetService: ITargetService = targetService;

  @property({ attribute: false })
  modalContext?: UmbModalContext<UrlTrackerSimpleRedirectModalData, UrlTrackerSimpleRedirectModalValue>;

  @property({ attribute: false })
  data?: UrlTrackerSimpleRedirectModalData;

  #notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;

  @property({ attribute: false })
  get advancedView() {
    return !!this.data?.advanced;
  }

  get sourceEditable(): boolean {
    return !!this.data?.sourceEditable;
  }

  @property({ attribute: false })
  get redirect() {
    return this.data?.data;
  }

  @state()
  private headerText = '';

  @state()
  private redirectData: RedirectRequest = {
    source: {
      strategy: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').url,
      value: '',
    },
    target: {
      strategy: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
      value: '',
    },
    permanent: false,
    retainQuery: true,
    force: false,
    advanced: false,
  };

  @state()
  private saveLoading: LoadingStatus = undefined;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this.headerText = this.data?.title || '';

    if (this.redirect) {
      this.redirectData = {
        source: this.redirect.source,
        target: this.redirect.target,
        permanent: this.redirect.permanent,
        retainQuery: this.redirect.retainQuery,
        force: this.redirect.force,
        advanced: this.redirect.advanced,
      };
    }
  }

  get saveDisabled() {
    if (this.saveLoading === 'waiting') return true;
    return !this.redirectData.target.value;
  }

  async save() {
    this.saveLoading = 'waiting';
    let response: any;
    try {
      if (this.data?.id) {
        const result = await tryExecute(
          this,
          postUmbracoManagementApiV1UrlTrackerRedirectsByRedirectId({
            client: umbHttpClient as unknown as Client,
            path: {
              redirectId: this.data.id,
            },
            body: {
              source: this.redirectData.source,
              target: this.redirectData.target,
              permanent: this.redirectData.permanent,
              retainQuery: this.redirectData.retainQuery,
              force: this.redirectData.force,
              advanced: this.redirectData.advanced,
            } as RedirectRequest,
          }),
        );
        response = result.data;

        this.#notificationContext?.peek('positive', {
          data: {
            headline: this.localize.term('urlTrackerGeneral_redirect-updated') || 'Redirect updated',
            message:
              this.localize.term('urlTrackerGeneral_redirect-updated-message') ||
              'The redirect has been successfully updated',
          },
        });
      } else {
        const result = await tryExecute(
          this,
          postUmbracoManagementApiV1UrlTrackerRedirects({
            client: umbHttpClient as unknown as Client,
            body: {
              source: this.redirectData.source,
              target: this.redirectData.target,
              permanent: this.redirectData.permanent,
              retainQuery: this.redirectData.retainQuery,
              force: this.redirectData.force,
              advanced: this.redirectData.advanced,
              solvedRecommendation: this.data?.solvedRecommendation,
            } as CreateRedirectRequest,
          }),
        );
        response = result.data;

        this.#notificationContext?.peek('positive', {
          data: {
            headline: this.localize.term('urlTrackerGeneral_redirect-created') || 'Redirect created',
            message:
              this.localize.term('urlTrackerGeneral_redirect-created-message') ||
              'The redirect has been successfully created',
          },
        });
      }

      this.saveLoading = 'success';
      this.modalContext?.setValue({ response: response });
      this.modalContext?.submit();
    } catch (error) {
      this.saveLoading = 'failed';
    }
  }

  close() {
    this.modalContext?.reject();
  }

  protected render() {
    return html`<div class="header">${this.headerText}</div>
      <div class="main">
        <urltracker-create-simple-redirect
          .advancedView=${this.advancedView}
          .sourceEditable=${this.sourceEditable}
          .redirect=${this.redirectData}
          @update=${({ detail }: { detail: RedirectRequest }) => {
            this.redirectData = detail;
            this.requestUpdate();
          }}
        ></urltracker-create-simple-redirect>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>
          <umb-localize key="urlTrackerGeneral_cancel">Cancel</umb-localize>
        </uui-button>
        <uui-button
          look="primary"
          .disabled=${this.saveDisabled}
          .state=${this.saveLoading}
          color="positive"
          @click=${this.save}
        >
          <umb-localize key="urlTrackerGeneral_save">Save</umb-localize>
        </uui-button>
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

export const element = UrlTrackerSidebarSimpleRedirect;
