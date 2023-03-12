import { IPromise } from "angular";
import { IUmbRequestHelper } from "../umbraco/umbRequestHelper";
import { IControllerUrlResource, IUrlResource, UrlResource } from "../util/UrlResource";
import { IPagedCollectionResponseBase } from "./models/PagedCollectionResponseBase";

export interface IRecommendation {

    id: number;
    ignore: boolean;
    url: string;
    strategy: string;
}

export interface IRecommendationCollection extends IPagedCollectionResponseBase<IRecommendation>{

}

export interface IRecommendationsService {

    list(page: number, pageSize: number): IPromise<IRecommendationCollection>;
}

export class RecommendationsService implements IRecommendationsService {
    public static alias: string = "urlTrackerRecommendationsService";

    public static $inject = ["umbRequestHelper", UrlResource.alias, "$http"];
    constructor(private umbRequestHelper: IUmbRequestHelper, private urlResource: IUrlResource, private $http: angular.IHttpService) { }

    private get controller(): IControllerUrlResource {

        return this.urlResource.getController('recommendations');
    }

    public list(page: number, pageSize: number): IPromise<IRecommendationCollection> {

        return this.umbRequestHelper.resourcePromise(
            this.$http.get(
                this.controller.getUrl("list"),
                {
                    params: {
                        page,
                        pageSize
                    }
                }),
            "Failed to get a list of recommendations");
    }
}