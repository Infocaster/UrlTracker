import { EntryService } from './Services/entry.service'
import { ngEnterDirective, UrlTrackerOverviewController } from './Dashboards/overview.controller'
import { UrlTrackerDetailsController } from './Panels/details.controller';
import { UrlTrackerImportExportOverlayController } from './Overlays/importexport.controller';
import { UrlResource } from './Services/url.resource';

const module = angular.module("umbraco");

// controllers
module.controller("UrlTracker.OverviewController", UrlTrackerOverviewController);
module.controller("UrlTracker.DetailsController", UrlTrackerDetailsController);
module.controller("UrlTracker.ImportExportOverlayController", UrlTrackerImportExportOverlayController);

// directives
module.directive('ngEnter', ngEnterDirective);

// services
module.service("urlTrackerEntryService", EntryService);
module.service("urlTrackerUrlResource", UrlResource);