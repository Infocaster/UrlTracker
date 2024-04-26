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
import { LitElement, css, html, svg } from "lit";
import { customElement, state } from "lit/decorators.js";
import { ifDefined } from "lit/directives/if-defined.js";

export const ContentElementTag = "urltracker-sidebar-recommendations";

@customElement(ContentElementTag)
export class UrlTrackerSidebarRecommendations extends LitElement {
  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: scopeContext })
  private $scope?: IScope;

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (!this.localizationService)
      throw new Error(
        "Some services are missing, check: localizationService, editorService, assetsService"
      );
  }

  close() {
    this.editorService!.close();
  }

  protected render() {
    return html` <h1>test sidebar</h1> `;
  }

  static styles = css``;
}
