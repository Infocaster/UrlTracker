import { ensureServiceExists } from '@/util/tools/existancecheck';
import { consume, provide } from '@lit/context';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { tabContext } from '../context/tabcontext.context';
import './footer/footer.lit';
import tabStrategy, { ITab, TabStrategyCollection } from './tab';
import { createNewRedirectOptions } from './sidebars/simpleRedirect/manageredirect';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from '@umbraco-cms/backoffice/modal';
import { URLTRACKER_EDIT_REDIRECT_MODAL } from './sidebars/simpleRedirect/manifest';

@customElement('urltracker-dashboard-content')
export default class UrlTrackerDashboardContent extends UmbElementMixin(LitElement) {
  private _modalManager?: UmbModalManagerContext;
  private get modalManager(): UmbModalManagerContext {
    ensureServiceExists(this._modalManager, 'modalManager');
    return this._modalManager;
  }

  @provide({ context: tabContext })
  private _tabs?: Array<ITab>;

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
    this.loading = 0;

    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance?: UmbModalManagerContext) => {
      this._modalManager = instance;
    });
  }

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this.loading++;
    try {
      const result: Array<ITab> = this.tabStrategyCollection.map((item, index) => ({
        name: this.localize.term(item.nameKey),
        label: this.localize.term(item.labelKey) ?? this.localize.term(item.nameKey),
        template: item.template,
      }));

      this.tabs = result;
    } finally {
      this.loading--;
    }
  }

  private async _openSidebar(_: Event) {
    const options = createNewRedirectOptions({
      title: 'New redirect',
      advanced: this.activeTab?.name === 'Advanced redirects',
    });

    const modal = this.modalManager.open(this, URLTRACKER_EDIT_REDIRECT_MODAL, {
      data: options,
    });
    try {
      await modal.onSubmit();
    } catch {
      /* Nothing to do when the promise fails */
    }
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
