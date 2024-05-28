import { IRecommendationResponse } from "@/services/recommendation.service";
import { ICancelSubmitEditor, ICustomEditor } from "@/umbraco/editor.service";
import { IRecommendationAction } from "./explainRecommendations.lit";

export interface IExplainRecommendationsModel {
    recommendation: IRecommendationResponse
}

export type ExplainRecommendationsEditor = ICustomEditor & ICancelSubmitEditor<IRecommendationAction> & IExplainRecommendationsModel;

const editorBase : ICustomEditor = {
    view: "/App_Plugins/UrlTracker/sidebar/recommendations/inspectRecommendations.html",
    size: "medium",
}

export function createExplainRecommendationsEditor(model: ICancelSubmitEditor<IRecommendationAction> & IExplainRecommendationsModel): ExplainRecommendationsEditor {
    return {
        ...editorBase,
        ...model
    }
}