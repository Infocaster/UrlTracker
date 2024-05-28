import { IRedirectResponse } from "@/services/redirect.service";
import { ICloseEditor, ICustomEditor } from "@/umbraco/editor.service";

export interface IInspectRedirectModel {

    redirect: IRedirectResponse
}

export type InspectRedirectEditor = ICustomEditor & IInspectRedirectModel & ICloseEditor

const editorBase: ICustomEditor = {
    
    view: "/App_Plugins/UrlTracker/sidebar/redirect/inspectRedirect.html",
    size: 'medium'
}

export function createInspectRedirectEditor(model: IInspectRedirectModel & ICloseEditor): InspectRedirectEditor {
    return {
        ...editorBase,
        ...model
    }
}