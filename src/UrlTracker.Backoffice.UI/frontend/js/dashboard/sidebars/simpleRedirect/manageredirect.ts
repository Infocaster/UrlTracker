import { ICancelSubmitEditor, ICustomEditor } from '@/umbraco/editor.service';
import type { RedirectRequest, RedirectResponse } from '../../../../../api-client/types.gen';

export interface IManageRedirectModel {
  advanced: boolean;
  title: string;
  sourceEditable: boolean;
}

export interface ICreateRedirectModel {
  solvedRecommendation?: number;
  data?: RedirectRequest;
}

export interface IUpdateRedirectModel {
  id: number;
  data: RedirectRequest;
}

export type ManageRedirectEditor = ICustomEditor &
  ICancelSubmitEditor<RedirectResponse> &
  IManageRedirectModel &
  ICreateRedirectModel & { id?: number };

const editorBase: ICustomEditor = {
  view: '/App_Plugins/UrlTracker/sidebar/redirect/simpleRedirect.html',
  size: 'medium',
};

export function createNewRedirectOptions(
  model: IManageRedirectModel & ICreateRedirectModel & ICancelSubmitEditor<RedirectResponse>,
): ManageRedirectEditor {
  return {
    ...editorBase,
    ...model,
  };
}

export function createEditRedirectOptions(
  model: IManageRedirectModel & IUpdateRedirectModel & ICancelSubmitEditor<RedirectResponse>,
): ManageRedirectEditor {
  return {
    ...editorBase,
    ...model,
  };
}
