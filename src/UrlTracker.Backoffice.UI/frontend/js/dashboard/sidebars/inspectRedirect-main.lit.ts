import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./inspectRedirect/inspectRedirect.lit";

@customElement("urltracker-inspect-redirect-sidebar")
export class InspectRedirectSidebar extends AngularBridgeMixin(
  LitElement,
  html`
    <urltracker-angular-icon-registry>
      <urltracker-sidebar-inspect-redirect></urltracker-sidebar-inspect-redirect>
    </urltracker-angular-icon-registry>
 `
) {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }
}
