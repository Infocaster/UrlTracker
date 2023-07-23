import { LitElement, css, html, nothing } from "lit";
import tabStrategy, { ITab, TabStrategyCollection } from "./tab";
import { customElement, state } from "lit/decorators.js";
import { consume } from "@lit-labs/context";
import { ILocalizationService } from "../umbraco/localization.service";
import { localizationServiceContext } from "../context/localizationservice.context";
import './footer/footer.lit'

@customElement("urltracker-dashboard-content")
export class UrlTrackerDashboardContent extends LitElement{

    private _tabs?: Array<ITab>;

    @state()
    set tabs(tabs: Array<ITab> | undefined) {
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
    private activeTab: ITab | null = null;

    @state()
    public loading: number;

    private tabStrategyCollection: TabStrategyCollection = tabStrategy;

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

            if (!this.localizationService) throw new Error("localization service is not defined, but is required by this element");
            
            let titleAliases = this.tabStrategyCollection.map((item) => item.nameKey);
            let labelAliases = this.tabStrategyCollection.map((item) => item.labelKey);
            
            let titlePromise = this.localizationService.localizeMany(titleAliases);
            let labels = await this.localizationService.localizeMany(labelAliases);
            let titles = await titlePromise;
            
            let result: Array<ITab> = this.tabStrategyCollection.map((item, index) => ({
                name: titles[index],
                label: labels[index] ? labels[index] : titles[index],
                template: item.template
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
                        ${this.activeTab?.template}
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
            background-image: url('/app_plugins/urltracker/assets/images/background.svg');
        }
        .dashboard-body-container{
            padding: 2rem;
        }
    `;
}