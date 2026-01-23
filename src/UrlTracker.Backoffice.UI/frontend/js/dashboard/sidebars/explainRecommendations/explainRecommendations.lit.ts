import { IRecommendationTypeStrategies } from '@/dashboard/tabs/recommendations/recommendationType/recommendationType.constant';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalContext, UmbModalExtensionElement } from '@umbraco-cms/backoffice/modal';
import { css, html, nothing } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import variableResource from '../../../util/tools/variableresource.service';
import {
  UrlTrackerExplainRecommendationModalData,
  UrlTrackerExplainRecommendationModalValue,
} from '../explainRecommendation-modal.token';

export const ContentElementTag = 'urltracker-sidebar-inspect-recommendations';

export const RECCOMENDATION_ACTIONS = {
  MAKE_PERMANENT: 'MAKE_PERMANENT',
  MAKE_TEMPORARY: 'MAKE_TEMPORARY',
  IGNORE: 'IGNORE',
} as const;

export type IRecommendationAction = (typeof RECCOMENDATION_ACTIONS)[keyof typeof RECCOMENDATION_ACTIONS];

@customElement(ContentElementTag)
export class UrlTrackerSidebarRecommendations
  extends UmbLitElement
  implements
    UmbModalExtensionElement<UrlTrackerExplainRecommendationModalData, UrlTrackerExplainRecommendationModalValue>
{
  @property({ attribute: false })
  modalContext?: UmbModalContext<UrlTrackerExplainRecommendationModalData, UrlTrackerExplainRecommendationModalValue>;

  @property({ attribute: false })
  data?: UrlTrackerExplainRecommendationModalData;

  save(action: IRecommendationAction = RECCOMENDATION_ACTIONS.IGNORE) {
    this.modalContext?.setValue({ type: action });
    this.modalContext?.submit();
  }

  close() {
    this.modalContext?.reject();
  }

  private canShowRedirectOptions(): boolean {
    const strategyTypes = variableResource.get<IRecommendationTypeStrategies>('recommendationTypeStrategies');
    // const currentType = this.explainRecommendationsScope.model.recommendation.strategy;
    const currentType = strategyTypes.file;
    if (currentType === strategyTypes.image || currentType === strategyTypes.technicalFile) {
      return false;
    }

    return true;
  }

  private renderTemporaryRedirect() {
    if (!this.canShowRedirectOptions()) return nothing;
    return html`
      <uui-box .headline="${this.localize.term('urlTrackerExplainRecommendation_create')}">
        <p>
          <umb-localize key="urlTrackerExplainRecommendation_temporary-redirect-description">
            A temporary redirect will redirect users to a different page, but will also tell google and other search
            engines that the content on this URL will be back later. Use this option if content is only temporarily
            moved to a different URL.
          </umb-localize>
        </p>
        <span>
          <umb-localize key="urlTrackerGeneral_example-usage">Example usage</umb-localize>
        </span>
        <ul>
          <li>
            <umb-localize key="urlTrackerExplainRecommendation_campaign"
              >You run a campaign but it’s momentarily suspended and will be continued next month or year</umb-localize
            >
          </li>
        </ul>
        <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.MAKE_TEMPORARY)}
          ><umb-localize key="urlTrackerExplainRecommendation_apply">Apply</umb-localize></uui-button
        >
      </uui-box>
    `;
  }

  private renderPermanentRedirect() {
    if (!this.canShowRedirectOptions()) return nothing;
    return html`
      <uui-box .headline="${this.localize.term('urlTrackerExplainRecommendation_create-permanent')}">
        <p>
          <umb-localize key="urlTrackerExplainRecommendation_create-permanent-description">
            A permanent redirect will redirect users to a different page, but will also tell google and other search
            engines that the current URL is no longer relevant. Use this option if content is moved to a different URL
            forever.
          </umb-localize>
        </p>
        <span>
          <umb-localize key="urlTrackerGeneral_example-usage">Example usage</umb-localize>
        </span>
        <ul>
          <li>
            <umb-localize key="urlTrackerExplainRecommendation_blogs"
              >You used to post your blogs on /news, but they are now found below /blogs</umb-localize
            >
          </li>
          <li>
            <umb-localize key="urlTrackerExplainRecommendation_image"
              >You rely on an image in a social media post, but the image no longer exists or has moved</umb-localize
            >
          </li>
        </ul>
        <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.MAKE_PERMANENT)}
          ><umb-localize key="urlTrackerExplainRecommendation_apply">Apply</umb-localize></uui-button
        >
      </uui-box>
    `;
  }

  private renderIgnore() {
    return html`
      <uui-box .headline="${this.localize.term('urlTrackerExplainRecommendation_ignore')}">
        <p>
          <umb-localize key="urlTrackerExplainRecommendation_ignore-description">
            Sometimes a url might pop up in here that you simply cannot do anything with. In that case, you can ignore
            the recommendation and it will be permanently removed from the overview.
          </umb-localize>
        </p>
        <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.IGNORE)}
          ><umb-localize key="urlTrackerExplainRecommendation_apply">Apply</umb-localize></uui-button
        >
      </uui-box>
    `;
  }

  protected render() {
    return html`
      <div class="header">
        ${this.localize.term('urlTrackerExplainRecommendation_header') + this.data?.recommendation.url}
      </div>
      <div class="main">${this.renderTemporaryRedirect()} ${this.renderPermanentRedirect()} ${this.renderIgnore()}</div>
      <div class="footer">
        <uui-button look="default" color="default" @click=${this.close}>
          <umb-localize key="urlTrackerGeneral_close">Close</umb-localize>
        </uui-button>
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

export const element = UrlTrackerSidebarRecommendations;
