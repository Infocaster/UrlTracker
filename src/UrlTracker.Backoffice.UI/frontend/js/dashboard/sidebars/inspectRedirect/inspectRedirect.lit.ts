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
import { consume } from "@lit/context";
import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
  
  export const ContentElementTag = "urltracker-sidebar-inspect-redirect";
  
  @customElement(ContentElementTag)
  export class UrlTrackerSidebarInspectRedirect extends LitElement {
    @consume({ context: editorServiceContext })
    private editorService?: IEditorService<any>;
  
    @consume({ context: scopeContext })
    private $scope?: IScope;
  
    @consume({ context: localizationServiceContext })
    private _localizationService?: ILocalizationService;
  
    @state()
    private _headerText = "";
  
    async connectedCallback(): Promise<void> {
        super.connectedCallback();
  
        console.log("model");
        console.log(this.$scope?.model);
        console.log(this.$scope?.model.title);
        this._headerText = this.$scope?.model.title ?? "";
    }
  
    close() {
      this.$scope?.model.close();
    }
  
    protected render() {
      return html`<div class="header">${this._headerText}</div>
        <div class="main">
            <h1>hello</h1>
        </div>
        <div class="footer">
          <uui-button look="default" color="default" @click=${this.close}
            >Cancel</uui-button
          >
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
  