import { provide } from '@lit/context';
import { css, html, LitElement } from 'lit';
import { customElement } from 'lit/decorators.js';
import { UrlTrackerMainContext } from '../context/maincontext.mixin';
import { notificationServiceContext } from '../context/notificationservice.context';
import { versionProviderContext } from '../context/versionprovider.context';
// Removed: redirectImportService and IRedirectImportService - using direct client
import versionProvider, { IVersionProvider } from '../util/tools/versionprovider.service';
import './content.lit';
import notificationService, { INotificationService } from './notifications/notification.service';

//Sidebar imports

@customElement('urltracker-dashboard')
export class UrlTrackerDashboard extends UrlTrackerMainContext(LitElement) {
  @provide({ context: notificationServiceContext })
  notificationService: INotificationService = notificationService;

  @provide({ context: versionProviderContext })
  versionProvider: IVersionProvider = versionProvider;

  protected render(): unknown {
    return html` <urltracker-dashboard-content></urltracker-dashboard-content> `;
  }

  static styles = css`
    :host {
      height: 100%;
    }
  `;
}
