import { LitElement } from "lit";
import { UrlTrackerRedirectSource } from "./sourcebase.mixin";
import { customElement } from "lit/decorators.js";

@customElement('urltracker-redirect-source-url')
export class UrlTrackerUrlSource extends UrlTrackerRedirectSource(LitElement, "urlTrackerRedirectSource_url") { }