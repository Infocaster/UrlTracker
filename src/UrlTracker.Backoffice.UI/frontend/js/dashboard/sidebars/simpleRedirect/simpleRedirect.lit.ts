import {
  IEditorService,
  editorServiceContext,
} from "@/context/editorservice.context";
import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { scopeContext } from "@/context/scope.context";
import { IScope } from "@/models/scope.model";
import { IRedirectResponse } from "@/services/redirect.service";
import { consume } from "@lit/context";
import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";

export const ContentElementTag = "urltracker-sidebar-simple-redirect";

@customElement(ContentElementTag)
export class UrlTrackerSidebarSimpleRedirect extends LitElement {
  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: scopeContext })
  private $scope?: IScope;

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @state()
  private headerText = "";

  @state()
  private redirectData: IRedirectResponse = {
      source: {
          strategy: "",
          value: ""
      },
      target: {
          strategy: "",
          value: ""
      },
      permanent: false,
      retainQuery: true,
      force: false,
  } as IRedirectResponse;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    console.log(this.$scope?.model);

    if(this.$scope?.model.value.id) {
      // Editing existing redirect
      this.redirectData = this.$scope?.model.value;
      this.headerText = `Edit: ${this.$scope?.model.value.source.value}` ?? "Edit redirect";
    } else {
      // Creating new redirect
      this.headerText = "Create new redirect";
    }
  }

  save() {
    console.log(this.redirectData);
    //this.$scope?.model.submit(this.redirectData);
  }

  close() {
    this.$scope?.model.close();
  }

  protected render() {
    return html`<div class="header">${this.headerText}</div>
      <div class="main">
        <urltracker-create-simple-redirect .redirect=${this.redirectData} @update=${({detail}: {detail: IRedirectResponse}) => this.redirectData = detail }></urltracker-create-simple-redirect>
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
