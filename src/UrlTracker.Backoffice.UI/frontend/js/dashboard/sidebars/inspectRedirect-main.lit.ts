import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import "./inspectRedirect/inspectRedirect.lit";

@customElement("urltracker-inspect-redirect-sidebar")
export class InspectRedirectSidebar extends AngularBridgeMixin(
  LitElement,
  html`   
  <urltracker-sidebar-inspect-redirect></urltracker-sidebar-inspect-redirect>
 `
) {}
