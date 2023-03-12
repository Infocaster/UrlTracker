import { IRecommendationCollection, IRecommendationsService, RecommendationsService } from "../../../services/Recommendations.service";
import { basePath } from "../../../util/constants";

export class UrlTrackerLandingpageRecommendations {

    public static alias: string = "urlTrackerLandingpageRecommendations";

    public collection: IRecommendationCollection | null = null;

    public static $inject = [RecommendationsService.alias];
    constructor(private recommendationsService: IRecommendationsService) { }

    public init(): void {

        this.recommendationsService.list(1, 10)
            .then((data) => {

                this.collection = data;
            });
    }
}

ngUrlTrackerLandingpageRecommendations.alias = "ngUrltrackerLandingpageRecommendations"
export function ngUrlTrackerLandingpageRecommendations(): angular.IDirective {

    return {
        restrict: 'E',
        templateUrl: basePath + '/dashboard/tabs/landingpage/recommendations.directive.html',
        controller: UrlTrackerLandingpageRecommendations.alias,
        controllerAs: 'vm'
    }
}