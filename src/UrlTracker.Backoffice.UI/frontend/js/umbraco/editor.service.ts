export interface IEditorService {
    contentEditor(editor: IContentEditor): void;
    close(): void;
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