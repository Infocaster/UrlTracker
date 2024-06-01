import { IEditorService, editorServiceContext } from '@/context/editorservice.context';
import { ILocalizationService, localizationServiceContext } from '@/context/localizationservice.context';
import { ITargetService, redirectTargetServiceContext } from '@/context/redirecttargetservice.context';
import { ITargetStrategies } from '@/dashboard/tabs/redirects/target/target.constants';
import { IContentTargetResponse } from '@/dashboard/tabs/redirects/target/target.service';
import { IContent } from '@/umbraco/editor.service';
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

@customElement('urltracker-redirect-outgoing-url')
export class UrlTrackerRedirectOutgoingUrl extends LitElement {
  @property({ type: String })
  private outgoingUrl: string = '';

  @property({ type: String })
  private outgoingStrategy: string = 'url';

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: redirectTargetServiceContext })
  private redirectTargetService?: ITargetService;

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

  private contentItem: (IContentTargetResponse & { id: number }) | undefined = undefined;
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
        const [id, culture] = this.outgoingUrl.split(';');
        const intId = Number.parseInt(id, 10);

        if (!isNaN(intId)) {
          const content = await this.redirectTargetService!.Content({
            id: Number.parseInt(id, 10),
            culture: culture,
          });

          this.contentItem = {
            ...content,
            id: Number.parseInt(id, 10),
          };
        }

        this.requestUpdate();
        break;
      case variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url:
        this.url = this.outgoingUrl;
        break;
    }
  }

  private _localizeHeaderText = async () => {
    const text = await this._localizationService?.localize('urlTrackerNewRedirect_outgoing-url');

    this._headerText = text ?? 'Outgoing URL fallback';
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize('urlTrackerNewRedirect_outgoing-url-info');

    this._infoText = text ?? 'Select where the URL should redirect to';
  };

  private _localizeButtonLabels = async () => {
    const labels = await this._localizationService?.localizeMany(this._typeButtons.map((item) => item.label));

    this._typeButtons = this._typeButtons.map((item, index) => ({
      ...item,
      label: labels?.[index] ?? item.labelFallback,
    }));
  };

  private openContentPicker = () => {
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

  private onInput = (e?: UUIInputEvent) => {
    this.dispatchEvent(
      new CustomEvent('input', {
        detail: this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '',
        bubbles: true,
        composed: false,
      }),
    );
  };

  private _debouncedOnInput = debounce(this.onInput, 500);

  private onTypeChange = (item: ITypeButton, e: Event) => {
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
