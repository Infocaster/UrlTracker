import { redirectTargetServiceContext } from "@/context/redirecttargetservice.context";
import { AngularBridgeMixin } from "@/util/bridge/angularbridge.mixin";
import { provide } from "@lit/context";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import targetService, { ITargetService } from "../tabs/redirects/target/target.service";
import "./simpleRedirect/simpleRedirect.lit";

@customElement("urltracker-simple-redirect-sidebar")
export class SimpleRedirectSidebar extends AngularBridgeMixin(
  LitElement,
  html`<urltracker-sidebar-simple-redirect></urltracker-sidebar-simple-redirect>`
) {
  @provide({ context: redirectTargetServiceContext })
  redirectTargetService: ITargetService = targetService;
}
