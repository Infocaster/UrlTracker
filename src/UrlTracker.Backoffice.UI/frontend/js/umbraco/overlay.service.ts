export interface IOverlayService {
  confirmDelete(overlay: IConfirmDeleteOverlay): void;
}

interface IConfirmDeleteOverlay {
  closeButtonLabelKey?: string;
  view?: string;
  confirmMessageStyle?: string;
  submitButtonStyle?: string;
  submitButtonLabelKey?: string;
  // eslint-disable-next-line @typescript-eslint/ban-types
  close?: Function;
  // eslint-disable-next-line @typescript-eslint/ban-types
  submit?: Function;
}
