import { RedirectRequest } from '@/api';

export type IManageRedirectModel = ICreateRedirectModel &
  IRedirectEditorConfig & {
    id?: number;
  };

export interface IRedirectEditorConfig {
  advanced: boolean;
  title: string;
}

export interface ICreateRedirectModel {
  solvedRecommendation?: number;
  data?: RedirectRequest;
}

export interface IUpdateRedirectModel {
  id: number;
  data: RedirectRequest;
}

export function createNewRedirectOptions(model: ICreateRedirectModel & IRedirectEditorConfig): IManageRedirectModel {
  return {
    ...model,
  };
}

export function createEditRedirectOptions(model: IUpdateRedirectModel & IRedirectEditorConfig): IManageRedirectModel {
  return {
    ...model,
  };
}
