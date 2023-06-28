import { LitElement, css, html } from "lit";
import { customElement } from "lit/decorators.js";

@customElement('urltracker-result-list-item')
export class UrlTrackerResultListItem extends LitElement {

    protected render(): unknown {
        return html`<slot></slot>`
    }

    static styles = [css`
        :host {
            display: block;
            border-radius: var(--uui-border-radius);
            background-color: white;
            padding: 16px 20px;
            box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
        }
    `];
}