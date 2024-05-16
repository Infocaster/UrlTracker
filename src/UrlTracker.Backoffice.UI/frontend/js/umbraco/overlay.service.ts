export interface IOverlayService {

    confirmDelete(overlay: IConfirmDeleteOverlay): void;
}

interface IConfirmDeleteOverlay {
    closeButtonLabelKey?: string
    view?: string
    confirmMessageStyle?: string
    submitButtonStyle?: string
    submitButtonLabelKey?: string
    close?: Function,
    submit?: Function
}