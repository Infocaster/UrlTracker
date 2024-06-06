import { ITargetStrategies } from '@/dashboard/tabs/redirects/target/target.constants';
import { debounce } from '@/util/functions/debounce';
import variableresourceService from '@/util/tools/variableresource.service';
import { consume } from '@lit/context';
import { UUIInputEvent } from '@umbraco-ui/uui';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { repeat } from 'lit/directives/repeat.js';
import './simpleRedirectTypeProvider';
import { ITypeButton } from './simpleRedirectTypeProvider';
import { ifDefined } from 'lit/directives/if-defined.js';
import { colors } from '@/dashboard/tabs/styles';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from '@umbraco-cms/backoffice/modal';
import { ensureServiceExists } from '@/util/tools/existancecheck';
import { UMB_TREE_PICKER_MODAL } from '@umbraco-cms/backoffice/tree';
import { ContentTargetResponse, getApiV1UrlTrackerRedirectTargetContent } from '@/api';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';

@customElement('urltracker-redirect-outgoing-url')
export class UrlTrackerRedirectOutgoingUrl extends UmbElementMixin(LitElement) {
  private modalManager?: UmbModalManagerContext;

  constructor() {
    super();

    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance?: UmbModalManagerContext) => {
      this.modalManager = instance;
    });
  }

  @property({ type: String })
  private outgoingUrl: string = '';

  @property({ type: String })
  private outgoingStrategy: string = 'url';

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  private inputRef: Ref<HTMLInputElement> = createRef();

  public _typeButtons = [
    {
      label: 'urlTrackerRedirectTarget_content',
      labelFallback: 'Content',
      value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
      placeholder: 'link to content placeholder',
      disabled: false,
    },
    // {
    //   label: "urlTrackerRedirectTarget_media",
    //   labelFallback: "Media",
    //   value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').media,
    //   placeholder: "link to media placeholder",
    //   disabled: true,
    // },
    {
      label: 'urlTrackerRedirectTarget_url',
      labelFallback: 'URL',
      value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url,
      placeholder: 'https://example.com/',
      disabled: false,
    },
  ] as ITypeButton[];

  private contentItem: (ContentTargetResponse & { id: number }) | undefined = undefined;
  private url: string = '';

  @state()
  private _selectedType: ITypeButton = this._typeButtons[0];

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeInfoText();
    this._localizeButtonLabels();

    this._selectedType =
      this._typeButtons.find((item) => item.value === this.outgoingStrategy) ??
      this._typeButtons.find(
        (item) => item.value === variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url,
      )!;

    switch (this.outgoingStrategy) {
      case variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content:
        // eslint-disable-next-line no-case-declarations
        const [id, culture] = this.outgoingUrl.split(';');
        // eslint-disable-next-line no-case-declarations
        const intId = Number.parseInt(id, 10);

        if (!isNaN(intId)) {
          const { data, error } = await tryExecuteAndNotify(
            this,
            getApiV1UrlTrackerRedirectTargetContent({
              id: Number.parseInt(id, 10),
              culture: culture,
            }),
          );

          if (data) {
            this.contentItem = {
              ...data,
              id: Number.parseInt(id, 10),
            };
          }
        }

        this.requestUpdate();
        break;
      case variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url:
        this.url = this.outgoingUrl;
        break;
    }
  }

  private _localizeHeaderText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_outgoing-url');

    this._headerText = text ?? 'Outgoing URL fallback';
  };

  private _localizeInfoText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_outgoing-url-info');

    this._infoText = text ?? 'Select where the URL should redirect to';
  };

  private _localizeButtonLabels = async () => {
    this._typeButtons = this._typeButtons.map((item, index) => ({
      ...item,
      label: this.localize.term(item.label) ?? item.labelFallback,
    }));
  };

  private openContentPicker = () => {
    ensureServiceExists(this.modalManager, 'modalManager');
    this.modalManager.open(this, UMB_TREE_PICKER_MODAL, {
      data: {},
    });

    this.editorService?.contentPicker({
      multiPicker: false,
      submit: this.submitContentPicker,
      close: () => this.editorService?.close(),
    });
  };

  private submitContentPicker = async (model: { selection: IContent[] }) => {
    this.editorService?.close();

    if (model.selection.length === 0) return;

    const selectedItem = model.selection[0];
    const newTarget = await this.redirectTargetService?.Content({ id: selectedItem.id });
    if (!newTarget) {
      this.contentItem = undefined;
      return;
    }

    this.contentItem = {
      id: selectedItem.id,
      ...newTarget,
    };
    this.onContentUpdate();
    this.requestUpdate();
  };

  private onContentUpdate = () => {
    this.dispatchEvent(
      new CustomEvent('input', {
        detail: this.contentItem?.id,
        bubbles: true,
        composed: false,
      }),
    );
  };

  private onInput = (_?: UUIInputEvent) => {
    this.dispatchEvent(
      new CustomEvent('input', {
        detail: this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '',
        bubbles: true,
        composed: false,
      }),
    );
  };

  private _debouncedOnInput = debounce(this.onInput, 500);

  private onTypeChange = (item: ITypeButton, _: Event) => {
    this._selectedType = item;

    this.dispatchEvent(
      new CustomEvent('typechange', {
        detail: item,
        bubbles: true,
        composed: false,
      }),
    );

    switch (item.value) {
      case variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content:
        this.onContentUpdate();
        this.url = '';
        break;
      case variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url:
        this.onInput();
        this.contentItem = undefined;
        break;
    }
  };

  private onDeleteContent = () => {
    this.contentItem = undefined;
    this.requestUpdate();
  };

  protected renderOutgoingStrategy(): unknown {
    if (
      this._selectedType.value === variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content
    ) {
      if (this.contentItem) {
        return html`
          <uui-ref-node-document-type
            standalone
            .name=${this.contentItem.name}
            .detail=${this.contentItem.url ?? ''}
            class="w-100"
          >
            <uui-icon
              slot="icon"
              .name=${this.contentItem.icon}
              class=${ifDefined(this.contentItem.iconColor)}
            ></uui-icon>
            <uui-action-bar slot="actions">
              <uui-button label="Remove" @click=${this.onDeleteContent}> Remove </uui-button>
            </uui-action-bar>
          </uui-ref-node-document-type>
        `;
      }

      return html`
        <uui-button class="w-100" look="placeholder" label="Toevoegen" @click=${this.openContentPicker}>
          Toevoegen
        </uui-button>
      `;
    }

    return html`
      <uui-input
        ${ref(this.inputRef)}
        .value=${this.url}
        .placeholder=${this._selectedType.placeholder}
        @input=${this._debouncedOnInput}
      ></uui-input>
    `;
  }

  protected render(): unknown {
    return html`
      <p>
        <strong>${this._headerText} <span class="required">*</span></strong>
      </p>
      <p>${this._infoText}</p>
      <uui-button-group>
        ${repeat(
          this._typeButtons,
          (item) => item.value,
          (item) =>
            html` <uui-button
              label=${item.label}
              look=${this._selectedType.value === item.value ? 'primary' : 'outline'}
              color="default"
              .disabled=${item.disabled}
              @click=${(e: Event) => this.onTypeChange(item, e)}
            ></uui-button>`,
        )}
      </uui-button-group>
      ${this.renderOutgoingStrategy()}
    `;
  }

  static styles = [
    colors,
    css`
      :host {
        display: block;
      }

      uui-button-group {
        width: 100%;
        margin-bottom: 1rem;
        flex-wrap: wrap;
      }

      uui-input {
        width: 100%;
      }

      .w-100 {
        width: 100%;
      }

      .required {
        color: #ba0000;
      }
    `,
  ];
}
