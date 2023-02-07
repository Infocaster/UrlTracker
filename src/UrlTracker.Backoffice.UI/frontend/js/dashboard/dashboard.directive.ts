import { IDashboardPageResponse } from "../extensions/dashboardPageResponse";
import { ILocalizationService } from "../umbraco/localizationService";
import { ILitEvent } from "../util/litEvent";
import { IDashboardFooter } from "./footer/dashboardFooter";
import { IDashboardTab } from "./dashboardTab";
import { IVersionProvider, VersionProvider } from "../util/versionProvider";

export interface IUrlTrackerDashboardScope extends angular.IScope { }
export interface ITabChangeEvent extends ILitEvent<IDashboardTab> { }

export class UrlTrackerDashboardController {

    public static alias = "UrlTracker.Dashboard.Controller";

    public tabs: Array<IDashboardTab> | null = null;
    public activeTab: IDashboardTab | null = null;
    public footer: IDashboardFooter | null = null;

    public loading: number = 0;

    public static $inject = ["$http", "localizationService", VersionProvider.alias];
    constructor(private $http: angular.IHttpService, private localizationService: ILocalizationService, private versionProvider: IVersionProvider) { }

    public setTab(e: ITabChangeEvent) {

        this.activeTab = e.detail;
    }

    public async init(): Promise<void> {
        await Promise.all([
            this.refreshTabs(),
            this.refreshFooter()
        ]);
    }

    private async refreshTabs(): Promise<void> {

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

    private async refreshFooter(): Promise<void> {

        this.footer = {
            logo: "/App_Plugins/UrlTracker/assets/layout/footerlogo.svg",
            logoUrl: "https://infocaster.net",
            version: this.versionProvider.version,
            links: [
                {
                    url: "https://github.com/Infocaster/UrlTracker/Discussions",
                    title: "Suggest a feature",
                    target: "_blank"
                },
                {
                    url: "https://github.com/Infocaster/UrlTracker/Issues",
                    title: "Report a bug",
                    target: "_blank"
                }
            ]
        };
    }
}

ngUrltrackerDashboard.alias = "ngUrltrackerDashboard";
export function ngUrltrackerDashboard(): angular.IDirective {

    return {
        restrict: 'E',
        templateUrl: '/App_Plugins/UrlTracker/dashboard/dashboard.directive.html',
        controller: UrlTrackerDashboardController.alias,
        controllerAs: 'vm'
    };
}