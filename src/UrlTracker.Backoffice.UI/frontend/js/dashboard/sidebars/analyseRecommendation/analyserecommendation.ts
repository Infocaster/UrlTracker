import { IRecommendationResponse } from '@/services/recommendation.service';
import { ICloseEditor, ICustomEditor } from '@/umbraco/editor.service';

export interface IAnalyseRecommendationModel {
  recommendation: IRecommendationResponse;
}

export type AnalyseRecommendationEditor = ICustomEditor & ICloseEditor & IAnalyseRecommendationModel;

const editorBase: ICustomEditor = {
  view: '/App_Plugins/UrlTracker/sidebar/recommendations/analyseRecommendation.html',
  size: 'medium',
};

export function createAnalyseRecommendationEditor(
  model: ICloseEditor & IAnalyseRecommendationModel,
): AnalyseRecommendationEditor {
  return {
    ...editorBase,
    ...model,
  };
}
