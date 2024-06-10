import { debounce } from '@/util/functions/debounce';
import { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';
import { LitElement, css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
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
import { ContentTargetResponse, RedirectStrategyResponse, getApiV1UrlTrackerRedirectStrategyContent } from '@/api';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { UMB_DOCUMENT_PICKER_MODAL } from '@umbraco-cms/backoffice/document';
import variableresourceService from '@/util/tools/variableresource.service';
import UrlTrackerRedirectTargetConstantsContext from '@/dashboard/tabs/redirects/target/api/redirecttargetconstants.context';
import { URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT } from '@/dashboard/tabs/redirects/target/api/redirecttargetconstants.contexttokens';
import { UmbArrayState, UmbBasicState } from '@umbraco-cms/backoffice/observable-api';

@customElement('urltracker-redirect-outgoing-url')
export class UrlTrackerRedirectOutgoingUrl extends UmbElementMixin(LitElement) {
  private modalManager?: UmbModalManagerContext;

  private _contentStrategyState: UmbBasicState<RedirectStrategyResponse | undefined> = new UmbBasicState<
    RedirectStrategyResponse | undefined
  >(undefined);
  private _contentStrategy?: RedirectStrategyResponse;

  private _urlStrategyState: UmbBasicState<RedirectStrategyResponse | undefined> = new UmbBasicState<
    RedirectStrategyResponse | undefined
  >(undefined);
  private _urlStrategy?: RedirectStrategyResponse;

  private _typeButtonsState: UmbArrayState<ITypeButton> = new UmbArrayState<ITypeButton>(
    [
      {
        label: 'urlTrackerRedirectTarget_content',
        labelFallback: 'Content',
        value: undefined,
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
        value: undefined,
        placeholder: 'https://example.com/',
        disabled: false,
      },
    ],
    (item) => item.label,
  );

  constructor() {
    super();

    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance?: UmbModalManagerContext) => {
      this.modalManager = instance;
    });

    this._typeButtonsState.asObservable().subscribe((value) => {
      this._typeButtons = value;
    });

    this._contentStrategyState.asObservable().subscribe((value) => {
      this._contentStrategy = value;
      if (!value) {
        return;
      }
      this._typeButtonsState.updateOne('urlTrackerRedirectTarget_content', {
        value: value.key,
      });
    });

    this._urlStrategyState.asObservable().subscribe((value) => {
      this._urlStrategy = value;
      if (!value) {
        return;
      }
      this._typeButtonsState.updateOne('urlTrackerRedirectTarget_url', {
        value: value.key,
      });
    });

    this.consumeContext(
      URLTRACKER_REDIRECT_TARGET_CONSTANTS_CONTEXT,
      (instance?: UrlTrackerRedirectTargetConstantsContext) => {
        instance?.getStrategyKey('content').subscribe((value) => {
          this._contentStrategyState.setValue(value);
        });
        instance?.getStrategyKey('url').subscribe((value) => {
          this._urlStrategyState.setValue(value);
        });
      },
    );
  }

  @property({ type: String })
  private outgoingUrl: string = '';

  @property({ type: String })
  private outgoingStrategy?: string;

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  private inputRef: Ref<HTMLInputElement> = createRef();

  public _typeButtons?: ITypeButton[];

  private contentItem: (ContentTargetResponse & { id: string }) | undefined = undefined;
  private url: string = '';

  @state()
  private _selectedType?: ITypeButton;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeInfoText();
    this._localizeButtonLabels();

    if (this._typeButtons) {
      this._selectedType =
        this._typeButtons.find((item) => item.value === this.outgoingStrategy) ??
        this._typeButtons.find((item) => item.value === this._contentStrategy?.key)!;
    }

    switch (this.outgoingStrategy) {
      case this._contentStrategy?.key:
        // eslint-disable-next-line no-case-declarations
        const [id, culture] = this.outgoingUrl.split(';');
        // eslint-disable-next-line no-case-declarations
        const intId = Number.parseInt(id, 10);

        if (!isNaN(intId)) {
          const { data } = await tryExecuteAndNotify(
            this,
            getApiV1UrlTrackerRedirectStrategyContent({
              id: id,
              culture: culture,
            }),
          );

          if (data) {
            this.contentItem = {
              ...data,
              id: id,
            };
          }
        }

        this.requestUpdate();
        break;
      case this._urlStrategy?.key:
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

  private _localizeButtonLabels = () => {
    this._typeButtons = this._typeButtons?.map((item, index) => ({
      ...item,
      label: this.localize.term(item.label) ?? item.labelFallback,
    }));
  };

  private openContentPicker = async () => {
    ensureServiceExists(this.modalManager, 'modalManager');
    const modal = this.modalManager.open(this, UMB_DOCUMENT_PICKER_MODAL);

    try {
      const result = await modal.onSubmit();
      this.submitContentPicker(result);
    } catch {
      /* Nothing to do when the modal result is rejected */
    }
  };

  private submitContentPicker = async (model: { selection: (string | null)[] }) => {
    if (model.selection.length === 0) return;

    const selectedItem = model.selection[0];
    if (!selectedItem) return;

    const { data, error } = await tryExecuteAndNotify(
      this,
      getApiV1UrlTrackerRedirectStrategyContent({ id: selectedItem }),
    );
    if (!data) {
      this.contentItem = undefined;
      return;
    }

    this.contentItem = {
      id: selectedItem,
      ...data,
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
      case this._contentStrategy?.key:
        this.onContentUpdate();
        this.url = '';
        break;
      case this._urlStrategy?.key:
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
    if (this._selectedType && this._selectedType.value === this._contentStrategy?.key) {
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
        .placeholder=${this._selectedType?.placeholder ?? ''}
        @input=${this._debouncedOnInput}
      ></uui-input>
    `;
  }

  protected render(): unknown {
    const typeButtonContent = !this._typeButtons
      ? nothing
      : html`<uui-button-group>
          ${repeat(
            this._typeButtons,
            (item) => item.value,
            (item) =>
              html` <uui-button
                label=${item.label}
                look=${this._selectedType?.value === item.value ? 'primary' : 'outline'}
                color="default"
                .disabled=${item.disabled}
                @click=${(e: Event) => this.onTypeChange(item, e)}
              ></uui-button>`,
          )}
        </uui-button-group>`;

    return html`
      <p>
        <strong>${this._headerText} <span class="required">*</span></strong>
      </p>
      <p>${this._infoText}</p>
      ${typeButtonContent} ${this.renderOutgoingStrategy()}
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
