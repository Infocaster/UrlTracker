import { css } from 'lit';

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

  h3,
  .inspect-button {
    line-height: 20px;
    font-size: 15px;
    font-weight: 400;
    text-align: start;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .inspect-button {
    padding: 0;
    border-radius: 0;
    border: none;
    background-color: transparent;
    font-family: Lato, 'Helvetica Neue', Helvetica, Arial, sans-serif;
    cursor: pointer;
    max-width: 100%;
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
    margin-top: 8px;
    height: 24px;
  }

  button.action-button:first-child {
    padding-left: 0;
  }

  button.action-button:last-child {
    padding-right: 0;
  }

  button.action-button {
    z-index: 1000;
    font-size: 12px;
    line-height: 12px;
    padding-left: 8;
    padding-right: 8;
    border-radius: 0;
    border: none;
    background-color: transparent;
    font-family: Lato, 'Helvetica Neue', Helvetica, Arial, sans-serif;
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

export const colors = css`
  uui-icon {
    fill: var(--uui-icon-color);
  }

  .color-black {
    --uui-icon-color: #000;
  }

  .color-blue-grey {
    --uui-icon-color: #607d8b;
  }

  .color-grey {
    --uui-icon-color: #9e9e9e;
  }

  .color-brown {
    --uui-icon-color: #795548;
  }

  .color-blue {
    --uui-icon-color: #2196f3;
  }

  .color-light-blue {
    --uui-icon-color: #03a9f4;
  }

  .color-indigo {
    --uui-icon-color: #3f51b5;
  }

  .color-purple {
    --uui-icon-color: #9c27b0;
  }

  .color-deep-purple {
    --uui-icon-color: #673ab7;
  }

  .color-cyan {
    --uui-icon-color: #00bcd4;
  }

  .color-green {
    --uui-icon-color: #4caf50;
  }

  .color-light-green {
    --uui-icon-color: #8bc34a;
  }

  .color-lime {
    --uui-icon-color: #cddc39;
  }

  .color-yellow {
    --uui-icon-color: #ffeb3b;
  }

  .color-amber {
    --uui-icon-color: #ffc107;
  }

  .color-orange {
    --uui-icon-color: #ff9800;
  }

  .color-deep-orange {
    --uui-icon-color: #ff5722;
  }

  .color-red {
    --uui-icon-color: #f44336;
  }

  .color-pink {
    --uui-icon-color: #e91e63;
  }
`;
