import { ISourceStrategies } from '@/dashboard/tabs/redirects/source/source.constants';
import { debounce } from '@/util/functions/debounce';
import variableresourceService from '@/util/tools/variableresource.service';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UUIInputElement, UUIInputEvent } from '@umbraco-ui/uui-input';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { repeat } from 'lit/directives/repeat.js';
import { ITypeButton } from './simpleRedirectTypeProvider';

@customElement('urltracker-redirect-incoming-url')
export class UrlTrackerRedirectIncomingUrl extends UmbElementMixin(LitElement) {
  @property({ type: String })
  private incomingUrl: string = '';

  @property({ type: String })
  private incomingStrategy: string = 'url';

  @property({ type: Boolean })
  public disabled: boolean = false;

  @property({ type: Boolean })
  private advancedView: boolean = false;

  @state()
  private _headerText: string = '';

  @state()
  private _infoText: string = '';

  private inputRef: Ref<UUIInputElement> = createRef();

  public _typeButtons = [
    {
      label: 'urlTrackerRedirectSource_url',
      labelFallback: 'Content',
      value: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').url,
      placeholder: 'https://example.com/',
    },
    // {
    //   label: "urlTrackerNewRedirect_incoming-url-path",
    //   labelFallback: "Path",
    //   value: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').path,
    //   placeholder: "lorem/ipsum",
    // },
    {
      label: 'urlTrackerRedirectSource_regex',
      labelFallback: 'URL',
      value: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').regex,
      placeholder: '$[a-z]^',
    },
  ] as ITypeButton[];

  @state()
  private _selectedType: ITypeButton = this._typeButtons[0];

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    this._localizeHeaderText();
    this._localizeInfoText();
    this._localizeButtonLabels();
    //@TODO: Create Content and Media type redirect functionality. Only URL type is implemented.
  }

  private _localizeHeaderText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_incoming-url');

    this._headerText = text ?? '';
  };

  private _localizeInfoText = async () => {
    const text = this.localize.term('urlTrackerNewRedirect_incoming-url-info');

    this._infoText = text ?? '';
  };

  private _localizeButtonLabels = async () => {
    const labels = this._typeButtons.map((item) => item.label).map((label) => this.localize.term(label));

    this._typeButtons = this._typeButtons.map((item, index) => ({
      ...item,
      label: labels?.[index] ?? item.labelFallback,
    }));
  };

  private onInput = (_: UUIInputEvent) => {
    this.incomingUrl = this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '';
    this.dispatchEvent(
      new CustomEvent('input', {
        detail: this.incomingUrl,
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
  };

  protected renderIncomingStrategy(): unknown {
    if (!this.advancedView) {
      return nothing;
    }
    return html`
      <uui-button-group>
        ${repeat(
          this._typeButtons,
          (item) => item.value,
          (item) =>
            html` <uui-button
              label=${item.label}
              look=${this._selectedType.value === item.value ? 'primary' : 'outline'}
              color="default"
              .disabled=${this.disabled}
              @click=${(e: Event) => this.onTypeChange(item, e)}
            ></uui-button>`,
        )}
      </uui-button-group>
    `;
  }

  protected render(): unknown {
    return html`
      <p><strong>${this._headerText}</strong></p>
      <p>${this._infoText}</p>
      ${this.renderIncomingStrategy()}
      <uui-input
        ${ref(this.inputRef)}
        .value=${this.incomingUrl}
        .placeholder=${this._selectedType.placeholder}
        .disabled=${this.disabled}
        @input=${this._debouncedOnInput}
      ></uui-input>
    `;
  }

  static styles = [
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

      .required {
        color: #ba0000;
      }
    `,
  ];
}
