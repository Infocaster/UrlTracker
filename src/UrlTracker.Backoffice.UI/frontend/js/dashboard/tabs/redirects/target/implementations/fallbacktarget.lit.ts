import { LitElement, css } from 'lit';
import { UrlTrackerRedirectTarget } from '../targetbase.mixin';
import { customElement } from 'lit/decorators.js';

const baseType = UrlTrackerRedirectTarget(LitElement, 'urlTrackerRedirectTarget_unknown');

@customElement('urltracker-redirect-target-unknown')
export class UrlTrackerUnknownRedirectTarget extends baseType {
  static styles = [
    ...baseType.styles,
    css`
      :host,
      span {
        font-style: italic;
        color: var(--uui-color-danger);
      }
    `,
  ];
}
