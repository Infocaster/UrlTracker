import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { IDashboardTab } from "./dashboardTab";

@customElement('urltracker-dashboard')
export class UrlTrackerDashboard extends LitElement {

    constructor() {
        super();
        this.tabs = null;
        this.loading = false;
    }

    static styles = css`
        .dashboard{
            position: absolute;
            left: 0;
            right: 0;
            top: 0;
            bottom: 0;
            padding-top: 70px;
            pointer-events: none;
        }
        .dashboard-content{
            width: 100%;
            height:100%;
            display:flex;
            flex-direction: column;
            pointer-events: all;
        }
        uui-tab-group{
            --uui-tab-background: white;
            border-bottom: 1px solid #e9e9eb;
            box-sizing: border-box;
            height: 70px;
        }
        .dashboard-body{
            flex: 1;
        }
        .dashboard-body-container{
            padding: 2rem;
        }
    `;

    render() {

        let contentOrLoader;

        if (this.loading) {
            contentOrLoader = html`<uui-loader-bar animationDuration="1.5"></uui-loader-bar>`
        }
        else {
            let tabsOrNothing;
            if (this.tabs && this.tabs?.length > 1) {
                tabsOrNothing = html`
                <uui-tab-group>
                    ${this.tabs?.map((item) => html`<uui-tab label="${item.label ? item.label : item.name}" ?active="${item === this.activeTab}" @click="${this.onActivateTab(item)}">${item.name}</uui-tab>`)}
                </uui-tab-group>`
            }
            else {
                tabsOrNothing = html``;
            }
            contentOrLoader = html`
            ${tabsOrNothing}
            <uui-scroll-container class="dashboard-body">
                <div class="dashboard-body-container">
                    <slot name="tabs"></slot>
                </div>
            </uui-scroll-container>
            <urltracker-dashboard-footer>
            </urltracker-dashboard-footer>
            `;
        }

        return html`
<div class="dashboard">
    <div class="dashboard-content">
        ${contentOrLoader}
    </div>
</div>
        `;
    }

    private _tabs: Array<IDashboardTab> | null = null;

    @property({ type: Object, reflect: true })
    set tabs(tabs: Array<IDashboardTab> | null) {
        this._tabs = tabs;
        if (this._tabs && this._tabs.length > 0) {
            this.activeTab = this._tabs[0];
        }
        else {
            this.activeTab = null;
        }
    }
    get tabs() {
        return this._tabs;
    }

    private _activeTab: IDashboardTab | null = null;

    @state()
    private set activeTab(activeTab: IDashboardTab | null) {
        this._activeTab = activeTab;
        let tabEvent = new CustomEvent('tabchange', {
            detail: this._activeTab
        });
        this.dispatchEvent(tabEvent);
    }
    private get activeTab() {
        return this._activeTab;
    }

    @property({ type: Boolean, reflect: true })
    public loading: boolean;

    private onActivateTab(item: IDashboardTab): (e: Event) => void {
        return (_e: Event) => {
            this.activeTab = item;
        }
    }
}