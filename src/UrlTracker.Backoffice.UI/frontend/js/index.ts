import { EntryService } from './Services/entry.service'
import { UrlTrackerDashboardController } from './dashboard/dashboard.controller'
import { UrlTrackerDetailsController } from './Panels/details.controller';
import { UrlTrackerImportExportOverlayController } from './Overlays/importexport.controller';
import { UrlResource } from './Services/url.resource';
import { ngEnterDirective } from './keyboard/enter.directive'

const module = angular.module("umbraco");

// controllers
module.controller("UrlTracker.Dashboard.Controller", UrlTrackerDashboardController);
module.controller("UrlTracker.DetailsController", UrlTrackerDetailsController);
module.controller("UrlTracker.ImportExportOverlayController", UrlTrackerImportExportOverlayController);

// directives
module.directive('ngEnter', ngEnterDirective);

// services
module.service("urlTrackerEntryService", EntryService);
module.service("urlTrackerUrlResource", UrlResource);