import { ILocalizationService, localizationServiceContext } from '@/context/localizationservice.context';
import { scopeContext } from '@/context/scope.context';
import { ISourceStrategies } from '@/dashboard/tabs/redirects/source/source.constants';
import { ITargetStrategies } from '@/dashboard/tabs/redirects/target/target.constants';
import { IRedirectData, IRedirectResponse, IRedirectService } from '@/services/redirect.service';
import { ensureExists, ensureServiceExists } from '@/util/tools/existancecheck';
import variableresourceService from '@/util/tools/variableresource.service';
import { consume } from '@lit/context';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ManageRedirectScope } from './scope';

import '../../../util/elements/redirects/simpleRedirect/createSimpleRedirect.lit';
import { redirectServiceContext } from '@/context/redirectservice.context';
import {
  IUmbracoNotificationsService,
  umbracoNotificationsServiceContext,
} from '@/context/notificationsservice.context';

export const ContentElementTag = 'urltracker-sidebar-simple-redirect';

@customElement(ContentElementTag)
export class UrlTrackerSidebarSimpleRedirect extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @consume({ context: redirectServiceContext })
  private _redirectService?: IRedirectService | undefined;
  public get redirectService(): IRedirectService {
    ensureServiceExists(this._redirectService, 'redirectService');
    return this._redirectService;
  }
  public set redirectService(value: IRedirectService | undefined) {
    this._redirectService = value;
  }

  @consume({ context: umbracoNotificationsServiceContext })
  private _notificationService?: IUmbracoNotificationsService | undefined;
  public get notificationService(): IUmbracoNotificationsService {
    ensureServiceExists(this._notificationService, 'notificationService');
    return this._notificationService;
  }
  public set notificationService(value: IUmbracoNotificationsService | undefined) {
    this._notificationService = value;
  }

  @consume({ context: scopeContext })
  private $scope?: ManageRedirectScope;

  @property({ attribute: false })
  get scope() {
    ensureExists(this.$scope, 'scope');
    return this.$scope;
  }

  @property({ attribute: false })
  get advancedView() {
    ensureExists(this.$scope, 'scope');
    return this.scope.model.advanced;
  }

  @property({ attribute: false })
  get redirect() {
    ensureExists(this.$scope, 'scope');
    return this.scope.model.data;
  }

  @state()
  private headerText = '';

  @state()
  private redirectData: IRedirectData = {
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
  };

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this.headerText = this.scope.model.title;

    if (this.redirect) {
      this.redirectData = {
        source: this.redirect.source,
        target: this.redirect.target,
        permanent: this.redirect.permanent,
        retainQuery: this.redirect.retainQuery,
        force: this.redirect.force,
      };
    }
  }

  async save() {
    let response: IRedirectResponse;
    if (this.scope.model.id) {
      response = await this.redirectService.update(this.scope.model.id, this.redirectData);
      this.notificationService.success('Redirect updated', 'The redirect has been successfully updated');
    } else {
      response = await this.redirectService.create({
        ...this.redirectData,
        solvedRecommendation: this.$scope?.model.solvedRecommendation,
      });
      this.notificationService.success('Redirect created', 'The redirect has been successfully created');
    }

    this.scope.model.submit(response);
  }

  close() {
    this.scope.model.close();
  }

  protected render() {
    return html`<div class="header">${this.headerText}</div>
      <div class="main">
        <urltracker-create-simple-redirect
          .advancedView=${this.advancedView}
          .redirect=${this.redirectData}
          @update=${({ detail }: { detail: IRedirectData }) => (this.redirectData = detail)}
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
