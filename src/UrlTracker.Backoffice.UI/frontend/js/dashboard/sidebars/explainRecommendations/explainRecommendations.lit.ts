import { css, html } from '@umbraco-cms/backoffice/external/lit';
import { customElement, state } from 'lit/decorators.js';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import { IExplainRecommendationsModel, IRecommendationAction, RECCOMENDATION_ACTIONS } from './explainrecommendations';

export const ContentElementTag = 'urltracker-sidebar-inspect-recommendations';

@customElement(ContentElementTag)
export default class UrlTrackerSidebarRecommendations extends UmbModalBaseElement<
  IExplainRecommendationsModel,
  IRecommendationAction
> {
  @state()
  private _headerText = '';

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this._headerText = 'Recommendations for: ' + this.data?.recommendation.url;
  }

  save(action: IRecommendationAction = RECCOMENDATION_ACTIONS.IGNORE) {
    this.modalContext?.setValue(action);
    this.modalContext?.submit();
  }

  close() {
    this.modalContext?.reject();
  }

  protected render() {
    return html`
      <div class="header">${this._headerText}</div>
      <div class="main">
        <uui-box headline="Create a temporary redirect">
          <p>
            A temporary redirect will redirect users to a different page, but will also tell google and other search
            engines that the content on this URL will be back later. Use this option if content is only temporarily
            moved to a different URL.
          </p>
          <span>Example usage:</span>
          <ul>
            <li>You run a campaign but it’s momentarily suspended and will be continued next month or year</li>
          </ul>
          <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.MAKE_TEMPORARY)}
            >Apply this recommendation</uui-button
          >
        </uui-box>
        <uui-box headline="Create a permanent redirect">
          <p>
            A permanent redirect will redirect users to a different page, but will also tell google and other search
            engines that the current URL is no longer relevant. Use this option if content is moved to a different URL
            forever.
          </p>
          <span>Example usage:</span>
          <ul>
            <li>You used to post your blogs on /news, but they are now found below /blogs</li>
            <li>You rely on an image in a social media post, but the image no longer exists or has moved</li>
          </ul>
          <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.MAKE_PERMANENT)}
            >Apply this recommendation</uui-button
          >
        </uui-box>
        <uui-box headline="Ignore this">
          <p>
            Sometimes a url might pop up in here that you simply cannot do anything with. In that case, you can ignore
            the recommendation and it will be permanently removed from the overview.
          </p>
          <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.IGNORE)}
            >Apply this recommendation</uui-button
          >
        </uui-box>
      </div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>Close</uui-button>
      </div>
    `;
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

    uui-box {
      margin-bottom: 1rem;
      font-family: lato, sans-serif;
      font-weight: 400;
      font-size: 15px;
      line-height: 1.25;
    }

    ul,
    p {
      margin-top: 0;
    }

    uui-box uui-button {
      width: 100%;
    }

    .footer {
      display: flex;
      justify-content: flex-end;
      background-color: white;
      padding: 10px 20px;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }
  `;
}
