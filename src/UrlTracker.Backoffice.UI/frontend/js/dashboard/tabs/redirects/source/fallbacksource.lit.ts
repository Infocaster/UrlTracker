import { LitElement, css } from "lit";
import { UrlTrackerRedirectSource } from "./sourcebase.mixin";
import { customElement } from "lit/decorators.js";

let baseType = UrlTrackerRedirectSource(LitElement, "urlTrackerRedirectSource_unknown");

@customElement('urltracker-redirect-source-unknown')
export class UrlTrackerUnknownSource extends baseType {
    static styles = [
        ...baseType.styles,
        css`
            :host {
                font-style: italic;
                color: var(--uui-color-danger);
            }
        `
    ];
}