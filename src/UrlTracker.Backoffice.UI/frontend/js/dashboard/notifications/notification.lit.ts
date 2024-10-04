import { css, html, LitElement, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ITranslatedNotification } from './notification';

@customElement('urltracker-notification-collection')
export class UrlTrackerNotificationCollection extends LitElement {
  @property({ type: Array })
  public notifications: Array<ITranslatedNotification> = [];

  @state()
  private selectedNotification: ITranslatedNotification | null = null;

  @state()
  private notificationInterval: number | null = null;

  private selectNextNotification() {
    const currentIndex = this.notifications.findIndex((n) => n.id === this.selectedNotification?.id);
    const nextIndex = currentIndex + 1 >= this.notifications.length ? 0 : currentIndex + 1;
    this.selectedNotification = this.notifications[nextIndex];
  }

  private handleClose() {
    if (this.notificationInterval) {
      clearInterval(this.notificationInterval);
    }
    this.dispatchEvent(new CustomEvent('notification-closed', { bubbles: true, detail: this.selectedNotification }));
    this.selectedNotification = null;
  }

  connectedCallback(): void {
    super.connectedCallback();
    this.selectedNotification = this.notifications[0] || null;

    if (this.notifications.length > 1) {
      this.notificationInterval = window.setInterval(() => {
        this.selectNextNotification();
      }, 5000);
    }
  }

  disconnectedCallback(): void {
    super.disconnectedCallback();
    if (this.notificationInterval) {
      clearInterval(this.notificationInterval);
    }
  }

  render() {
    if (!this.notifications.length || !this.selectedNotification) {
      return nothing;
    }

    return html`
      <uui-box>
        <section class="notification">
          <button aria-label="close notification" @click=${this.handleClose}>
            <uui-icon name="remove"></uui-icon>
          </button>
          <h6>
            <span>${this.selectedNotification.title}</span>
            <span
              >${this.notifications.findIndex((n) => n.id === this.selectedNotification?.id) + 1}/${this.notifications
                .length}</span
            >
          </h6>
          <p>${this.selectedNotification.body}</p>
        </section>
      </uui-box>
    `;
  }

  static styles = css`
    uui-box {
      margin-bottom: 1rem;
    }

    button {
      background: none;
      color: inherit;
      border: none;
      padding: 0;
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
