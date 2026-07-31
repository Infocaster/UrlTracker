import { toReadableDate } from '@/util/functions/dateformatter';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalContext, UmbModalExtensionElement } from '@umbraco-cms/backoffice/modal';
import { css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import type { RedirectResponse } from '../../../api-client/types.gen';
import sourceStrategyResolver from '../../tabs/redirects/source/source.strategy';
import {
  UrlTrackerInspectRedirectModalData,
  UrlTrackerInspectRedirectModalValue,
} from '../inspectRedirect-modal.token';

export const ContentElementTag = 'urltracker-sidebar-inspect-redirect';

@customElement(ContentElementTag)
export class UrlTrackerSidebarInspectRedirect
  extends UmbLitElement
  implements UmbModalExtensionElement<UrlTrackerInspectRedirectModalData, UrlTrackerInspectRedirectModalValue>
{
  @state()
  private _headerText = 'Inspect Redirect';

  @property({ attribute: false })
  modalContext?: UmbModalContext<UrlTrackerInspectRedirectModalData, UrlTrackerInspectRedirectModalValue>;

  @property({ attribute: false })
  data?: UrlTrackerInspectRedirectModalData;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    const sourceStrategy = sourceStrategyResolver.getStrategy({
      redirect: this.data?.redirect! as unknown as RedirectResponse,
      element: this,
    });

    if (sourceStrategy) this._headerText = await sourceStrategy.getTitle();
  }

  close() {
    this.modalContext?.reject();
  }

  protected render() {
    return html`<div class="header">${this._headerText}</div>
      <div class="main">
        <uui-box>
          <div class="item">
            <dt>
              <umb-localize key="urlTrackerGeneral_permanent">Permanent</umb-localize>
            </dt>
            <dd>
              ${this.data?.redirect?.permanent
                ? this.localize.term('urlTrackerGeneral_yes')
                : this.localize.term('urlTrackerGeneral_no')}
            </dd>
          </div>
          <div class="item">
            <dt>
              <umb-localize key="urlTrackerGeneral_created-at">Created at</umb-localize>
            </dt>
            <dd>${toReadableDate(new Date(this.data?.redirect?.createDate!))}</dd>
          </div>
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>
          <umb-localize key="urlTrackerGeneral_cancel">Cancel</umb-localize>
        </uui-button>
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

export const element = UrlTrackerSidebarInspectRedirect;
