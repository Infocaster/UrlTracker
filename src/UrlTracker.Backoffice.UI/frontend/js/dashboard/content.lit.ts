import { IEditorService, editorServiceContext } from '@/context/editorservice.context';
import { redirectServiceContext } from '@/context/redirectservice.context';
import { IRedirectService } from '@/services/redirect.service';
import { ensureServiceExists } from '@/util/tools/existancecheck';
import { consume, provide } from '@lit/context';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { localizationServiceContext } from '../context/localizationservice.context';
import { tabContext } from '../context/tabcontext.context';
import { ILocalizationService } from '../umbraco/localization.service';
import './footer/footer.lit';
import tabStrategy, { ITab, TabStrategyCollection } from './tab';
import {
  IUmbracoNotificationsService,
  umbracoNotificationsServiceContext,
} from '@/context/notificationsservice.context';
import { createNewRedirectOptions } from './sidebars/simpleRedirect/manageredirect';

@customElement('urltracker-dashboard-content')
export class UrlTrackerDashboardContent extends LitElement {
  @provide({ context: tabContext })
  private _tabs?: Array<ITab>;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: redirectServiceContext })
  private _redirectService?: IRedirectService;

  @consume({ context: umbracoNotificationsServiceContext })
  private _notificationsService?: IUmbracoNotificationsService | undefined;
  public get notificationsService(): IUmbracoNotificationsService {
    ensureServiceExists(this._notificationsService, 'notificationsService');
    return this._notificationsService;
  }
  public set notificationsService(value: IUmbracoNotificationsService | undefined) {
    this._notificationsService = value;
  }

  @state()
  set tabs(tabs: Array<ITab> | undefined) {
    this._tabs = tabs;
    if (this._tabs && this._tabs.length > 0) {
      this.activeTab = this._tabs[0];
    } else {
      this.activeTab = null;
    }
  }
  get tabs() {
    return this._tabs;
  }

  @state()
  private activeTab: ITab | null = null;

  @state()
  public loading: number;

  private tabStrategyCollection: TabStrategyCollection = tabStrategy;

  @consume({ context: localizationServiceContext })
  public localizationService?: ILocalizationService;

  constructor() {
    super();
    this.loading = 0;
  }

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    ensureServiceExists(this._redirectService, 'redirect service');

    this.loading++;
    try {
      if (!this.localizationService)
        throw new Error('localization service is not defined, but is required by this element');

      const titleAliases = this.tabStrategyCollection.map((item) => item.nameKey);
      const labelAliases = this.tabStrategyCollection.map((item) => item.labelKey);

      const titlePromise = this.localizationService.localizeMany(titleAliases);
      const labels = await this.localizationService.localizeMany(labelAliases);
      const titles = await titlePromise;

      const result: Array<ITab> = this.tabStrategyCollection.map((item, index) => ({
        name: titles[index],
        label: labels[index] ? labels[index] : titles[index],
        template: item.template,
      }));

      this.tabs = result;
    } finally {
      this.loading--;
    }
  }

  closePanel = () => {
    this.editorService!.close();
  };

  private _openSidebar(_: Event) {
    const options = createNewRedirectOptions({
      title: 'New redirect',
      submit: this.closePanel,
      close: this.closePanel,
      advanced: this.activeTab?.name === 'Advanced redirects',
    });

    this.editorService!.open(options);
  }

  render() {
    let contentOrLoader;

    if (this.loading) {
      contentOrLoader = html`<uui-loader-bar animationDuration="1.5"></uui-loader-bar>`;
    } else {
      let tabsOrNothing;
      if (this.tabs && this.tabs?.length > 1) {
        tabsOrNothing = html` <div class="tabs-wrapper">
          <uui-tab-group>
            ${this.tabs?.map(
              (item) =>
                html`<uui-tab
                  label="${item.label ? item.label : item.name}"
                  ?active="${item === this.activeTab}"
                  @click="${() => (this.activeTab = item)}"
                  >${item.name}</uui-tab
                >`,
            )}
          </uui-tab-group>
          <uui-button
            class="new-redirect"
            style=""
            look="primary"
            color="positive"
            label="Basic"
            @click="${this._openSidebar}"
          >
            <uui-icon name="add"></uui-icon>
            New redirect
          </uui-button>
        </div>`;
      } else {
        tabsOrNothing = nothing;
      }
      contentOrLoader = html`
        ${tabsOrNothing}
        <uui-scroll-container class="dashboard-body">
          <div class="dashboard-body-container">${this.activeTab?.template}</div>
        </uui-scroll-container>
        <urltracker-dashboard-footer> </urltracker-dashboard-footer>
      `;
    }

    return html`
      <div class="dashboard">
        <div class="dashboard-content">${contentOrLoader}</div>
      </div>
    `;
  }

  static styles = css`
    [popover] {
      position: fixed;
      z-index: 2147483647;
      inset: 0;
      padding: 0.25em;
      width: fit-content;
      height: fit-content;
      border: solid;
      background: canvas;
      color: canvastext;
      overflow: auto;
      margin: auto;
    }
    @supports not selector([popover]:open) {
      [popover]:not(.\:popover-open, dialog[open]) {
        display: none;
      }
      [anchor].\:popover-open {
        inset: auto;
      }
    }
    @supports not selector([popover]:popover-open) {
      [popover]:not(.\:popover-open, dialog[open]) {
        display: none;
      }
      [anchor].\:popover-open {
        inset: auto;
      }
    }

    .dashboard {
      position: absolute;
      left: 0;
      right: 0;
      top: 0;
      bottom: 0;
      padding-top: 70px;
      pointer-events: none;
    }
    .dashboard-content {
      width: 100%;
      height: 100%;
      display: flex;
      flex-direction: column;
      pointer-events: all;
    }
    .tabs-wrapper {
      background-color: white;
      border-bottom: 1px solid #e9e9eb;
      box-sizing: border-box;
      height: 70px;
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      gap: 1rem;
      padding-left: 1rem;
      padding-right: 1rem;
    }
    .dashboard-body {
      flex: 1;
      background-image: url('/app_plugins/urltracker/assets/images/background.svg');
    }
    .dashboard-body-container {
      padding: 2rem;
    }

    .new-redirect {
      margin: auto 1rem auto auto;
    }
  `;
}
