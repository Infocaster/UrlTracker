import { html, LitElement, nothing } from "lit";
import { customElement, property } from "lit/decorators.js";
import { ITranslatedNotificationCollection } from "./notification";

@customElement('urltracker-notification-collection')
export class UrlTrackerNotificationCollection extends LitElement {

    constructor() {
        super();
        this.model = null;
    }

    render() {

        if (!this.model || !this.model.notifications){
            return nothing;
        }

        let msg = this.model.notifications[0];

        return html`
        <div>
            <header>${msg.title}</header>
            <p>${msg.body}</p>
        <div>
        `
    }

    @property({ type: Object, reflect: true })
    public model: ITranslatedNotificationCollection | null;
}