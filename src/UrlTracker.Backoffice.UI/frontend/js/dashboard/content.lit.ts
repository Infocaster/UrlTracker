import { IEditorService, editorServiceContext } from '@/context/editorservice.context';
import { consume, provide } from '@lit/context';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { tabContext } from '../context/tabcontext.context';
import './footer/footer.lit';
import { URL_TRACKER_SIMPLE_REDIRECT_MODAL } from './sidebars/simpleRedirect-modal.token';
import tabStrategy, { ITab, TabStrategyCollection } from './tab';

@customElement('urltracker-dashboard-content')
export class UrlTrackerDashboardContent extends UmbElementMixin(LitElement) {
  @provide({ context: tabContext })
  private _tabs?: Array<ITab>;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  #notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;

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

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });
    this.loading = 0;

    window.addEventListener('url-tracker-open-tab', ((evt: CustomEvent) => {
      if (!this.tabs) return;
      const tab = this.tabs.find((item) => item.alias === evt.detail.alias);
      if (tab) {
        this.activeTab = tab;
      }
    }) as EventListener);
  }

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this.loading++;
    try {
      const titleAliases = this.tabStrategyCollection.map((item) => item.nameKey);
      const labelAliases = this.tabStrategyCollection.map((item) => item.labelKey);

      const titles = titleAliases.map((alias) => this.localize.term(alias));
      const labels = labelAliases.map((alias) => this.localize.term(alias));

      const result: Array<ITab> = this.tabStrategyCollection.map((item, index) => ({
        name: titles[index],
        label: labels[index] ? labels[index] : titles[index],
        template: item.template,
        alias: item.alias,
        showQuickCreate: item.showQuickCreate,
      }));

      this.tabs = result;
    } finally {
      this.loading--;
    }
  }

  closePanel = () => {};

  private async _openSidebar(_: Event) {
    await umbOpenModal(this, URL_TRACKER_SIMPLE_REDIRECT_MODAL, {
      data: {
        title: this.localize.term('urlTrackerGeneral_new-redirect') || 'New redirect',
        advanced: this.activeTab?.alias === 'advancedRedirects',
        sourceEditable: true,
      },
    })
      .then(() => undefined)
      .catch(() => undefined);
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
          ${this.activeTab?.showQuickCreate
            ? html`<uui-button
                class="new-redirect"
                style=""
                look="primary"
                color="positive"
                label="Basic"
                @click="${this._openSidebar}"
              >
                <uui-icon name="add"></uui-icon>
                <umb-localize key="urlTrackerGeneral_new-redirect">New redirect</umb-localize>
              </uui-button>`
            : nothing}
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
    :host {
      height: 100%;
    }
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
      display: block;
      width: 100%;
      height: 100%;
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
      width: 100%;
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
      align-items: center;
    }
  `;
}
