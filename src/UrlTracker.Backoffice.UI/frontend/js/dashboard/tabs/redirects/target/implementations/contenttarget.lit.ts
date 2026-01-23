import { colors } from '@/dashboard/tabs/styles';
import { consume } from '@lit/context';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { LitElement, css, html } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import {
  ContentTargetResponse,
  getUmbracoManagementApiV1UrlTrackerRedirectTargetContent,
} from '../../../../../../../api-client';
import type { Client } from '../../../../../../../api-client/client/types.gen';
import { IChangeManager, changeManagerContext } from '../../../../../context/changemanager.context';
import { UrlTrackerRedirectTarget } from '../targetbase.mixin';

export class ContentUpdateEvent extends Event {
  static event = 'content-update';

  constructor(
    public contentId: string,
    public contentItem: ContentTargetResponse,
    eventInitDict?: EventInit,
  ) {
    super(ContentUpdateEvent.event, {
      bubbles: true,
      composed: true,
      ...eventInitDict,
    });
  }
}

const baseType = UrlTrackerRedirectTarget(LitElement, 'urlTrackerRedirectTarget_content');

@customElement('urltracker-redirect-target-content')
export class UrlTrackerContentRedirectTarget extends baseType {
  @consume({ context: changeManagerContext })
  private changeManager?: IChangeManager;

  @state()
  private contentItem?: ContentTargetResponse;

  @state()
  private contentId?: string;

  @state()
  private loading = 0;

  @state()
  private errorText?: string;

  async connectedCallback(): Promise<void> {
    await super.connectedCallback();

    this.changeManager?.element.addEventListener(ContentUpdateEvent.event, this.onContentUpdate);

    await this.init();
  }

  disconnectedCallback(): void {
    super.disconnectedCallback();

    this.changeManager?.element.removeEventListener(ContentUpdateEvent.event, this.onContentUpdate);
  }

  private onContentUpdate = (e: Event) => {
    if (!(e instanceof ContentUpdateEvent)) return;
    if (e.contentId !== this.contentId) return;

    this.contentItem = e.contentItem;
  };

  private async init(): Promise<void> {
    this.loading++;
    this.errorText = undefined;

    try {
      if (!this.redirect) throw new Error('No redirect available');
      if (!umbHttpClient) throw new Error('No HTTP client available');

      const [id, culture] = this.redirect.target.value.split(';');

      const { data } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerRedirectTargetContent({
          client: umbHttpClient as unknown as Client,
          query: {
            Id: id,
            Culture: culture,
          },
        }),
      );

      if (!data) throw new Error('Content item could not be found');

      this.contentItem = data;
      this.contentId = id;
    } catch (error) {
      this.errorText = this.localize.term('urlTrackerRedirectTarget_contenterror');
    } finally {
      this.loading--;
    }
  }

  private onClick = (e: Event) => {
    e.stopImmediatePropagation();

    const onClose = async () => {
      await this.init();
      this.dispatchEvent(new ContentUpdateEvent(this.contentId!, this.contentItem!));
    };

    // this.editorService.contentEditor({
    //   id: this.contentId!,
    //   create: false,
    //   submit: onClose,
    //   close: onClose,
    //   documentTypeAlias: '',
    //   allowPublishAndClose: false,
    //   allowSaveAndClose: false,
    //   parentId: '',
    // });
  };

  protected renderBody(): unknown {
    if (this.loading) return html`<uui-loader></uui-loader>`;
    if (this.errorText) return html`<span class="error">${this.errorText}</span>`;
    return html`
      <button @click="${this.onClick}">
        <uui-icon class=${ifDefined(this.contentItem?.iconColor)} name=${ifDefined(this.contentItem?.icon)}></uui-icon
        >${this.contentItem?.name}
      </button>
    `;
  }

  static styles = [
    ...baseType.styles,
    colors,
    css`
      uui-icon {
        align-self: center;
        margin-left: 4px;
      }

      .error {
        font-style: italic;
        color: var(--uui-color-danger);
      }

      :host {
        position: relative;
      }

      button {
        background: none;
        border: none;
        cursor: pointer;
        font-family: Lato, 'Helvetica Neue', Helvetica, Arial, sans-serif;
        font-size: 12px;
        line-height: 12px;
        padding: 0;
      }

      button:hover {
        text-decoration: underline;
      }

      button::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        bottom: 0;
        right: 0;
        z-index: 1000;
      }

      button uui-icon {
        margin-right: 4px;
      }
    `,
  ];
}
