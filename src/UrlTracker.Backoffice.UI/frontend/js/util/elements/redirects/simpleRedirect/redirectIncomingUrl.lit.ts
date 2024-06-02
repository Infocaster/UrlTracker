import { ISourceStrategies } from '@/dashboard/tabs/redirects/source/source.constants';
import { debounce } from '@/util/functions/debounce';
import variableresourceService from '@/util/tools/variableresource.service';
import { consume } from '@lit/context';
import { UUIInputElement, UUIInputEvent } from '@umbraco-ui/uui';
import { LitElement, css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { repeat } from 'lit/directives/repeat.js';
import { localizationServiceContext } from '../../../../context/localizationservice.context';
import { ILocalizationService } from '../../../../umbraco/localization.service';
import { ITypeButton } from './simpleRedirectTypeProvider';

@customElement('urltracker-redirect-incoming-url')
export class UrlTrackerRedirectIncomingUrl extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @property({ type: String })
  private incomingUrl: string = '';

  @property({ type: String })
  private incomingStrategy: string = 'url';

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
      disabled: false,
    },
    // {
    //   label: "urlTrackerNewRedirect_incoming-url-path",
    //   labelFallback: "Path",
    //   value: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').path,
    //   placeholder: "lorem/ipsum",
    //   disabled: false,
    // },
    {
      label: 'urlTrackerRedirectSource_regex',
      labelFallback: 'URL',
      value: variableresourceService.get<ISourceStrategies>('redirectSourceStrategies').regex,
      placeholder: '$[a-z]^',
      disabled: false,
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
    const text = await this._localizationService?.localize('urlTrackerNewRedirect_incoming-url');

    this._headerText = text ?? '';
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize('urlTrackerNewRedirect_incoming-url-info');

    this._infoText = text ?? '';
  };

  private _localizeButtonLabels = async () => {
    const labels = await this._localizationService?.localizeMany(this._typeButtons.map((item) => item.label));

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
              .disabled=${item.disabled}
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
