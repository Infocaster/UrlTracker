import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { scopeContext } from "@/context/scope.context";
import { ISourceStrategies } from "@/dashboard/tabs/redirects/source/source.constants";
import { ITargetStrategies } from "@/dashboard/tabs/redirects/target/target.constants";
import { IScope } from "@/models/scope.model";
import { IRedirectResponse } from "@/services/redirect.service";
import { ensureExists } from "@/util/tools/existancecheck";
import variableresourceService from "@/util/tools/variableresource.service";
import { consume } from "@lit/context";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";

import '../../../util/elements/redirects/simpleRedirect/createSimpleRedirect.lit';

export const ContentElementTag = "urltracker-sidebar-simple-redirect";

@customElement(ContentElementTag)
export class UrlTrackerSidebarSimpleRedirect extends LitElement {

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @consume({ context: scopeContext })
  private $scope?: IScope;

  @property({ attribute: false})
  get scope() { 
    ensureExists(this.$scope, "scope");
    return this.$scope;
  }

  @property({ attribute: false})
  get advancedView () {
    ensureExists(this.$scope, "scope");
    return this.scope.model.value?.advancedView ?? false;
  }

  @property({ attribute: false})
  get redirect () {
    ensureExists(this.$scope, "scope");
    return this.scope.model.value ?? {} as IRedirectResponse;
  }

  @state()
  private headerText = "";

  @state()
  private redirectData: IRedirectResponse = {
      source: {
          strategy: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').url,
          value: ""
      },
      target: {
          strategy: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
          value: ""
      },
      permanent: false,
      retainQuery: true,
      force: false,
  } as IRedirectResponse;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    
    if(this.redirect.id || this.redirect.source?.value) {
      // Editing existing redirect, id will be available if redirect is already saved, source will exist if coming from a recommendation
      this.redirectData = this.scope.model.value;
      this.headerText = `Edit: ${this.scope.model.value.source.value}` ?? "Edit redirect";
    } else {
      // Creating new redirect
      this.headerText = "Create new redirect";
    }
  }

  save() {
    this.scope.model.submit(this.redirectData);
  }

  close() {
    this.scope.model.close();
  }

  protected render() {
    return html`<div class="header">${this.headerText}</div>
      <div class="main">
        <urltracker-create-simple-redirect .advancedView=${this.advancedView} .redirect=${this.redirectData} @update=${({detail}: {detail: IRedirectResponse}) => this.redirectData = detail }></urltracker-create-simple-redirect>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}
          >Cancel</uui-button
        >
        <uui-button look="primary" color="positive"  @click=${this.save}>Save</uui-button>
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
