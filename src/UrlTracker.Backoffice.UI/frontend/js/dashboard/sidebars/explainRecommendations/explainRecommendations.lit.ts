import { ILocalizationService, localizationServiceContext } from '@/context/localizationservice.context';
import { scopeContext } from '@/context/scope.context';
import { IRecommendationTypeStrategies } from '@/dashboard/tabs/recommendations/recommendationType/recommendationType.constant';
import { ensureExists } from '@/util/tools/existancecheck';
import { consume } from '@lit/context';
import { Task } from '@lit/task';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import variableResource from '../../../util/tools/variableresource.service';
import { ExplainRecommendationsScope } from './scope';

export const ContentElementTag = 'urltracker-sidebar-inspect-recommendations';

export const RECCOMENDATION_ACTIONS = {
  MAKE_PERMANENT: 'MAKE_PERMANENT',
  MAKE_TEMPORARY: 'MAKE_TEMPORARY',
  IGNORE: 'IGNORE',
} as const;

export type IRecommendationAction = (typeof RECCOMENDATION_ACTIONS)[keyof typeof RECCOMENDATION_ACTIONS];

type Translations = {
  create: string;
  temporaryRedirectDescription: string;
  campaign: string;
  apply: string;
  createPermanent: string;
  createPermanentDescription: string;
  blogs: string;
  image: string;
  ignore: string;
  ignoreDescription: string;
  exampleUsage: string;
  close: string;
};

@customElement(ContentElementTag)
export class UrlTrackerSidebarRecommendations extends LitElement {
  @consume({ context: scopeContext })
  private $scope?: ExplainRecommendationsScope;

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @property({ attribute: false })
  get scope() {
    ensureExists(this.$scope, 'scope');
    return this.$scope;
  }

  @state()
  private _headerText = '';

  @state()
  private translationTaskKey: number = 0;

  @state()
  private translations: Partial<Translations> = {};

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this._headerText = 'Recommendations for: ' + this.$scope?.model.recommendation.url;
  }

  save(action: IRecommendationAction = RECCOMENDATION_ACTIONS.IGNORE) {
    this.scope.model.submit(action);
  }

  close() {
    this.scope.model.close();
  }

  private canShowRedirectOptions(): boolean {
    const strategyTypes = variableResource.get<IRecommendationTypeStrategies>('recommendationTypeStrategies');
    const currentType = this.scope.model.recommendation.strategy;
    if (currentType === strategyTypes.image || currentType === strategyTypes.technicalFile) {
      return false;
    }

    return true;
  }

  private _translationTask = new Task(this, {
    task: async (): Promise<Partial<Translations>> => {
      const [
        create,
        temporaryRedirectDescription,
        campaign,
        apply,
        createPermanent,
        createPermanentDescription,
        blogs,
        image,
        ignore,
        ignoreDescription,
        exampleUsage,
        close,
      ] = await Promise.all([
        this.localizationService?.localize('urlTrackerExplainRecommendation_create'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_temporary-redirect-description'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_campaign'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_apply'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_create-permanent'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_create-permanent-description'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_blogs'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_image'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_ignore'),
        this.localizationService?.localize('urlTrackerExplainRecommendation_ignore-description'),
        this.localizationService?.localize('urlTrackerGeneral_example-usage'),
        this.localizationService?.localize('urlTrackerGeneral_close'),
      ]);

      const translations: Partial<Translations> = {
        create,
        temporaryRedirectDescription,
        campaign,
        apply,
        createPermanent,
        createPermanentDescription,
        blogs,
        image,
        ignore,
        ignoreDescription,
        exampleUsage,
        close,
      };

      this.translations = translations;

      return translations;
    },
    args: () => [this.translationTaskKey],
  });

  private renderTemporaryRedirect() {
    if (!this.canShowRedirectOptions()) return nothing;
    return html`
      <uui-box .headline="${this.translations.create}">
        <p>${this.translations.temporaryRedirectDescription}</p>
        <span>${this.translations.exampleUsage}</span>
        <ul>
          <li>${this.translations.campaign}</li>
        </ul>
        <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.MAKE_TEMPORARY)}
          >${this.translations.apply}</uui-button
        >
      </uui-box>
    `;
  }

  private renderPermanentRedirect() {
    if (!this.canShowRedirectOptions()) return nothing;
    return html`
      <uui-box .headline="${this.translations.createPermanent}">
        <p>${this.translations.createPermanentDescription}</p>
        <span>${this.translations.exampleUsage}</span>
        <ul>
          <li>${this.translations.blogs}</li>
          <li>${this.translations.image}</li>
        </ul>
        <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.MAKE_PERMANENT)}
          >${this.translations.apply}</uui-button
        >
      </uui-box>
    `;
  }

  private renderIgnore() {
    return html`
      <uui-box .headline="${this.translations.ignore}">
        <p>${this.translations.ignoreDescription}</p>
        <uui-button look="primary" @click=${() => this.save(RECCOMENDATION_ACTIONS.IGNORE)}
          >${this.translations.apply}</uui-button
        >
      </uui-box>
    `;
  }

  protected render() {
    return this._translationTask.render({
      complete: () => html`
        <div class="header">${this._headerText}</div>
        <div class="main">
          ${this.renderTemporaryRedirect()} ${this.renderPermanentRedirect()} ${this.renderIgnore()}
        </div>
        <div class="footer">
          <uui-button look="default" color="default" @click=${this.close}>${this.translations.close}</uui-button>
        </div>
      `,
    });
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
