import { css } from "lit";

export const errorStyle = css`
    .error {
        font-style: italic;
        color: var(--uui-color-danger);
    }
`;

export const cardWithClickableHeader = css`
    :host {
        position: relative;
      }

      h3 {
        margin: 0;
      }
      
      h3, .inspect-button {

          line-height: 20px;
          font-size: 15px;
          font-weight: 400;
      }

      .inspect-button {
        padding: 0;
        border-radius: 0;
        border: none;
        background-color: transparent;
        font-family: Lato, "Helvetica Neue", Helvetica, Arial, sans-serif;
        cursor: pointer;
      }

      .inspect-button:hover {
        text-decoration: underline;
      }

      .inspect-button::before {
        content: '';
        position: absolute;
        left: 0;
        right: 0;
        top: 0;
        bottom: 0;
        z-index: 999;
      }
`;

export const actionButton = css`

      .actions {
        gap: 16px;
        margin-top: 8px;
        height: 24px;
      }
      
      button.action-button {
        z-index: 1000;
        font-size: 12px;
        line-height: 12px;
        padding-left: 0;
        padding-right: 0;
        border-radius: 0;
        border: none;
        background-color: transparent;
        font-family: Lato, "Helvetica Neue", Helvetica, Arial, sans-serif;
        text-align: center;
        text-decoration: underline;
        cursor: pointer;
      }

      button.action-button:hover {
        text-decoration: none;
      }

      button.action-button uui-icon.icon-before {
        margin-right: 4px;
      }

      button.action-button uui-icon.icon-after {
        margin-left: 4px;
      }
`;