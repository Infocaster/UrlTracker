// based on https://our.umbraco.com/apidocs/v8/ui/#/api/umbraco.services.editorService
export interface IEditorService<T> {
  contentEditor(editor: IContentEditor): void;
  close: () => void;
  closeAll: () => void;
  copy: (editor: IEditor) => void;
  embed: (editor: IEditor) => void;
  focus: () => void;
  getEditors: () => IEditor[];
  open: (editor: ICustomEditor) => void;
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
  submit: () => Promise<void>;
  close: () => Promise<void>;
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

type EditorSize = 'medium' | 'small';

export interface ICustomEditor {
  view: string;
  size?: EditorSize;
  [key: string]: unknown;
}

export interface ICancelSubmitEditor<T> extends ICloseEditor {
  submit: (value: T) => void;
}

export interface ICloseEditor {
  close: () => void;
}
