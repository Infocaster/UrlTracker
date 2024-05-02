// based on https://our.umbraco.com/apidocs/v8/ui/#/api/umbraco.services.editorService
export interface IEditorService<T extends any> {
  contentEditor(editor: IContentEditor): void;
  close: () => void;
  closeAll: () => void;
  copy: (editor: IEditor) => void;
  embed: (editor: IEditor) => void;
  focus: () => void;
  getEditors: () => IEditor[];
  open: (editor: ICustomEditor<T>) => void;
  submit: (value: T) => void;
  contentPicker: (editor: IContentPicker) => void;
}

export interface IContentPicker {
  multiPicker: boolean;
  submit: (model: { selection: IContent[] }) => void;
  close: () => void;
}

export interface IContent {
  name: string;
  id: number;
  udi: string;
  icon: string;
  trashed: boolean;
  key: string;
  parentId: number;
  alias: string;
  path: string;
  metaData: {
    ContentTypeAlias: string;
    IsPublished: boolean;
    IsContainer: boolean;
  };
}

export interface IContentEditor {
  id: string;
  create: boolean;
  submit: Function;
  close: Function;
  parentId: string;
  documentTypeAlias: string;
  allowSaveAndClose: boolean;
  allowPublishAndClose: boolean;
}

interface IEditor {
  id: string;
  create: boolean;
  submit: () => void;
  close: () => void;
  parentId: string;
  documentTypeAlias: string;
  allowSaveAndClose: boolean;
  allowPublishAndClose: boolean;
}

export interface ICustomEditor<T> {
  title: string;
  view: string;
  size: string;
  submit: (value: T) => void;
  close: () => void;
  value: T;
}
