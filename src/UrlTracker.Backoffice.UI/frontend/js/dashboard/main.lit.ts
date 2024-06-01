import { recommendationsAnalysisServiceContext } from '@/context/recommendationsanalysis.context';
import recommendationsAnalysisService, {
  RecommendationsAnalysisService,
} from '@/services/recommendationanalysis.service';
import { provide } from '@lit/context';
import { html, LitElement } from 'lit';
import { customElement } from 'lit/decorators.js';
import { landingpageServiceContext } from '../context/landingspageservice.context';
import { UrlTrackerMainContext } from '../context/maincontext.mixin';
import { notificationServiceContext } from '../context/notificationservice.context';
import { recommendationServiceContext } from '../context/recommendationservice.context';
import { IRedirectService, redirectServiceContext } from '../context/redirectservice.context';
import { ITargetService, redirectTargetServiceContext } from '../context/redirecttargetservice.context';
import { versionProviderContext } from '../context/versionprovider.context';
import landingspageService, { ILandingspageService } from '../services/landingspage.service';
import recommendationService, { IRecommendationsService } from '../services/recommendation.service';
import redirectService from '../services/redirect.service';
import redirectImportService, { IRedirectImportService } from '../services/redirectimport.service';
import '../util/elements/angulariconregistry.lit';
import versionProvider, { IVersionProvider } from '../util/tools/versionprovider.service';
import './content.lit';
import notificationService, { INotificationService } from './notifications/notification.service';
import targetService from './tabs/redirects/target/target.service';

//Sidebar imports
import { redirectImportServiceContext } from '@/context/redirectimportservice.context';
import '@sidebar/analyseRecommendation-main.lit';
import '@sidebar/explainRecommendations-main.lit';
import '@sidebar/inspectRedirect-main.lit';
import '@sidebar/simpleRedirect-main.lit';

@customElement('urltracker-dashboard')
export class UrlTrackerDashboard extends UrlTrackerMainContext(LitElement) {
  @provide({ context: notificationServiceContext })
  notificationService: INotificationService = notificationService;

  @provide({ context: recommendationServiceContext })
  recommendationService: IRecommendationsService = recommendationService;

  @provide({ context: versionProviderContext })
  versionProvider: IVersionProvider = versionProvider;

  @provide({ context: redirectServiceContext })
  redirectService: IRedirectService = redirectService;

  @provide({ context: redirectImportServiceContext })
  redirectImportService: IRedirectImportService = redirectImportService;

  @provide({ context: redirectTargetServiceContext })
  redirectTargetService: ITargetService = targetService;

  @provide({ context: landingpageServiceContext })
  landingspageService: ILandingspageService = landingspageService;

  @provide({ context: recommendationsAnalysisServiceContext })
  recommendationsAnalysisService: RecommendationsAnalysisService = recommendationsAnalysisService;

  protected render(): unknown {
    return html`
      <urltracker-angular-icon-registry>
        <urltracker-dashboard-content></urltracker-dashboard-content>
      </urltracker-angular-icon-registry>
    `;
  }
}
