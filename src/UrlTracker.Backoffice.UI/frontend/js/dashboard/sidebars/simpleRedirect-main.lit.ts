import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./simpleRedirect/simpleRedirect.lit";

@customElement("urltracker-simple-redirect-sidebar")
export class SimpleRedirectSidebar extends AngularBridgeMixin(
  LitElement,
  html`
  <urltracker-angular-icon-registry>
    <urltracker-sidebar-simple-redirect></urltracker-sidebar-simple-redirect>
  </urltracker-angular-icon-registry>`
) {
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
  }
}
