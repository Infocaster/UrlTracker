import { LitElement, css, html, nothing } from "lit";
import { TabService } from "../services/tabs.service";
import { IDashboardTab } from "./tab";
import { customElement, state } from "lit/decorators.js";
import { consume } from "@lit-labs/context";
import { tabServiceContext } from "../context/tabservice.context";
import { ILocalizationService } from "../umbraco/localizationService";
import { localizationServiceContext } from "../context/localizationservice.context";
import { unsafeHTML } from "lit/directives/unsafe-html.js";
import './footer/footer.lit'

@customElement("urltracker-dashboard-content")
export class UrlTrackerDashboardContent extends LitElement{

    private _tabs?: Array<IDashboardTab>;

    @state()
    set tabs(tabs: Array<IDashboardTab> | undefined) {
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

    @state()
    private activeTab: IDashboardTab | null = null;

    @state()
    public loading: number;

    @consume({context: tabServiceContext})
    public tabService?: TabService;

    @consume({context: localizationServiceContext})
    public localizationService?: ILocalizationService;

    constructor() {
        super();
        this.loading = 0;
    }

    async connectedCallback(): Promise<void> {
        
        super.connectedCallback();

        this.loading++;
        try{

            if (!this.tabService) throw Error("Tab service is not defined, but is required by this element.");
            if (!this.localizationService) throw Error("localization service is not defined, but is required by this element");
            
            let response = await this.tabService.GetTabs();
            
            let titleAliases = response.results.map((item) => "urlTrackerDashboardTabs_" + item.alias);
            let labelAliases = response.results.map((item) => "urlTrackerDashboardTabLabels_" + item.alias);
            
            let titlePromise = this.localizationService.localizeMany(titleAliases);
            let labels = await this.localizationService.localizeMany(labelAliases);
            let titles = await titlePromise;
            
            let result: Array<IDashboardTab> = response.results.map((item, index) => ({
                name: titles[index],
                label: labels[index] ? labels[index] : titles[index],
                template: item.view.replace(/[^A-Za-z0-9\-]/g, '')
            }));
            
            this.tabs = result;
        }
        finally{
            this.loading--;
        }
    }

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
                    ${this.tabs?.map((item) => html`<uui-tab label="${item.label ? item.label : item.name}" ?active="${item === this.activeTab}" @click="${() => this.activeTab = item}">${item.name}</uui-tab>`)}
                </uui-tab-group>`
            }
            else {
                tabsOrNothing = nothing;
            }
            contentOrLoader = html`
            ${tabsOrNothing}
            <uui-scroll-container class="dashboard-body">
                <div class="dashboard-body-container">
                    ${unsafeHTML(`<${this.activeTab?.template}></${this.activeTab?.template}>`)}
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
}