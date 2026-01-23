import '@oddbird/popover-polyfill';
import './dashboard';

import './dashboard/main.lit';

import { TabBuilder } from './util/tools/builder/tabBuilder';

window.URL_TRACKER = {
  TabBuilder: new TabBuilder(),
};

import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { css, html } from 'lit';
import { customElement } from 'lit/decorators.js';

import './dashboard/tabs/recommendations/recommendationType/index.ts';
import './dashboard/tabs/redirects/source/index.ts';
import './dashboard/tabs/redirects/target/index.ts';

@customElement('url-tracker')
export default class UrlTracker extends UmbLitElement {
  constructor() {
    super();
  }

  render() {
    return html` <urltracker-dashboard></urltracker-dashboard> `;
  }

  static styles = css`
    :host {
      height: 100%;
    }
  `;
}
