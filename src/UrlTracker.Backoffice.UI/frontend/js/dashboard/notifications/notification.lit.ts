import { css, html, LitElement, nothing } from "lit";
import { customElement, property } from "lit/decorators.js";
import { ITranslatedNotification } from "./notification";

@customElement('urltracker-notification-collection')
export class UrlTrackerNotificationCollection extends LitElement {
    @property({ type: Array})
    public notifications: Array<ITranslatedNotification> = [];

    private selectedNotification: ITranslatedNotification | null = this.notifications[0] || null;

    connectedCallback(): void {
        super.connectedCallback();
        this.selectedNotification = this.notifications[0] || null;
    }

    render() {
        if (!this.notifications.length || !this.selectedNotification){
            return nothing;
        }

        return html`
        <uui-box>
            <section class="notification">
                <uui-icon name="remove"></uui-icon>
                <h6>
                    <span>${this.selectedNotification.title}</span>
                    <span>${this.notifications.findIndex(n => n.id === this.selectedNotification?.id) + 1}/${this.notifications.length}</span> 
                </h6>
                <p>${this.selectedNotification.body}</p>
            </section>    
        </uui-box>
        `
    }

    static styles = css`
        uui-box {
            margin-bottom: 1rem;
        }

        uui-icon {
            position: absolute;
            right: 0;
            top: 0;
            cursor: pointer;
        }

        .notification {
            position: relative;
        }

        span {
            display: inline-block;
            margin-right: 0.5rem;
        }

        h6 {
            font-family: Lato;
            font-size: 15px;
            font-weight: 800;
            line-height: 20px;
            margin-top: 0;
            margin-bottom: 0.5rem;
        }

        p {
            font-family: Lato;
            font-size: 15px;
            font-weight: 400;
            line-height: 20px;
            margin: 0;
        }
    `;
}