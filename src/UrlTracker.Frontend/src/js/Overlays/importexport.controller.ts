export class UrlTrackerImportExportOverlayController {

	public mode: string;
	public title: string;
	public file: any;
	public fileInputDisabled: boolean;
	public export: any;
	public import: any;

	public static $inject = ["$scope", "urlTrackerEntryService", 'notificationsService']
	constructor(private $scope: any, private urlTrackerEntryService: urltracker.services.IEntryService, private notificationsService: any) {

		this.mode = "";
		this.title = "";
		this.file = null;
		this.fileInputDisabled = false;

		this.export = {
			loading: false
		};

		this.import = {
			loading: false
		};
	}

	public exportRedirects(): void {
		if (!this.export.loading) {
			this.export.loading = true;

			this.urlTrackerEntryService.exportRedirects().then(() => {
				this.export.loading = false;
			});

		}
	}

	public importRedirects(): void {
		this.mode = "import";
		this.title = "Import redirects";
	}

	public back(): void {
		if (this.mode == "import" && this.import.loading == true)
			return;

		this.mode = "";
		this.title = "";
		this.file = null;
	}

	public submitImport(): void {
		if (this.file && !this.import.loading) {
			this.import.loading = true;
			this.fileInputDisabled = true;

			this.urlTrackerEntryService.importRedirects(this.file).then((response) => {
				this.notificationsService.success("Success", `Succesfully imported ${response} redirects`);
				this.import.loading = false;
				this.fileInputDisabled = false;
				this.$scope.model.refreshRedirects();
			}).catch((response) => {
				this.notificationsService.error("Error", response.data.Message);
				this.import.loading = false;
				this.fileInputDisabled = false;
			})
		}
	}
}