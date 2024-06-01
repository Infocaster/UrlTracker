import { ILocalizationService, localizationServiceContext } from '@/context/localizationservice.context';
import { scopeContext } from '@/context/scope.context';
import { IRedirectResponse } from '@/services/redirect.service';
import { toReadableDate } from '@/util/functions/dateformatter';
import { ensureExists } from '@/util/tools/existancecheck';
import { consume } from '@lit/context';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { InspectRedirectScope } from './scope';
import sourceStrategyResolver from '../../tabs/redirects/source/source.strategy';

export const ContentElementTag = 'urltracker-sidebar-inspect-redirect';

@customElement(ContentElementTag)
export class UrlTrackerSidebarInspectRedirect extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @consume({ context: scopeContext })
  private $scope?: InspectRedirectScope;

  @property({ attribute: false })
  get scope() {
    ensureExists(this.$scope, 'scope');
    return this.$scope;
  }

  @state()
  private data!: IRedirectResponse;

  @state()
  private _headerText = 'Inspect Redirect';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this.data = this.scope.model.redirect;

    const sourceStrategy = sourceStrategyResolver.getStrategy({ redirect: this.data, element: this });

    if (sourceStrategy) this._headerText = await sourceStrategy.getTitle();
  }

  close() {
    this.scope.model.close();
  }

  protected render() {
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
