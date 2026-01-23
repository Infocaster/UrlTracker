import { ITargetStrategies } from '@/dashboard/tabs/redirects/target/target.constants';
import { colors } from '@/dashboard/tabs/styles';
import { debounce } from '@/util/functions/debounce';
import variableresourceService from '@/util/tools/variableresource.service';
import {
  UMB_DOCUMENT_PICKER_MODAL,
  UmbDocumentDetailRepository,
  UmbDocumentItemModel,
  UmbDocumentItemRepository,
  UmbDocumentTreeRepository,
  UmbDocumentUrlRepository,
} from '@umbraco-cms/backoffice/document';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { UUIInputEvent } from '@umbraco-ui/uui-input';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { repeat } from 'lit/directives/repeat.js';
import {
  ContentTargetResponse,
  getUmbracoManagementApiV1UrlTrackerRedirectTargetContent,
} from '../../../../../../api-client';
import type { Client } from '../../../../../../api-client/client/types.gen';
import './simpleRedirectTypeProvider';
import { ITypeButton } from './simpleRedirectTypeProvider';

@customElement('urltracker-redirect-outgoing-url')
export class UrlTrackerRedirectOutgoingUrl extends UmbElementMixin(LitElement) {
  @property({ type: String })
  private outgoingUrl = '';

  @property({ type: String })
  private outgoingStrategy = 'url';

  @state()
  private _headerText = '';

  @state()
  private _infoText = '';

  @property({ type: Array })
  selection: Array<UmbDocumentItemModel> = [];

  private inputRef: Ref<HTMLInputElement> = createRef();

  #documentTreeRepository = new UmbDocumentTreeRepository(this);
  #documentItemRepository = new UmbDocumentItemRepository(this);
  #documentUrlRepository = new UmbDocumentUrlRepository(this);
  #documentDetailRepository = new UmbDocumentDetailRepository(this);

  public _typeButtons = [
    {
      label: this.localize.term('urlTrackerRedirectTarget_content'),
      labelFallback: 'Content',
      value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
      placeholder: 'link to content placeholder',
    },
    // {
    //   label: this.localize.term('urlTrackerRedirectTarget_media'),
    //   labelFallback: "Media",
    //   value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').media,
    //   placeholder: "link to media placeholder",
    // },
    {
      label: this.localize.term('urlTrackerRedirectTarget_url'),
      labelFallback: 'URL',
      value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url,
      placeholder: 'https://example.com/',
    },
  ] as ITypeButton[];

  private contentItem: (ContentTargetResponse & { id: string }) | undefined = undefined;
  private url = '';

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
        if (this.outgoingUrl) {
          try {
            // Legacy support: if it's in "id;culture" format
            const [id, culture] = this.outgoingUrl.split(';');

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

            this.contentItem = {
              id: id,
              name: data.name,
              icon: data.icon,
              url: data.url,
              iconColor: data.iconColor,
            };
          } catch (error) {
            console.warn('Failed to load existing content:', error);
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
    const labels = await this._typeButtons.map((item) => this.localize.term(item.label));

    this._typeButtons = this._typeButtons.map((item, index) => ({
      ...item,
      label: labels?.[index] ?? item.labelFallback,
    }));
  };

  private openContentPicker = async () => {
    const modalContext = await umbOpenModal(this, UMB_DOCUMENT_PICKER_MODAL, {
      data: {
        multiple: false,
        hideTreeRoot: true,
        treeAlias: 'Umb.Tree.Document',
        pickableFilter: (treeItem: UmbDocumentItemModel) => {
          return treeItem.unique !== null && treeItem.unique !== undefined;
        },
      },
    });

    const result = modalContext.selection.filter((item) => item !== null);

    if (!result) return console.warn('No result from content picker modal');

    if (result && result.length > 0) {
      await this.submitContentPicker(result);
    }
  };

  async #requestDocumentItem(unique: string) {
    if (!unique) throw new Error('Could not open permissions modal, no unique was provided');

    const { data } = await this.#documentItemRepository.requestItems([unique]);

    const documentItem = data?.[0];
    if (!documentItem) throw new Error('No document item found');
    return documentItem;
  }

  async #requestDocumentUrl(unique: string) {
    if (!unique) throw new Error('Could not open permissions modal, no unique was provided');

    const { data } = await this.#documentUrlRepository.requestItems([unique]);

    const documentItem = data?.[0];
    if (!documentItem) throw new Error('No document item found');
    return documentItem;
  }

  private submitContentPicker = async (selection: string[]) => {
    console.log(selection);
    if (!selection || selection.length === 0) {
      this.contentItem = undefined;
      this.onContentUpdate();
      return;
    }

    const selectedUniqueKey = selection[0]; // This is the GUID

    try {
      // Get document item details
      const documentItem = await this.#requestDocumentItem(selectedUniqueKey);

      if (!documentItem) {
        console.warn('No document item found for:', selectedUniqueKey);
        this.contentItem = undefined;
        this.onContentUpdate();
        return;
      }

      // Update selection for UI
      this.selection = [documentItem];
      if (!umbHttpClient) throw new Error('No HTTP client available');
      const { data } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerRedirectTargetContent({
          client: umbHttpClient as unknown as Client,
          query: {
            Id: selectedUniqueKey,
          },
        }),
      );

      if (!data) throw new Error('Content item could not be found');

      const targetInfo = data;

      this.contentItem = {
        id: selectedUniqueKey, // Use the GUID as the ID
        name: documentItem.name || '',
        icon: targetInfo?.icon || 'icon-document',
        url: targetInfo?.url || '',
        iconColor: targetInfo?.iconColor || '',
      };

      this.onContentUpdate();
      this.requestUpdate();
    } catch (error) {
      console.error('Error in submitContentPicker:', error);
      this.contentItem = undefined;
      this.onContentUpdate();
    }
  };

  private onContentUpdate = () => {
    this.dispatchEvent(
      new CustomEvent('input', {
        detail: this.contentItem?.id, // Send the GUID instead of numeric ID
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
    this.selection = [];
    this.onContentUpdate();
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
              <uui-button label="Remove" @click=${this.onDeleteContent}
                ><umb-localize key="urlTrackerGeneral_remove">Remove</umb-localize></uui-button
              >
            </uui-action-bar>
          </uui-ref-node-document-type>
        `;
      }

      return html`
        <uui-button class="w-100" look="placeholder" label="Select" @click=${this.openContentPicker}>
          <umb-localize key="urlTrackerGeneral_choose">Choose</umb-localize>
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
