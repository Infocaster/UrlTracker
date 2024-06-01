import { IRedirectData, IRedirectResponse } from '@/services/redirect.service';
import { ICancelSubmitEditor, ICustomEditor } from '@/umbraco/editor.service';

export interface IManageRedirectModel {
  advanced: boolean;
  title: string;
}

export interface ICreateRedirectModel {
  solvedRecommendation?: number;
  data?: IRedirectData;
}

export interface IUpdateRedirectModel {
  id: number;
  data: IRedirectData;
}

export type ManageRedirectEditor = ICustomEditor &
  ICancelSubmitEditor<IRedirectResponse> &
  IManageRedirectModel &
  ICreateRedirectModel & { id?: number };

const editorBase: ICustomEditor = {
  view: '/App_Plugins/UrlTracker/sidebar/redirect/simpleRedirect.html',
  size: 'medium',
};

export function createNewRedirectOptions(
  model: IManageRedirectModel & ICreateRedirectModel & ICancelSubmitEditor<IRedirectResponse>,
): ManageRedirectEditor {
  return {
    ...editorBase,
    ...model,
  };
}

export function createEditRedirectOptions(
  model: IManageRedirectModel & IUpdateRedirectModel & ICancelSubmitEditor<IRedirectResponse>,
): ManageRedirectEditor {
  return {
    ...editorBase,
    ...model,
  };
}
