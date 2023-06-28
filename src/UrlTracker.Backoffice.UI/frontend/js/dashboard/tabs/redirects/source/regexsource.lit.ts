import { LitElement } from "lit";
import { UrlTrackerRedirectSource } from "./sourcebase.mixin";
import { customElement } from "lit/decorators.js";

@customElement('urltracker-redirect-source-regex')
export class UrlTrackerRegexSource extends UrlTrackerRedirectSource(LitElement, "urlTrackerRedirectSource_regex") { }