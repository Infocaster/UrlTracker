import { toReadableDate } from '@/util/functions/dateformatter';
import { css, html } from '@umbraco-cms/backoffice/external/lit';
import { customElement, state } from 'lit/decorators.js';
import sourceStrategyResolver from '../../tabs/redirects/source/source.strategy';
import { RedirectResponse } from '@/api';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import { ensureExists } from '@/util/tools/existancecheck';

export const ContentElementTag = 'urltracker-sidebar-inspect-redirect';

@customElement(ContentElementTag)
export default class UrlTrackerSidebarInspectRedirect extends UmbModalBaseElement<RedirectResponse, void> {
  @state()
  private _headerText = 'Inspect Redirect';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    ensureExists(this.data, 'This modal requires data');
    const sourceStrategy = sourceStrategyResolver.getStrategy({ redirect: this.data, element: this });

    if (sourceStrategy) this._headerText = await sourceStrategy.getTitle();
  }

  close() {
    this.modalContext?.submit();
  }

  protected render() {
    ensureExists(this.data, 'Redirect data must be provided to this modal');
    return html`<div class="header">${this._headerText}</div>
      <div class="main">
        <uui-box>
          <div class="item">
            <dt>Permanent</dt>
            <dd>${this.data.permanent ? 'Yes' : 'No'}</dd>
          </div>
          <div class="item">
            <dt>Created at</dt>
            <dd>${toReadableDate(this.data.createDate)}</dd>
          </div>
          <!-- updateDate is not persisted yet in the database -->
          <!-- <div class="item">
                <dt>Last updated on</dt>
                <dd>${toReadableDate(this.data.updateDate)}</dd>
              </div> -->
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>Cancel</uui-button>
      </div>`;
  }

  static styles = css`
    :host {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }

    .header {
      display: flex;
      align-items: center;
      font-weight: 600;
      padding: 10px 20px;
      height: 2.5rem;
      background-color: white;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }

    .main {
      flex: 1;
      padding: 16px 20px;
    }

    .footer {
      display: flex;
      justify-content: flex-end;
      background-color: white;
      padding: 10px 20px;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }

    .item {
      display: grid;
      grid-template-columns: 160px 1fr;

      dt {
        font-weight: 600;
      }
    }
  `;
}
