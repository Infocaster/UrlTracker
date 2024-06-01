import { IRedirectResponse } from '@/services/redirect.service';
import { consume } from '@lit/context';
import { LitElement, css, html } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { IChangeManager, changeManagerContext } from '../../../../../context/changemanager.context';
import { IEditorService, editorServiceContext } from '../../../../../context/editorservice.context';
import { ITargetService, redirectTargetServiceContext } from '../../../../../context/redirecttargetservice.context';
import { ensureServiceExists } from '../../../../../util/tools/existancecheck';
import { IContentTargetResponse } from '../target.service';
import { UrlTrackerRedirectTarget } from '../targetbase.mixin';
import { ifDefined } from 'lit/directives/if-defined.js';
import { colors } from '@/dashboard/tabs/styles';

export class ContentUpdateEvent extends Event {
  static event = 'content-update';

  constructor(
    public contentId: string,
    public contentItem: IContentTargetResponse,
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
  @consume({ context: redirectTargetServiceContext })
  private redirectTargetService?: ITargetService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<IRedirectResponse>;

  @consume({ context: changeManagerContext })
  private changeManager?: IChangeManager;

  @state()
  private contentItem?: IContentTargetResponse;

  @state()
  private contentId?: string;

  @state()
  private loading: number = 0;

  @state()
  private errorText?: string;

  async connectedCallback(): Promise<void> {
    await super.connectedCallback();

    ensureServiceExists(this.changeManager, 'changeManager');
    this.changeManager.element.addEventListener(ContentUpdateEvent.event, this.onContentUpdate);

    if (this.redirect && 'content' in this.redirect.additionalData) {
      this.errorText = undefined;
      this.contentItem = this.redirect.additionalData.content as IContentTargetResponse;
      if (!this.contentItem) {
        this.errorText = await this.localizationService?.localize('urlTrackerRedirectTarget_contenterror');
      }
      const [id, culture] = this.redirect.target.value.split(';');
      this.contentId = id;
    } else {
      await this.init();
    }
  }

  disconnectedCallback(): void {
    super.disconnectedCallback();

    ensureServiceExists(this.changeManager, 'changeManager');
    this.changeManager.element.removeEventListener(ContentUpdateEvent.event, this.onContentUpdate);
  }

  private onContentUpdate = (e: Event) => {
    if (!(e instanceof ContentUpdateEvent)) return;
    if (e.contentId !== this.contentId) return;

    this.contentItem = e.contentItem;
  };

  private async init(): Promise<void> {
    ensureServiceExists(this.redirectTargetService, 'redirect target resource');
    ensureServiceExists(this.redirect, 'redirect');

    this.loading++;
    this.errorText = undefined;
    try {
      const [id, culture] = this.redirect.target.value.split(';');

      this.contentItem = await this.redirectTargetService.Content({
        id: Number.parseInt(id),
        culture: culture,
      });
      this.contentId = id;
    } catch {
      this.errorText = await this.localizationService?.localize('urlTrackerRedirectTarget_contenterror');
    } finally {
      this.loading--;
    }
  }

  private onClick = (e: Event) => {
    e.stopImmediatePropagation();
    ensureServiceExists(this.editorService, 'editor service');

    const onClose = async () => {
      this.editorService!.close();
      await this.init();
      this.dispatchEvent(new ContentUpdateEvent(this.contentId!, this.contentItem!));
    };

    this.editorService.contentEditor({
      id: this.contentId!,
      create: false,
      submit: onClose,
      close: onClose,
      documentTypeAlias: '',
      allowPublishAndClose: false,
      allowSaveAndClose: false,
      parentId: '',
    });
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
