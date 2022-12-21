import { IDashboardPageResponse } from "../extensions/dashboardPageResponse";
import { ILocalizationService } from "../umbraco/localizationService";
import { ILitEvent } from "../util/litEvent";
import { IDashboardTab } from "./dashboardTab";

export interface IUrlTrackerDashboardScope extends angular.IScope { }
export interface ITabChangeEvent extends ILitEvent<IDashboardTab> { }

export class UrlTrackerDashboardController {

    public static $inject = ["$scope", "$http", "localizationService"]

    public tabs: Array<IDashboardTab> | null = null;
    public activeTab: any | null = null;
    public loading: number = 0;

    constructor(private $http: angular.IHttpService, private localizationService: ILocalizationService) { }

    public setTab(e: ITabChangeEvent) {

        this.activeTab = e.detail;
    }

    public init(): Promise<void> {
        return this.refreshTabs();
    }

    public async refreshTabs(): Promise<void> {

        this.loading++;
        try {

            let response = await this.$http.get<IDashboardPageResponse>('/umbraco/backoffice/urltracker/extensions/dashboardpages');
            if (response.status != 200) return;

            let titleAliases = response.data.results.map((item) => "urlTrackerDashboardTabs_" + item.alias);
            let labelAliases = response.data.results.map((item) => "urlTrackerDashboardTabLabels_" + item.alias);

            let titlePromise = this.localizationService.localizeMany(titleAliases);
            let labels = await this.localizationService.localizeMany(labelAliases);
            let titles = await titlePromise;

            let result: Array<IDashboardTab> = response.data.results.map((item, index) => ({
                name: titles[index],
                label: labels[index] ? labels[index] : titles[index],
                template: item.view
            }));

            this.tabs = result;
        }
        finally {

            this.loading--;
        }
    }
}
